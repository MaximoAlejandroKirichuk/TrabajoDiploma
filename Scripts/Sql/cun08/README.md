# Deploy and test CUN08 evaluation results

CUN08 lets the assigned professor stage a subset of grades in memory, then register that batch atomically. Results and DVH/DVV commit together; the audit event follows commit. Saved results are immutable in this screen. No notifications, certificates, payment writes or cross-date grade averaging are included.

**Stored-result corrections now have a separate screen:** **Cobros, Morosidad y Actas → Evaluaciones**. Permissions 73 (view) and 74 (modify) are additive and never auto-granted. Corrections are allowed only before any certificate is issued for the enrollment; only grade, date and observations are editable. See the [Spanish deployment, acceptance and code-to-DSS guide](EVALUACIONES_MANUAL.md), including the required `006_evaluaciones.sql` migration after CUN08/CUN09. The intake screen remains insert-only.

Every course has **exactly two assessments** per enrollment, not a configurable count. CUN08 permits first and second results on distinct dates, never a third. **Rerun the updated `001_migracion.sql` even on existing CUN08 installations** to install SQL-level protection; opening the screen does not migrate an existing database.

## Quick path: an existing, current database

1. **Back up the selected application database** and verify that the backup can be restored. Stop all application writers while migrating. Do not execute the full schema against an existing database.
2. Desde la raíz del repositorio, ejecutar la migración (sustituir `SERVER` y `DATABASE`):

   ```powershell
   sqlcmd -S "SERVER" -d "DATABASE" -E -b -f 65001 -i "Scripts/Sql/cun08/001_migracion.sql"
   ```

3. Build `Solucion.sln`, assign permission **71 — Registrar resultados de evaluación** through role/family administration, and sign out/in to refresh session permissions.
4. Seguir los casos manuales siguientes. `rollback.sql` **no** forma parte del despliegue. Los ejecutores y conjuntos de pruebas CUN08/CUN09/Evaluaciones fueron retirados por decisión del usuario; no se incluyen datos de demostración.

Prerequisites: SQL Server 2017+ (SHA2_256, SESSION_CONTEXT, TRIM), Windows integrated authentication, base commission/student/security schema, DVH/DVV infrastructure, and the current financing model (`Cuota.TipoConcepto`, enrollment financing snapshots, etc.). The migration rejects missing prerequisites and patent-71 collisions; it never assigns the patent to an administrator or professor automatically. A rerun preserves result data and hashes, rather than hiding tampering with a backfill.

### Older existing databases

If the financing update has not been installed, first back up, stop writers, and run the **existing** installer from the repository root:

```powershell
sqlcmd -S "SERVER" -d "DATABASE" -E -b -f 65001 -i "Scripts/Sql/financiacion-cuotas/aplicar.sql"
```

Ese instalador incluye, en orden: `financiacion-cuotas/001_modelo.sql`, `cun03/001_migracion.sql`, `cun05/001_migracion.sql`, `cun06/001_migracion.sql`, `reporte-ocupacion-comisiones/001_migracion.sql`, `cun07/001_migracion.sql`, `financiacion-cuotas/002_completar_inscripciones.sql`. No sustituir sus dependencias por una cadena supuesta. Después ejecutar CUN08 `001_migracion.sql` y realizar la verificación manual siguiente. En SSMS, el instalador existente necesita modo SQLCMD y `RepoRoot` configurado con la ruta del repositorio; la migración independiente de CUN08 no utiliza directivas SQLCMD.

### New database

For a genuinely new installation, confirm `InstitutoVisionEnAccion` does **not** already exist. `EsquemaCompleto.sql` explicitly creates and switches to that name; `-d` does not rename its target. Never use it as an upgrade script.

```powershell
sqlcmd -S "SERVER" -E -b -f 65001 -i "Scripts/Sql/EsquemaCompleto.sql"
sqlcmd -S "SERVER" -d "InstitutoVisionEnAccion" -E -b -f 65001 -i "Scripts/Sql/financiacion-cuotas/aplicar.sql"
sqlcmd -S "SERVER" -d "InstitutoVisionEnAccion" -E -b -f 65001 -i "Scripts/Sql/cun08/001_migracion.sql"
```

Review/secure the historical accounts and sample content supplied by the existing full schema; this change adds no known login credentials. Verify the effective application connection before testing. The current fresh-database bootstrap executes the financing chain, CUN08, CUN09 and Evaluaciones permissions; existing databases still require explicit migration. Merely opening a screen does not upgrade a database.

## Identity, course and eligibility mapping

| UI/domain term | Actual source |
|---|---|
| Course selector | Assigned course name, deduplicated by **`Curso.IdCurso`** across the logged-in professor's commissions. The positive ID remains internal; there is no `Curso.Codigo`. |
| Commission selector | **`Comision.Codigo`** with start/end dates, filtered by the selected course and session professor, and only commissions already started (`FechaInicio <= database today`). Neither selector accepts free text. |
| Professor | Session user DNI, matched as an exact invariant string to active `Profesor.DNI`. No UI-supplied professor identity; no numeric casting of database DNI strings. Leading zeros, whitespace variants and a different DNI do not match. |
| Al día | Current CUN07 standing: an academically active enrollment with **no overdue positive installment balance**, evaluated using database-local today. Due today and future installments do not block. Legacy `Al día` / `Con deuda` enrollment-state spellings are active equivalents, but never bypass the balance check. |
| Loading date | Results are loaded **during the course**: the commission must have started, but it does not need to have finished. `FechaLimitePago` is irrelevant to this gate. |
| Exam date | Date-only, within `FechaInicio` through `FechaFin`, including both endpoints, and **not later than database-local today** (`Cun08.FechaFutura`). |

**Design adaptation:** financing migration `002_completar_inscripciones.sql` restores legacy financial states to `activa`; CUN07 derives administrative standing from installment balances. A literal `Estado='Al día'` filter would exclude normal current enrollments. CUN08 reuses that existing standing instead of reintroducing a financial state into the academic column. It does not collect payments or alter balances. Authorization, dates, standing and duplicates are checked again under transactional locks before any result is inserted.

### Set up a manual sandbox using normal application operations

- In user/role administration, create or choose an active, unblocked user. Create/choose an active professor whose DNI is **exactly the same digits**, without padding/leading zeros. Grant patent 71 directly or through a family, then sign out/in. Merely being an administrator does not make the user an assigned professor.
- Choose a commission assigned to that professor. Its course name and commission code/date range will appear in the related selectors. Use a sandbox commission with `FechaInicio <= database today` (finished or still in progress). Keep its normal payment deadline before course start; do not use an invalid payment deadline just to test CUN08.
- Prepare several active enrollments through existing enrollment/payment flows. At least two should have no overdue balance; another should have an overdue unpaid course installment. An unpaid **future** installment is suitable to prove that Al día does not mean fully paid.
- Verificar identificadores, estados académicos, fechas y situación administrativa mediante las consultas y pantallas existentes. No cargar datos sintéticos en la base operativa.

## Manual acceptance checklist (Spanish / English)

Open **RF2: Cobros, Morosidad y Actas → Registrar resultados de evaluación** / **RF2: Payments, Delinquency and Minutes → Register evaluation results**. Permission 71 alone exposes this RF2 parent/child, not RF1 or the payment siblings. No other screen was moved.

| Case | Steps and expected result |
|---|---|
| Happy path | Select an assigned course, then its commission by code/date range, and enter an exam date in range. Click **Buscar / Search**. Verify DNI/name/surname and that overdue students are absent. Select a row, enter `7` and blank observations, then **Agregar / corregir provisorio / Add / correct provisional result**. No result exists in SQL yet. Confirm **Registrar resultados definitivamente / Register results definitively**. One result with `Aprobado` is saved. |
| Subset and resume | Grade only one of several students, commit, then search the **same** commission/date. The saved student is absent and the remaining eligible students are still offered. A different date permits the second result; after two persisted results the enrollment is absent for every date. |
| Correct before saving | Stage `6.99`, select that student again, replace it with `7`, and add/correct. Only one staged row remains. **Quitar / Remove** removes a provisional grade, not a saved result. Empty staging disables definitive registration. |
| Selectors and empty state | Initially no course/commission is selected and **Buscar / Search** is disabled. Selecting a course lists only its assigned commissions and still requires a commission choice. With no assignments, show a localized empty message and disable search. No general commission/course-management permission is required. Only assigned commissions that have already started (`FechaInicio <= database today`) are listed, including commissions still in progress. The exam date is never automatically overwritten. |
| Invalid inputs | Try blank grade, `0.99`, `10.01`, `7.001` and an exam date outside the selected commission's range. No writes; valid provisional rows remain after validation failures. Use the Windows regional decimal separator (`6,99` or `6.99`); changing application language does not change the Windows numeric culture. Observations are optional, maximum 500 characters. |
| Boundaries | Grades `1`, `6.99`, `7`, `10` yield `Desaprobado`, `Desaprobado`, `Aprobado`, `Aprobado`. In a finished commission, test exam dates exactly at course start/end and one day outside each. In a commission still in progress, an exam date of today or earlier (within the range) is allowed and tomorrow is rejected as a future date, irrespective of payment deadline. |
| Criteria changes | Change course/commission/date after staging: definitive registration is disabled until criteria match the loaded search. Selection changes and assignment-list reloads retain staged rows, even if an assignment disappears. Reload preserves selections that still exist. A new search or closing asks before discarding staged rows; cancel keeps them, and a failed search also preserves them. Unsaved editor text must be explicitly added to staging. |
| Security | A user without 71 cannot see/open the menu. A professor with 71 but not assigned to the commission cannot search or register. Test missing professor mapping, leading-zero DNI, inactive/blocked user, inactive professor, and revoked permission or changed assignment after a valid search: no students/results are exposed/saved by the stale operation. |
| Concurrent duplicate | In two app instances, search and stage the same student/date. Commit one, then the other: only one result exists; the rejected instance retains provisional data for review. Refresh/re-search to resume. |
| Concurrent third | Start with one result; two sessions stage different dates. Only one may add the second result, leaving exactly two. Direct SQL and multirow statements are also capped by the trigger. |
| Audit warning | Las pruebas históricas inyectaron un fallo de auditoría después del commit. Solo en un entorno manual desechable y con un mecanismo autorizado disponible: la confirmación debe indicar que las notas **ya están guardadas**, no sugerir reintentar y limpiar los provisorios. No revocar permisos de producción para simularlo. |
| Localization | Repeat in Spanish and English, including menu, headings, buttons, validation, confirmation and audit warning. Condition values remain the persisted Spanish domain values. |

## SQL verification after a manual save

Replace the parameters with the actual IDs/date; run in the same database selected in the app:

```sql
DECLARE @comision int = 1, @fecha date = '20261001'; -- Sustituir ambos valores.
SELECT r.IdResultadoEvaluacion, r.IdInscripcion, a.DNI, a.Apellido, a.Nombre,
       r.FechaEvaluacion, r.Calificacion, r.Observaciones, r.CondicionFinal, r.DVH
FROM dbo.ResultadoEvaluacion r
JOIN dbo.SolicitudInscripcion si ON si.IdSolicitudInscripcion=r.IdInscripcion
JOIN dbo.Alumno a ON a.IdAlumno=si.IdAlumno
WHERE r.IdComision=@comision AND r.FechaEvaluacion=@fecha;
SELECT NombreTabla,DVV,FechaActualizacion FROM dbo.DigitoVerificador_83KI
WHERE NombreTabla=N'ResultadoEvaluacion';
SELECT TOP (20) * FROM dbo.BitacoraEventos
WHERE Descripcion LIKE N'CUN08:%' ORDER BY Fecha DESC;
```

Comparar cantidades antes de preparar provisorios, después de prepararlos (sin cambios) y después de registrar definitivamente (solo el subconjunto elegido). La consulta de hashes almacenados no demuestra integridad: verificar DVH canónicos, DVV agregado, unicidad, relaciones y correspondencia nota/condición mediante las herramientas de integridad existentes. Ante diferencias, investigar su procedencia; no recalcular hashes para ocultarlas.

### SQL cap and legacy history

`TR_ResultadoEvaluacion_MaximoDos` is an **AFTER INSERT/UPDATE** trigger. It visits distinct affected enrollment IDs in ascending order, takes `UPDLOCK,HOLDLOCK` on each parent enrollment, then counts that enrollment's results with `UPDLOCK,HOLDLOCK`. A count above two throws 52815 and rolls back the statement/transaction. Reassigning an enrollment is checked too; ordinary DVH maintenance does not revalidate historical counts. The original identity, defaults, per-row condition and unique enrollment/date constraint are unchanged. No assessment-number column is introduced.

**Concurrency proof:** two successful writers for the same enrollment cannot both hold the incompatible parent update lock. The lock is retained until transaction completion, and the count uses locking reads, not a stale snapshot. The second successful checker therefore includes the first committed insertion and rejects a third result. With an AFTER trigger, a direct SQL writer may already hold a child insert lock while waiting for the parent: a cycle can cause SQL deadlock 1205, whose victim is rolled back. That is a failed write, not a third committed result. Normal CUN08 writes retain their existing application lock plus parent locks and atomic DVH/DVV maintenance. The cap alone does **not** maintain hashes for arbitrary direct SQL; supported writes must still use the integrity boundary. Privileged trigger disabling or bulk import without firing triggers is outside this protection.

La migración no modifica ni rechaza históricos con más de dos notas y no repara sus hashes. Se pueden identificar sin escrituras mediante `SELECT IdInscripcion, COUNT(*) AS Cantidad FROM dbo.ResultadoEvaluacion GROUP BY IdInscripcion HAVING COUNT(*) > 2;`. No admiten evaluaciones adicionales y CUN09 los excluye/rechaza como inconsistentes, sin elegir ni eliminar notas. Las instantáneas de certificados emitidos permanecen intactas.

## Compilación y evidencia histórica

Los ejecutores automatizados y los conjuntos de datos CUN08/CUN09/Evaluaciones fueron eliminados por decisión del usuario. La aceptación actual utiliza los casos manuales anteriores y las guías conservadas; no hay comandos de esos ejecutores disponibles en el repositorio. La compilación requiere Visual Studio MSBuild y el paquete de desarrollo .NET Framework 4.7.2:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Solucion.sln" /t:Build /p:Configuration=Debug /v:minimal
```

Las ejecuciones descritas a continuación ocurrieron **antes de retirar las pruebas**. Son evidencia histórica, no una nueva ejecución ni una garantía de reproducibilidad con los archivos actuales. Cubrieron autorización, fechas, notas, subconjuntos, reanudación, preservación de provisorios, auditoría posterior al commit, selectores y menú bilingüe; las integraciones utilizaron DAL real y bases LocalDB desechables. No utilizaron ni cambiaron la conexión persistida de la aplicación.

Para preparar casos manuales, mantener el modelo real de financiación: matrícula como cuota 1 sin período; cuotas de curso desde el número 2, con `Periodo` al inicio del mes. No debilitar `CK_Cuota_ConceptoPeriodo` ni alterar el comportamiento de pagos.

### Execution evidence for this apply batch

**Incremental selector/RF2 update (2026-10-22):** the normal Debug build encountered MSB3021/MSB3027 because the running UI/debugger locked `UI/bin/Debug` assemblies. No processes were stopped. A solution build with isolated output passed for all five projects:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Solucion.sln" /t:Build /p:Configuration=Debug /p:OutDir="C:\Users\User\AppData\Local\Temp\opencode\cun08-ux-build\" /v:minimal
```

Antes de su retirada, las pruebas BLL/UI y de integración pasaron, incluidas las firmas DC/DSS y la consulta real de selectores para dos profesores, mapeo inactivo, reasignación y coincidencia exacta de DNI. El arnés eliminó `Cun08Tests_ea12b35d961c442f8fb2b71c0bbd78fc`. No cambió la base de la aplicación ni su configuración persistida. Esta evidencia no sustituye una revisión independiente ni la aceptación visual interactiva.

**Earlier CUN08 implementation evidence (retained):**

- Solution build: passed (all five projects), after correcting a missing observer import.
- Default `Cun08.Tests.ps1`: passed; both language JSON files parsed. Test harness load/DI issues were corrected without global changes.
- Initial apply did not execute SQL. Independent verification then found the invalid course-installment fixture (`CK_Cuota_ConceptoPeriodo`). Remediation corrected its installment number/period without modifying production constraints.
- Subsequent real integration exposed parameter ownership reuse in the result-integrity helper and SQL Server's restriction on plain `OUTPUT` with enabled triggers. Remediation allocates fresh update parameters and uses `OUTPUT INTO` for identity retrieval; insert/integrity/commit/audit order is unchanged.
- Integración histórica: **PASS** en `(localdb)\MSSQLLocalDB`, con reversión real de inserción/integridad, revalidación de permisos/usuario/profesor/asignación/elegibilidad, variantes de DNI, duplicados concurrentes, reanudación en la misma fecha, restricciones, hashes C# y reejecución vacía/poblada con DVH/DVV SQL. El ejecutor confirmó la eliminación de `Cun08Tests_d9c51fa05be649e0b6b9d61b5b465dc6`. No se modificó la base del usuario ni su configuración persistida.
- Default tests also assert the exact DC operation parameter/return types and visibility through reflection. Interactive manual UI/database acceptance and coverage measurement remain outstanding. The original independent FAIL report remains historical evidence and must be refreshed independently; an apply-phase pass is not a replacement review.

### Explicit DC/DSS mapping

Compared directly with `DC/CUN08-DC-RegistrarResultadosEvaluacion.js` (class definitions, lines 29–194) and `DC/CUN08-DSS-RegistrarResultadosEvaluacion.plantuml` (messages, lines 19–89):

| Reference participant | Implemented operations (same parameter/return types and visibility) |
|---|---|
| `FrmRegistrarResultadosEvaluacion_83KI` | Public `BuscarAlumnosHabilitados(): void`, `SeleccionarAlumno(int): void`, `AgregarResultadoProvisorio(ResultadoAlumnoRequest_83KI): void`, `ActualizarEstadoBotonRegistrar(): void`, `RegistrarResultadosDefinitivamente(): void`, `MostrarError(string): void`, `MostrarConfirmacion(): void`. Named provisional fields remain form memory only. |
| `KI68ResultadoEvaluacionBLL` | Public `BuscarAlumnosHabilitados(BusquedaResultadoEvaluacionRequest_83KI): List<AlumnoEvaluacionListado_83KI>` and `RegistrarResultados(ResultadoEvaluacionRequest_83KI): void`; private `ValidarDatos(request): void`, `ValidarProfesorYComision(int): Comision_83KI`, `ValidarAlumnosHabilitados(request): void`, `DeterminarCondicionFinal(decimal): string`, `RegistrarAuditoria(request): void`. Here `request` is `ResultadoEvaluacionRequest_83KI`. |
| `KI68ComisionBLL` | Public `ObtenerComisionPorCursoYNumero(string,string): Comision_83KI`, `ValidarProfesorAsignado(int,int): void`. |
| `IResultadoEvaluacionDAL_83KI` / `ResultadoEvaluacionDAL_83KI` | Public `BuscarAlumnosHabilitados(int,DateTime): List<AlumnoEvaluacionListado_83KI>`, `ExisteResultado(int,DateTime): bool`, `RegistrarResultados(ResultadoEvaluacionRequest_83KI): void`; DAL private `RecomputarIntegridadResultados(List<int>,SqlConnection,SqlTransaction): void`. |
| `IntegridadBLL_83KI` | Public `ActualizarDVHyDVVResultados(List<int>,SqlConnection,SqlTransaction): void`, delegating to the DAL on the supplied transaction. |
| `IIntegridadDAL_83KI` / `IntegridadDAL_83KI` | Public `ActualizarDVHyDVVTransaccional(string,Dictionary<string,object>,string,List<SqlParameter>,SqlConnection,SqlTransaction): void`. |
| Session / audit / result entity | Reuse `UsuarioActivo`, `TienePermiso(PermisoSistema_83KI): bool`, `RegistrarEvento(BitacoraEvento_83KI): void`; entity `DeterminarCondicionFinal(): string`. DTOs retain all DC properties; additive `IdComision`/`AuditoriaFallida` support selection and committed-warning reporting. |

**Implementation adaptations, not literal diagram replicas:** the design deliberately uses a factory-injected integrity callback rather than the DC's concrete `_integridad`/`_integridadDAL` fields. The callback still enters `IntegridadBLL_83KI` before commit. Academic `KI68BaseDeDatos` aliases existing `AccesoDAL_83KI`; `BeginTransaction`/`CommitTransaction` are lifecycle steps inside `EjecutarTransaccion`, not newly invented public methods. Result insertion uses its scalar transactional SQL helper to retrieve generated IDs (`OUTPUT INTO`), while integrity uses `EscribirTransaccional`. Search/prevalidation reads use read-only transactions for safe authorization/eligibility reads; no business-write transaction starts until BLL validation succeeds. Error rendering adds localized exception handling without replacing any named DC form operation.

The preserved write sequence is **validate → begin write transaction → locked revalidation → selected result inserts/conditions → integrity BLL/DAL on the same connection/transaction → commit → audit → form confirmation**. Invalid/failed batches do not audit; committed audit failures warn and clear staging. No DC/DSS artifacts were rewritten to conceal differences.

## Rollback and review boundary

Revert application changes as one CUN08 work unit. With writers stopped and a verified backup, `rollback.sql` removes the additive table only when **empty**; otherwise it refuses without deleting results. It removes patent 71 and its grants only when its expected feature name matches, updates affected permission DVVs, and removes CUN08's parent unique index. If grades exist, retain the additive schema or use an approved backup recovery plan; never delete grades to force rollback.

Revisar flujo BLL/DC → transacción y bloqueos DAL → migración e integridad → formulario de provisorios y menú → aceptación manual. Se conservan las operaciones DC/DSS; `AccesoDAL_83KI.EjecutarTransaccion` mantiene el ciclo real de inicio/commit/rollback y el callback BLL inyectado conecta la integridad. Esta limpieza no modifica la arquitectura, la lógica ni las migraciones y no crea commits ni PR.

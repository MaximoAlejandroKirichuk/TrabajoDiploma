# CUN09 — Issue and save completion certificates

Academic coordinators can issue one certificate per enrollment after the commission ends, **exactly two assessments are recorded, each grade is at least 7**, and the **entire** outstanding balance is zero. The average is informational only. Delivery email and mode remain metadata: sending/delivery registration changes are outside this batch; the application does not email or print on paper.

**Guía en español:** [pruebas manuales y mapa código → DSS](PRUEBAS_MANUALES.md). Incluye preparación segura, casos de aceptación, verificaciones de persistencia y límites de la evidencia automatizada.

## Installation

1. Back up the database and stop application writers. Existing databases must already have the current financing installer and CUN08 migration applied; see `../cun08/README.md`. Do not run test fixtures on a user database.
2. Apply the additive migration to the intended database (replace both placeholders):

   ```powershell
   sqlcmd -S "SERVER" -d "DATABASE" -E -b -f 65001 -i "Scripts/Sql/cun09/001_migracion.sql"
   ```

3. Assign **72 — Generar certificado de finalización** to the academic coordinator's role/family through existing permission administration. No role receives this permission automatically. Sign out/in to refresh session permissions.
4. Build and open **Planificación Académica → Generar certificado de finalización**. `Microsoft Print to PDF` must be installed for PDF saving; issuance itself does not require that printer.

New-database installation through the application now packages and executes the financing dependency chain followed by CUN08/CUN09. Connection validation does **not** migrate an existing database. No migration is automatically executed merely by opening this screen.

## Operator flow

- Opening lists commissions whose end **date** is today or earlier; **Buscar** separately loads enrollments with **exactly two** persisted assessments. Assessment 1/2 follow ascending assessment date (dates are unique per enrollment). DNI, name/surname, both grades, informational mean and condition are displayed. Both grades must be >=7: `6+8` (mean 7) and `10+6` (mean 8) fail; `7+7` passes.
- Select a student. An approved, debt-free enrollment prefills name, surname, course, average and issuance date. Signer/title come from the active session and are read-only. Review **Prefijo del folio**, enter only the positive integer in **Número de folio**, and complete delivery mode and valid email; mention and observations are optional. There is no default number, counter or reservation.
- An existing certificate shows original, read-only metadata and enables **Guardar certificado**. If another session issues first, the original wins; the newly entered folio is not assigned.
- Pending balances include future installments. Failed students also show available professor observations. Withholding reasons are automatic. If eligibility changes after selection, **Retener** refreshes without logging; another click is required if reasons remain. Successful withholding logs enrollment, DNI, derived reasons and observations, then closes the screen.
- **Guardar certificado** opens a PDF-only save dialog with a sanitized student/course filename. Cancel or failure preserves the committed certificate and allows retry. The coordinator delivers externally to the recorded email.

### Folio input and normalization

The BLL composes `CommissionCode-NormalizedCourseName-CommissionEndDateYYYYMMDD-Number`. Example: commission `C08D-HOY`, course `Tárot inicial`, end date October 25, 2026 and number `12` produce `C08D-HOY-TAROT-INICIAL-20261025-0012`. The date is the commission's `FechaFin`, **not** today's date or the issuance date. Commission code is preserved exactly, including existing hyphens.

Course normalization uses Unicode canonical decomposition, removes combining marks, uppercases invariantly and retains ASCII `A-Z`/`0-9`. Every run of other characters (spaces, hyphens, punctuation, path characters or unsupported letters) becomes one internal hyphen; leading/trailing separators are omitted. An empty normalized course is rejected. No component is truncated. The full folio must fit the existing 200-character column; otherwise request a master-data review, not a shorter arbitrary folio.

Only ASCII digits are accepted for the manually chosen number, range **1–9223372036854775807 (Int64)**. Empty, zero, signs, spaces, fractions, scientific notation and overflow are rejected before writing. Leading zeros canonicalize: `12`, `0012` and `00012` all become `0012`; `1` becomes `0001`, and `10000` stays `10000`. The existing SQL unique constraint checks the **whole** folio institution-wide using the database collation: the same number in different commissions is allowed when full folios differ.

The request contains only `NumeroFolio`, not a trusted prefix/full folio. Selection records the reviewed prefix in the BLL; issuance rereads authoritative enrollment/course/commission data under the existing transaction locks. A changed prefix blocks issuance: **select the enrollment again**, review the refreshed prefix and reenter the number. Existing certificates, including legacy free-form folios, remain read-only and download with their exact original snapshot. This change requires **no schema migration or backfill**.

## Persistence and failure handling

`CertificadoFinalizacion` stores issue-time name, surname, course, average, signer/title, folio, date, mode, email, mention and observations, associated with the enrollment—not an arbitrary assessment. There is no retention table or PDF blob. Later downloads regenerate the same certificate data, independently of student/course edits; PDF binary bytes can differ between print jobs.

Issuance serializes certificate writers, locks enrollment/grade/debt/certificate reads, rechecks current permissions and eligibility, inserts the snapshot, updates DVH/DVV on the **same** SQL connection/transaction, then commits. Database uniqueness covers both enrollment and institution-wide folio. A trigger prevents business-field updates/deletes while allowing DVH maintenance. The stored average has 16 fractional digits; this representation never determines approval.

The separate **Evaluaciones** screen now permits historical grade/date/observation corrections **only before any certificate exists for that enrollment**. Issuance and correction acquire transaction-owned application locks in the same order (`CUN09:Certificado` → `CUN08:ResultadoEvaluacion`) before reading grades, so a committed certificate cannot represent an older grade snapshot followed by a successful correction. Existing snapshots are never rewritten. Deploy the matching DAL binaries together; see [Evaluaciones deployment and acceptance](../cun08/EVALUACIONES_MANUAL.md).

Audit runs after issuance commit. An audit failure is reported as **issued with audit warning**, not as rollback. Search again after an uncertain connection failure to discover any committed certificate before retrying. Retention performs no business writes and keeps eligibility locks during the audit attempt. Repeat clicks after successful retention in the same operation do not log twice.

`RevalidarEnTransaccion` names the shared DAL boundary explicitly: the BLL decides using freshly locked data; returning a snapshot requests issuance, while returning null makes no business writes. One per-enrollment retention state remembers the last reviewed reasons and whether logging already succeeded. The transaction owner preserves the original failure if rollback also fails, attaching secondary evidence as `Exception.Data["RollbackException"]`.

Enrollments with 0, 1 or more than 2 assessments are excluded and direct issue/retention calls are rejected. Existing >2 histories are retained without choosing/deleting grades; the message requests review. Rerunning CUN08 installs its cap trigger without scanning/rejecting legacy counts or rewriting DVH/DVV. Existing issued snapshots are never recalculated or changed because eligibility rules changed. New snapshots store `(assessment1 + assessment2) / 2` as informational mean.

## Compilación y comprobación manual

Desde el directorio de la solución:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Solucion.sln" /t:Build /p:Configuration=Debug /verbosity:minimal /nologo
```

Los ejecutores automatizados CUN08/CUN09/Evaluaciones y los conjuntos de prueba CUN08 fueron retirados por decisión del usuario. Seguir ahora [la guía manual](PRUEBAS_MANUALES.md), confirmando primero la conexión a una instalación aislada autorizada. La compilación no demuestra por sí sola el comportamiento SQL ni la aceptación visual.

Como evidencia histórica anterior a la retirada, las integraciones pasaron con bases LocalDB desechables y PDF real. Comprobaron reejecución de migración con tabla vacía y con certificados emitidos, conservación de cantidad/snapshot/DVH/DVV y recálculo de integridad viva; también el fallo secundario de rollback sin ocultar la excepción original. Los resultados detallados y sus límites están en la guía manual; no se presentan como pruebas ejecutables disponibles actualmente.

Manual desktop acceptance still covers the actual save dialog (cancel, overwrite, inaccessible output), bilingual interaction, and visual PDF inspection. A real printer smoke test does not replace those checks. There is no destructive rollback script: issued certificates are permanent records; restoring a backup is an administrator-controlled recovery operation.

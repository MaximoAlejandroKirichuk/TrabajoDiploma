# Consultar y corregir evaluaciones guardadas

La nueva pantalla **Cobros, Morosidad y Actas → Evaluaciones** permite consultar todos los resultados guardados de una comisión y corregir **únicamente calificación, fecha y observaciones**. La inscripción, el alumno, la comisión y la identidad del resultado no se editan. Si la inscripción tiene cualquier certificado emitido, **todas sus evaluaciones quedan solo para consulta**; no se modifica ni invalida el certificado.

## Preparación y despliegue

1. Trabajar primero sobre una instalación de prueba autorizada, con una copia de seguridad restaurable. Detener los escritores antes del despliegue. No ejecutar el esquema completo ni los conjuntos de prueba sobre una base existente.
2. Verificar que estén instaladas las dependencias de financiación y las migraciones actuales `cun08/001_migracion.sql` y `cun09/001_migracion.sql`. Aplicar después el nuevo script, reemplazando **ambos** parámetros:

   ```powershell
   sqlcmd -S "SERVIDOR" -d "BASE_AUTORIZADA" -E -b -f 65001 -i "Scripts/Sql/cun08/006_evaluaciones.sql"
   ```

3. Asignar mediante la administración existente de roles/familias:

   | Permiso | Capacidad |
   |---|---|
   | **73 — Ver evaluaciones** | Abrir, buscar y consultar; sin modificaciones. |
   | **74 — Modificar evaluaciones** | Corregir; requiere también el permiso 73 para consultar/seleccionar. |

   No se conceden permisos automáticamente, ni siquiera a administradores. Se admite asignación directa o por familias anidadas. No se exige ser profesor asignado ni pertenecer a un rol de nombre específico. Cerrar sesión e iniciar nuevamente para actualizar los permisos de la sesión; SQL también los revalida al guardar.

4. Compilar y desplegar el conjunto completo de binarios; no mezclar una DAL nueva con una versión anterior de emisión de certificados. El instalador de una base nueva incluye la cadena de dependencias y el nuevo script. Abrir la pantalla o validar una conexión **no migra una base existente**.

> **Conexión manual:** actualmente `UI/Aplicacion/ProveedorConfiguracionBD_83KI.cs` devuelve constantes para `(localdb)\ProjectModels`, base `TrabajoDiploma9/13`. Este cambio no las modifica. No iniciar la aplicación suponiendo que las pruebas automáticas la redirigen a su base temporal. Para aceptación interactiva se necesita una instalación cuya conexión de prueba esté autorizada y comprobada; no usar datos reales para simular fallos.

## Uso habitual

1. Abrir **Cobros, Morosidad y Actas → Evaluaciones**, seleccionar una comisión con resultados y pulsar **Buscar**. Se incluyen comisiones en curso y finalizadas; no se pide una fecha de examen que oculte resultados de otros días.
2. Verificar evaluación, inscripción, DNI y alumno, fecha, calificación, condición y observaciones en la grilla. Las dos evaluaciones aparecen como filas distintas, con sus respectivas fechas y valores persistidos.
3. Seleccionar una fila. En modo consulta se muestran los valores, pero no se habilita Guardar. Con ambos permisos y sin certificado, modificar solamente los tres campos admitidos.
4. Pulsar **Guardar modificación** y confirmar. La lista se vuelve a consultar y la condición mostrada se recalcula. Para continuar editando, seleccionar nuevamente una fila.
5. **Cancelar edición**, responder No a la confirmación, cambiar de búsqueda o cerrar descarta el editor; no hay guardado automático. Esta pantalla no agrega ni elimina evaluaciones, no exporta ni envía documentos.

## Reglas y mensajes

| Situación | Comportamiento esperado |
|---|---|
| Calificación | Decimal entre 1 y 10, con hasta dos decimales. `6,99` desaprueba; `7` aprueba. El separador decimal sigue la configuración regional de Windows. |
| Fecha | Solo fecha, entre inicio y fin de comisión, ambos inclusive, y no posterior a la fecha actual del servidor SQL. El reloj del cliente no decide. |
| Observaciones | Opcionales, máximo 500 caracteres; vacías se guardan como cadena vacía. |
| Fecha histórica fuera de rango | El valor original permanece en la grilla. El selector propone un valor válido y lo advierte; nada se guarda hasta confirmar. Un rango imposible deshabilita la edición sin provocar un error del selector. |
| Fecha repetida | No puede coincidir con la otra evaluación de la misma inscripción. La operación se rechaza sin cambios parciales. |
| Inscripción con certificado | Solo consulta, aunque el certificado se haya emitido después de seleccionar la evaluación. No se reescribe la instantánea ni su promedio. |
| Historia con más de dos notas | Solo consulta con mensaje de revisión administrativa. No se eligen, eliminan ni normalizan notas automáticamente. |
| Datos modificados por otra sesión | Mensaje de conflicto; repetir Buscar y revisar antes de guardar. No se sobreescribe la versión nueva. |
| Permiso revocado después de abrir | La BLL comprueba la sesión y la DAL vuelve a consultar permisos efectivos en la transacción. No se guarda con permisos obsoletos. |
| Sin cambios | No se actualizan datos, DVH/DVV ni se genera un evento de modificación. |
| Fallo de integridad antes del commit | Resultado e integridad se revierten juntos. Se conserva el error original aunque también falle el rollback. |
| Fallo de bitácora después del commit | Se informa que los datos ya están guardados y se muestra la advertencia. No repetir el guardado. |

La corrección histórica no vuelve a exigir el estado financiero usado para ingresar un examen nuevo. La aprobación por evaluación es `Calificacion >= 7`; CUN09 sigue exigiendo **exactamente dos notas y ambas >= 7**, con promedio informativo. No se agregan componentes teóricos/prácticos.

## Recorrido del código para el DSS

| Paso | Método real y responsabilidad |
|---|---|
| Menú | `FrmPrincipal.evaluacionesToolStripMenuItem_Click` comprueba 73 y construye la pantalla mediante `ServiceFactory_83KI.CrearEvaluacionesBLL()`. |
| Comisiones | `KI68EvaluacionesBLL.ListarComisionesConResultados()` → `ResultadoEvaluacionDAL_83KI.ListarComisionesConResultados()`; autorización y comisiones con resultados, sin filtro por docente o deuda. |
| Buscar | `FrmEvaluaciones_83KI.Buscar()` → `KI68EvaluacionesBLL.Buscar(int)` → `BuscarEvaluaciones(int)`; devuelve resultados persistidos de todas las fechas de esa comisión. Buscar descarta la selección anterior. |
| Seleccionar | `Seleccionar()` → `KI68EvaluacionesBLL.Seleccionar(int)` → `ObtenerEvaluacion(int)`; la BLL conserva privadamente una instantánea original inmutable obtenida del servidor, no un original editable enviado por la UI. |
| Confirmar | `FrmEvaluaciones_83KI.Guardar()` muestra confirmación y envía `ModificarEvaluacionRequest_83KI`, que contiene solo nota, fecha y observaciones, sin identidades editables. |
| Validar | `KI68EvaluacionesBLL.Modificar(...)` comprueba sesión/permisos, nota, longitud y rango de comisión. Captura actor y valores nuevos; delega sin registrar todavía bitácora. |
| Transacción | `ResultadoEvaluacionDAL_83KI.ModificarEvaluacion(original, cambios)` usa `AccesoDAL_83KI.EjecutarTransaccion`. Adquiere primero `CUN09:Certificado` y después `CUN08:ResultadoEvaluacion`; `AutorizarConsulta` lee usuario/rol/grafo vigente; `LeerEvaluacion` vuelve a leer fila, contexto, fecha SQL, cantidad y existencia de certificado con bloqueos. |
| Conflictos | Compara identidad de resultado/inscripción/comisión, alumno visible, valores originales, condición y DVH. Rechaza fila ausente, conflicto, certificado, historia >2, fechas inválidas/futuras o duplicadas. No-op retorna sin UPDATE. |
| Actualizar | Un UPDATE cambia únicamente los tres campos autorizados y `CondicionFinal` derivada mediante `ResultadoEvaluacion_83KI.DeterminarCondicionFinal()`. Nunca escribe en `CertificadoFinalizacion`, inscripción, alumno ni comisión. |
| Integridad/commit | `RecomputarIntegridadResultados` reutiliza `ActualizarDVHyDVVResultados(ids, conn, tran)` en la misma conexión y transacción; el helper confirma o revierte toda la operación. |
| Bitácora | Después del retorno confirmado de DAL, BLL registra evaluación/inscripción/comisión y nota/fecha antes→después. Para observaciones registra longitud y SHA256 antes→después, no el texto personal, siguiendo la minimización usada por CUN08. Actor/fecha usan `BitacoraEvento_83KI.CrearNuevo`. Si falla, devuelve `AuditoriaFallida=true`, sin fingir rollback. |
| Refrescar/cancelar | UI informa resultado real y vuelve a buscar. `KI68EvaluacionesBLL.Cancelar()` solo descarta la instantánea; si falla la recarga posterior se informa por separado, sin sugerir repetir una escritura confirmada. |

**Emisión concurrente:** `CertificadoDAL_83KI.RevalidarEnTransaccion` ahora adquiere esos mismos bloqueos en el mismo orden, antes de leer las notas. Si la corrección confirma primero, el certificado lee las nuevas notas; si la emisión confirma primero, la corrección detecta el certificado y se rechaza. Los bloqueos son transaccionales y globales porque los DVV abarcan tablas completas. Esto prioriza integridad sobre concurrencia; las búsquedas de evaluaciones también usan ese orden para evitar lecturas mezcladas. SQL privilegiado que omita la DAL no está dentro de este modelo de autorización.

## Evidencia histórica anterior a la retirada de pruebas

Los ejecutores automatizados CUN08/CUN09/Evaluaciones y los conjuntos CUN08 fueron eliminados por decisión del usuario. Las ejecuciones siguientes ocurrieron **antes de esa retirada**; no son instrucciones ejecutables actuales. El arnés utilizó bases LocalDB desechables, comprobó su eliminación y no guardó configuración de conexión. La aceptación actual se realiza con la lista interactiva de esta guía.

El 25/10/2026 el build Debug normal alcanzó la copia de dependencias de UI, pero falló con `MSB3027/MSB3021` por DLL/PDB bloqueados por la aplicación, Visual Studio y el depurador. No se detuvo ningún proceso. Se verificó el directorio padre temporal y se compiló el código actual completo con salida aislada:

```powershell
Test-Path -LiteralPath "C:\Users\User\AppData\Local\Temp\opencode"
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Solucion.sln" /t:Rebuild /p:Configuration=Debug /p:OutDir=C:/Users/User/AppData/Local/Temp/opencode/Evaluaciones-20261025-build/ /verbosity:minimal /nologo
```

Las pruebas de la nueva pantalla incluyen fakes BLL, controles WinForms reales y DAL con SQL real: dos fechas, consulta/edición sin docente ni deuda habilitante, permisos revocados, validaciones, no-op/cancelación, condición derivada, conflicto de instantánea, fecha repetida, rollback completo de resultado/DVH/DVV por fallo inyectado **después** del mantenimiento de integridad, advertencia de auditoría posterior al commit y dos órdenes controlados de carrera con emisión. Las carreras usan conexiones reales, tareas y puntos de espera dentro de las transacciones; no simulan el resultado con un fake. Se comprueba que no haya tercera evaluación y que la historia >2 se conserve. Se ejecutan además las regresiones CUN08 y CUN09, incluida exportación PDF real.

Resultados del último recorrido sobre los binarios aislados:

| Verificación ejecutada | Resultado |
|---|---|
| Rebuild de los cinco proyectos C# | Correcto, sin advertencias en esa ejecución. |
| `Evaluaciones.Tests.ps1 -Integration` | 34 aserciones BLL/UI y 102 acumuladas tras SQL; traducciones completas, recursos, familias anidadas, rechazo de colisiones de permisos y migración idempotente antes/después de emisión. |
| `Cun08.Tests.ps1 -Integration` | Correcto, 166 aserciones acumuladas; conserva el contrato y la ubicación RF2 de la pantalla de ingreso. |
| `Cun09.Tests.ps1 -Integration -Pdf` | Correcto, 107 aserciones acumuladas; impresión PDF real y pruebas de emisión/integridad. |
| `Cun08.DatasetCanonicalization.Tests.ps1 -BuildPath <salida aislada>` | Correcto, 40 filas en ocho combinaciones de tipos/compatibilidad SQL. |
| `Cun08.DanielDataset.Tests.ps1 -BuildPath <salida aislada>` | Correcto, conjunto protegido, idempotencia, selectores y detección de alteraciones. |
| `FinanciacionCuotas.Tests.ps1 -Server "(localdb)\MSSQLLocalDB" -BuildPath <salida aislada>` | Correcto, instalador SQLCMD real, reglas de pagos y consistencia SQL/.NET. |
| Limpieza | Cada runner eliminó su base temporal. Una consulta posterior de `sys.databases` en la instancia de pruebas no encontró bases `Cun08Tests_*`, `Cun08Canonical_*` ni `FinanceTests_*`. |

La revisión final de código y las regresiones se realizaron después del incidente de archivos bloqueados, sin agentes secundarios ni cambios al entorno global. No constituyen una revisión independiente ni aceptación visual.

**Pendiente de aceptación manual:** presentación visual y navegación real en español/inglés, confirmaciones Sí/No, cancelación de campos mediante ratón/teclado, varias sesiones de la aplicación, despliegue MSI y prueba del instalador en una máquina limpia. El build de `Solucion.sln` no genera el MSI: el proyecto `.vdproj` no está marcado Build en esa configuración. La revisión de recursos empaquetados no sustituye esa prueba. El directorio Debug habitual requiere un nuevo build cuando el usuario libere sus bloqueos. No se realizó migración ni modificación de la base del usuario durante estas verificaciones.

## Lista de aceptación interactiva

- [ ] Con solo 73, abrir/buscar y ver ambas evaluaciones; editor bloqueado. Con solo 74 o sin permisos, no abrir.
- [ ] Con 73+74, modificar los tres campos, confirmar y volver a seleccionar: condición y valores actualizados.
- [ ] Cancelar o responder No no cambia filas ni bitácora; guardar exactamente lo mismo no genera evento.
- [ ] Seleccionar antes de que otra sesión corrija; guardar después muestra conflicto y conserva la primera corrección.
- [ ] Emitir un certificado para la inscripción; comprobar que ambas evaluaciones siguen visibles pero quedan bloqueadas.
- [ ] Revisar el caso de fecha histórica fuera del rango sin aceptar automáticamente la propuesta del selector.
- [ ] Comprobar bitácora con identidades y evidencia antes/después, sin texto personal de observaciones.
- [ ] Probar idioma español/inglés y desplegar el MSI en entorno aislado antes de usarlo con datos operativos.

## Archivos de esta funcionalidad

La tabla enumera los archivos funcionales y las guías conservados de este trabajo; los ejecutores de pruebas fueron retirados. No se revirtió ni se incluyó en un commit la reubicación previa de DTOs u otro trabajo CUN08/CUN09.

| Estado en este trabajo | Ruta exacta |
|---|---|
| Nuevo | `BE/DTOs/Evaluaciones/EvaluacionConsulta_83KI.cs` |
| Modificado | `BE/BE.csproj` |
| Nuevo | `BLL/Negocio/KI68EvaluacionesBLL.cs` |
| Modificado | `BLL/BLL.csproj` |
| Modificado | `BLL/ServiceFactory_83KI.cs` |
| Nuevo | `DAL/ResultadoEvaluacionConsultaDAL_83KI.cs` |
| Modificado | `DAL/ResultadoEvaluacionDAL_83KI.cs` |
| Modificado; archivo ya sin seguimiento | `DAL/CertificadoDAL_83KI.cs` |
| Modificado | `DAL/BootstrapBaseDatosDAL_83KI.cs` |
| Modificado | `DAL/DAL.csproj` |
| Nuevo | `Service/Interfaces/IEvaluacionesDAL_83KI.cs` |
| Modificado | `Service/Service.csproj` |
| Modificado | `Service/Entidades/Enums.cs` |
| Modificado | `Service/Idiomas/es-AR.json` |
| Modificado | `Service/Idiomas/en-US.json` |
| Nuevo | `UI/Modulos/PlanificacionAcademica/FrmEvaluaciones_83KI.cs` |
| Modificado | `UI/UI.csproj` |
| Modificado | `UI/Principal/FrmPrincipal.cs` |
| Modificado | `UI/Principal/FrmPrincipal.Designer.cs` |
| Modificado | `Solucion_83KI/Solucion_83KI.vdproj` |
| Nuevo | `Scripts/Sql/cun08/006_evaluaciones.sql` |
| Nuevo | `Scripts/Sql/cun08/EVALUACIONES_MANUAL.md` |
| Modificado; archivo ya sin seguimiento | `Scripts/Sql/cun08/README.md` |
| Modificado; archivo ya sin seguimiento | `Scripts/Sql/cun09/README.md` |

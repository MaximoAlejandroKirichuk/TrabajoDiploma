# CUN09: aceptación manual y explicación del DSS

**Objetivo:** comprobar que se emite un único certificado por inscripción aprobada y totalmente pagada, o se registra la retención con los motivos vigentes. **Generar** persiste; **Guardar certificado** exporta un PDF del contenido original. Ninguna acción envía correo ni imprime en papel.

## 1. Preparación segura

1. Utilizar una **base aislada de aceptación**, con respaldo y datos sintéticos. No ejecutar preparación, carreras ni fallos inducidos sobre la base de trabajo real.
2. Confirmar la conexión efectiva antes de abrir la aplicación. En esta versión, `UI/Aplicacion/ProveedorConfiguracionBD_83KI.cs` devuelve valores fijos: `(localdb)\ProjectModels`, base `TrabajoDiploma9/13`. **No suponer que ejecutar las pruebas automáticas cambia esa conexión.** Si esa base contiene trabajo real, detener la preparación manual hasta disponer de una instalación de prueba cuya conexión aislada esté autorizada. Esta guía no solicita modificar ese proveedor ni la configuración global.
3. La base de aceptación debe tener el esquema, el instalador de financiación y las migraciones CUN08 y CUN09; consultar [instalación](README.md#installation). Una base existente **no se migra al validar conexión ni al abrir la pantalla**. La instalación debe realizarla el responsable de esa base; aquí no se incluyen comandos destructivos.
4. Crear un coordinador activo, no bloqueado, y asignarle mediante la administración de roles/familias el permiso **72 — Generar certificado de finalización**. Cerrar e iniciar sesión. Preparar otro usuario sin ese permiso.
5. Para descargar PDF: Windows con **Microsoft Print to PDF** habilitado, una carpeta de salida de prueba y un lector PDF. La emisión no depende de la impresora.

Los ejecutores automatizados CUN08/CUN09/Evaluaciones y los conjuntos CUN08 fueron retirados por decisión del usuario. Sus antiguas bases temporales se eliminaron; no hay una base persistente ni credenciales de demostración suministradas por esas pruebas. Preparar la aceptación con los flujos existentes sobre una instalación aislada autorizada.

### Datos que deben existir antes de cada caso

Registrar los identificadores reales de la base de aceptación; **no asumir IDs, DNI, folios ni datos de ejemplo preinstalados**. Preparar mediante los flujos existentes o un fixture aprobado que mantenga integridad:

| Grupo | Preparación verificable |
|---|---|
| Comisiones | Una finalizada ayer, una que finaliza hoy —incluida hora posterior a la actual si el campo lo permite— y otra mañana. Para el caso sin comisiones, utilizar otra base aislada sin comisiones finalizadas; no borrar las existentes. |
| Inscripciones | Una sin evaluaciones, una con una y varias con exactamente dos de fechas distintas; un mismo alumno en dos inscripciones/cursos para detectar mezclas. Registrar `IdSolicitudInscripcion`, no solamente DNI. Un histórico con tres se ensaya únicamente en el entorno técnico aislado. |
| Notas y promedio | `6/8` (promedio 7), `10/6` (promedio 8) y `6,99/7,01` desaprueban; `7/7` y `7/10` aprueban. El promedio es informativo: se requiere **cada nota >=7**. Todos los cursos tienen exactamente dos evaluaciones, sin cantidad configurable. |
| Deuda | Aprobado pagado; aprobado con una cuota futura pendiente; desaprobado pagado; desaprobado con deuda. Incluir varias cuotas pendientes con vencimientos anteriores y posteriores a hoy. |
| Certificados | Al menos dos inscripciones aprobadas/pagadas sin certificado para emisión y folio duplicado; después conservar una emitida para pruebas de consulta y descarga. No eliminar certificados para reiniciar casos. |
| Observaciones | Una evaluación con observación del profesor y otra sin observación; datos con tildes para inspeccionar PDF. |

No crear más de dos evaluaciones para probar promedios ni cambiar notas definitivas. Los históricos con más de dos se conservan, no aparecen en CUN09 y una invocación directa informa la inconsistencia sin generar ni retener. Las migraciones no los eliminan ni seleccionan dos arbitrariamente.

## 2. Apertura, búsqueda y selección

Abrir **Planificación Académica → Generar certificado de finalización**.

| Caso | Acción | Resultado y evidencia esperados |
|---|---|---|
| A1 · Sin comisiones | Abrir en la base sin comisiones finalizadas. | Mensaje informativo y cierre del formulario. Ningún certificado ni evento CUN09 nuevo. |
| A2 · Fecha inclusiva | Abrir con las comisiones de ayer, hoy y mañana. | Se listan ayer y hoy; mañana no. No se requiere un cierre manual adicional ni otro estado académico. La comparación usa la fecha del servidor. |
| A3 · Acciones independientes | Abrir y no pulsar Buscar; luego elegir comisión y pulsar **Buscar**. | Antes de Buscar no hay alumnos cargados. Después aparecen solo inscripciones con **EXACTAMENTE 2** evaluaciones persistidas. Cambiar comisión limpia selección y acciones; hace falta Buscar otra vez. |
| A4 · Sin resultados | Buscar una comisión sin inscripciones con exactamente dos evaluaciones (por ejemplo, todas con cero o una). | Mensaje sin alumnos; no habilita emisión, retención ni guardado. Cubre el listado elegible vacío, sin un flujo alternativo nuevo. |
| A5 · Notas y alcance | Buscar `6/8`, `10/6`, `6,99/7,01`, `7/7` y el mismo alumno en otro curso. | Los tres primeros desaprueban pese al promedio >=7; `7/7` aprueba. Se muestran DNI, nombre/apellido, Evaluación 1, Evaluación 2, promedio y condición de **esa inscripción y comisión**. |
| A6 · Orden y cantidad | Registrar primero la evaluación de fecha posterior; completar otra de fecha anterior. Comparar con inscripciones con 0, 1 o más de 2 notas históricas. | Evaluación 1/2 se ordenan por fecha ascendente, no por orden de carga ni valor. Solo exactamente dos aparece; invocación directa incompleta/inconsistente rechazada. |

## 3. Emisión y validaciones

En cada prueba fallida, comparar antes/después: **sin nueva fila** en `CertificadoFinalizacion`, sin folio asignado y sin evento de emisión. Buscar nuevamente si hubo un fallo de conexión con resultado incierto.

| Caso | Acción | Resultado y persistencia esperados |
|---|---|---|
| E1 · Camino principal | Seleccionar aprobado con saldo total cero y sin certificado. Revisar firmante/cargo y **Prefijo del folio** de solo lectura. Ingresar únicamente `12` en **Número de folio**, email válido y modalidad; opcionales vacíos. Pulsar **Generar certificado**. | Prefill de nombre, apellido, curso, promedio y fecha del servidor. El folio usa código exacto de comisión, curso normalizado, **FechaFin** y sufijo `0012`: por ejemplo `C08D-HOY-TAROT-INICIAL-20261025-0012`. Se crea exactamente una fila con DVH y se actualiza DVV. Luego se registra emisión. Mensaje menciona email registrado, **no enviado**. Guardar queda habilitado. |
| E2 · Obligatoriedad | Dejar el número vacío; escribir o pegar espacios, `0`, `0000`, `-1`, `+1`, `1.2`, `1e3`, letras o un folio completo. Probar email vacío, `sin-arroba` y `Nombre <persona@ejemplo.invalid>`. | Rechazo de validación, también en BLL. No hay número predeterminado ni reserva automática. Modalidad solo permite las dos opciones del combo. Ninguna persistencia parcial. |
| E3 · Límites | Probar números `1`, `12`, `10000`, `9223372036854775807` y desbordamiento `9223372036854775808`; mención de 200, observaciones de 500 y email válido hasta 254. Preparar nombres de curso que hagan el folio completo de 200 y 201 caracteres. | Sufijos `0001`, `0012`, `10000` y máximo Int64 completo, sin recorte. Desbordamiento y folio de 201 rechazados con orientación para corregir; 200 admitido. No se truncan nombres para evitar colisiones. La BLL rechaza mención de 201 y observaciones de 501. |
| E4 · Modalidades | Emitir dos certificados de inscripciones distintas, uno **Solo digital** y otro **Digital e impresa**. | Se conserva la modalidad seleccionada. No se abre impresión en papel ni se envía email; ambos permiten guardar PDF. |
| E5 · Folio global | En otra inscripción de la misma comisión, ingresar `0012` o `00012` después de E1. Luego probar `12` en una comisión distinta. | Los ceros iniciales se canonicalizan: rechazo del folio completo duplicado en la misma comisión. En otra comisión se permite el mismo número si cambia el folio completo. Unicidad institucional según la intercalación y restricción SQL existentes; original sin cambios. |
| E6 · Certificado existente | Cerrar, volver a abrir, Buscar y seleccionar E1. | Datos originales de solo lectura, fecha/folio/email originales y **Guardar certificado** habilitado; no habilita nueva emisión ni folio. No agrega fila ni nuevo evento de emisión. |
| E7 · Normalización y fecha | Usar curso `  Tárót -- inicial / : ` y código `C08D-HOY`; verificar fecha de fin distinta de la emisión. | Curso `TAROT-INICIAL`, código preservado y fecha **de finalización** en `yyyyMMdd`. Se eliminan marcas diacríticas; mayúsculas invariantes, letras ASCII A–Z y dígitos; toda secuencia restante se convierte en un guion interno, sin guiones de borde. Nombre normalizado vacío: solicitar revisión, sin emitir. |
| E8 · Prefijo desactualizado | Tras seleccionar, cambiar código, nombre del curso o fecha de fin mediante un flujo autorizado en el entorno aislado; intentar emitir. | Rechazo sin escrituras. Volver a seleccionar explícitamente para revisar el prefijo actualizado y volver a ingresar el número. No se emite silenciosamente con otro folio. |
| E9 · Folio histórico | Seleccionar un certificado anterior con folio libre, por ejemplo `CUN09-PRUEBA-...`, y guardar PDF. | Folio original exacto, solo lectura, sin exigir formato nuevo ni migrarlo. Snapshot y exportación no cambian. |

No se requiere una nueva migración para esta regla: se conserva `Folio` completo en la columna existente. No hay columna de sufijo, contador ni cambios de envío/email/modalidad.

## 4. Retención y cambios entre selección y confirmación

Una retención exitosa agrega **solo un evento de bitácora**, con DNI, identidad de inscripción, motivos calculados y observaciones opcionales; confirma y cierra. No crea certificado ni una entidad/tabla de retención. El usuario no edita motivos.

| Caso | Acción | Resultado esperado |
|---|---|---|
| R1 · Arancelario | Seleccionar aprobado con deuda, incluida una cuota que aún no venció. | No aparecen campos de emisión. Se muestran todas las cuotas con saldo positivo: número, saldo y vencimiento; saldo total incluye las futuras. Retener registra **Arancelario**. |
| R2 · Académico | Seleccionar desaprobado totalmente pagado y Retener. | Se muestran observaciones del profesor si existen; motivo **Académico**. No inventar observaciones cuando no existen. |
| R3 · Ambos | Seleccionar desaprobado con deuda y Retener. | Motivo **Académico + Arancelario**, detalle de cuotas pendientes y observaciones disponibles. |
| R4 · Observaciones | Retener otro caso con 500 caracteres; intentar pegar 501. | La UI no admite exceder 500; los 500 se conservan en el evento. La BLL también rechaza excedidos. Observaciones vacías son válidas. |
| R5 · Deuda pagada después de seleccionar | En sesión A seleccionar aprobado con deuda; mediante el flujo de pago en sesión B cancelar **todo** el saldo, incluida deuda futura. En A pulsar Retener. | Se reconsulta; no agrega evento de retención. Informa cambio y muestra datos para generación. Completar emisión solo si aún no existe certificado. |
| R6 · Cambian los motivos | En A seleccionar desaprobado con deuda; en B cancelar todo el saldo. En A pulsar Retener. | Primer clic: refresca a **Académico**, sin evento. Segundo clic: recién entonces registra Académico. Las observaciones se limpian al refrescar: revisarlas/reingresarlas antes del segundo clic. |
| R7 · Protección de notas | Con exactamente dos notas confirmadas, intentar una tercera en otra fecha mediante CUN08. | La inscripción no está habilitada. No se pueden agregar notas para cambiar aprobación ni editar las definitivas. El ensayo técnico también verifica rechazo al generar/retener con cantidad incompleta o histórica inconsistente. |
| R8 · Repetición accidental | Tras retención exitosa, intentar repetir el mismo envío en el ensayo técnico. | No duplica el evento en esa operación; también se mantiene la protección si se vuelve a seleccionar la inscripción con la misma instancia de BLL. No es una prohibición permanente de futuras retenciones en nuevas operaciones. |

## 5. Descarga y contenido original

| Caso | Acción | Resultado esperado |
|---|---|---|
| P1 · PDF correcto | Sobre E1, pulsar **Guardar certificado**, elegir carpeta escribible y guardar. Abrir archivo. | Diálogo solo PDF, nombre alumno+curso saneado; contenido legible, sin recortes, con nombre/apellido, curso, promedio, fecha, firmante/cargo, folio, modalidad, email, mención y observaciones originales. No cambia certificado, DVH/DVV ni email. |
| P2 · Cancelar | Abrir Guardar y cancelar. | Mensaje de cancelación, Guardar sigue habilitado; no cambia datos ni registra otra emisión. Reintentar con éxito. |
| P3 · Fallo y reintento | En carpeta de prueba, mantener el PDF de destino abierto con bloqueo exclusivo o utilizar destino sin permiso de escritura. Guardar; luego quitar el bloqueo y reintentar en ruta válida. | El diálogo o la aplicación informa el problema; no presenta un archivo fallido como guardado. Guardar permanece habilitado y el reintento funciona. No cambiar permisos del sistema ni deshabilitar impresoras globalmente para esta prueba. |
| P4 · Sobrescritura | Elegir un PDF de prueba existente y comprobar las decisiones del diálogo. | Al rechazar sobrescritura se puede cancelar/reintentar; al aceptar, el archivo se reemplaza por el certificado seleccionado, sin nueva emisión. |
| P5 · Snapshot | Después de emitir, cambiar nombre/apellido del alumno o nombre del curso mediante los flujos autorizados de la base aislada. Reabrir y guardar el certificado. | Permanece el contenido de emisión original en todos los campos. Los bytes del PDF pueden diferir entre trabajos de impresora: comparar contenido, no hash binario del archivo. |

## 6. Seguridad y comprobaciones de persistencia

- **Usuario sin permiso 72:** el menú no está disponible; una invocación directa de la BLL tampoco debe autorizarse. Usuario inactivo/bloqueado, sin sesión o en recuperación de integridad: rechazo. La revocación de permiso después de seleccionar también se revalida en la transacción (ensayo técnico).
- **Idioma:** repetir apertura, selección, mensajes y botones en español/inglés. El PDF representa el documento institucional en español; sus datos de emisión no se traducen ni sustituyen por datos maestros actuales.
- **Solo lectura para evidencia SQL:** el responsable de aceptación debe filtrar por los IDs/fechas del caso. No hace falta modificar tablas para verificar resultados:

```sql
-- Ejecutar solo en la base aislada ya confirmada y sustituir por el ID real del caso.
DECLARE @IdInscripcion int = 0;
SELECT * FROM dbo.CertificadoFinalizacion WHERE IdInscripcion=@IdInscripcion;
SELECT IdInscripcion, COUNT(*) Cantidad FROM dbo.CertificadoFinalizacion
WHERE IdInscripcion=@IdInscripcion GROUP BY IdInscripcion;
SELECT NombreTabla,DVV,FechaActualizacion FROM dbo.DigitoVerificador_83KI
WHERE NombreTabla=N'CertificadoFinalizacion';
SELECT Fecha,Descripcion,Username FROM dbo.BitacoraEventos
WHERE Descripcion LIKE N'CUN09:%' ORDER BY Fecha DESC;
```

Filtrar la bitácora por inscripción y momento del ensayo; el DNI puede aparecer en más de una inscripción. Una consulta de DVV almacenado **no demuestra** integridad por sí sola: las pruebas técnicas también recalculan DVH/DVV desde datos vivos.

## 7. Ensayos técnicos separados de la aceptación básica

No inducir estos fallos en una base real. La tabla conserva los criterios y el alcance de los ensayos técnicos históricos; sus ejecutores ya no están disponibles. Una nueva comprobación requiere un mecanismo de ensayo autorizado en un entorno aislado; no improvisar cambios sobre datos operativos.

| Ensayo | Evidencia exigida |
|---|---|
| Concurrencia 8.3 | Dos sesiones seleccionan la misma inscripción elegible sin certificado; A genera folio A y luego B intenta folio B con datos válidos. B recibe el certificado original A, con fecha/folio/email originales y Guardar habilitado; folio B no queda asignado. La carrera simultánea automatizada debe producir una emisión y un resultado existente, no dos filas. |
| Auditoría posterior al commit | Forzar fallo del auditor de emisión en el doble de pruebas: certificado persistido, `AuditoriaFallida=true` y descarga disponible. No afirmar rollback ni invitar a otra emisión. Esto prueba el tratamiento BLL; no es un corte real de infraestructura de bitácora. |
| Fallo de integridad antes del commit | Inyectar fallo después de actualizar DVH/DVV dentro de la transacción: no queda certificado ni cambio en DVV, ni evento de emisión. Se usa DAL real en base temporal. |
| Fallo secundario de rollback | Una operación de prueba revierte su transacción y lanza un error; el intento posterior de rollback también falla. Debe propagarse **la misma excepción original**, con su pila, y guardar la secundaria en `Data["RollbackException"]`; nunca devolver éxito. |
| Reejecución de migración | Comprobar tabla vacía explícitamente y reejecutar. Después emitir al menos un certificado, capturar cantidad/snapshot completo/DVH/DVV, reejecutar y compararlos. Recalcular integridad viva antes/después; todo debe permanecer coherente. |
| Restricciones e inmutabilidad | Unicidad de inscripción y folio; intento de cambio de datos de negocio o borrado rechazado. La actualización transaccional de DVH sigue permitida. No probar borrados sobre certificados reales. |

### Compilación actual y evidencia histórica

Ejecutar desde la raíz de la solución. Requisitos de compilación: VS2022/MSBuild y el paquete de desarrollo .NET Framework del proyecto. Para aceptación manual de PDF se necesita Microsoft Print to PDF. No instalar herramientas ni cambiar la conexión como solución improvisada ante un faltante: registrar el requisito no disponible.

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Solucion.sln" /t:Build /p:Configuration=Debug /verbosity:minimal /nologo
git diff --check
```

Si la UI o el depurador bloquean `UI/bin/Debug`, no finalizar procesos para forzar la compilación. Verificar primero la carpeta temporal autorizada y elegir una salida nueva:

```powershell
Test-Path -LiteralPath "C:\Users\User\AppData\Local\Temp\opencode"
# Continuar solo si devuelve True; usar una carpeta de salida aún no utilizada.
$build = "C:\Users\User\AppData\Local\Temp\opencode\cun09-folio-auditoria-nueva\"
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Solucion.sln" /t:Rebuild /p:Configuration=Debug "/p:OutDir=$build" /verbosity:minimal /nologo
```

Auditoría del cambio de folio (25/10/2026): `Solucion.sln /t:Rebuild` correcto en `C:\Users\User\AppData\Local\Temp\opencode\cun09-folio-20261025-final`. CUN09: 100 comprobaciones BLL/UI, 150 acumuladas al informar la integración SQL y PDF real correcto; Evaluaciones: 120 acumuladas; CUN08: 166 acumuladas. También correctos `Cun08.DatasetCanonicalization.Tests.ps1` (40 filas) y `Cun08.DanielDataset.Tests.ps1`, todos con esa salida explícita. Bases LocalDB temporales eliminadas. La compilación de salida normal encontró archivos bloqueados por UI/VS; no se cerraron procesos ni se modificó la base real. Para usar la nueva versión en la aplicación habitual, detener voluntariamente la ejecución/depuración, recompilar y volver a abrir. Falta la aceptación visual de escritorio indicada en esta guía.

Conservar salida, fecha y resultado de cada comprobación nueva. La auditoría del 25/10/2026 descrita arriba ocurrió **antes de retirar las pruebas**: no implica que sus ejecutores sigan en el repositorio ni que se hayan vuelto a ejecutar después. Las pruebas BLL/estado UI no equivalen a verificar SQL; el ensayo PDF histórico comprobó generación real y cabecera, pero no reemplaza inspección visual, cancelación ni errores del diálogo.

## 8. Mapa breve: código → DSS académico

En el DSS usar **Coordinador**, `FrmGenerarCertificado_83KI`, `KI68CertificadoBLL`, servicios de integridad/bitácora y **una sola abstracción `KI68BaseDeDatos`**. Sin estereotipos ni líneas de vida DAL/BE. `KI68BaseDeDatos` agrupa académicamente consultas/persistencia: **no es una clase nueva de producción**. Las filas/DTOs y el snapshot son datos intercambiados, no participantes adicionales.

| Mensaje/responsabilidad del DSS | Código real y orden |
|---|---|
| Iniciar pantalla / listar finalizadas | `FrmGenerarCertificado_83KI.OnShown` → `KI68CertificadoBLL.ListarFinalizadas` → `CertificadoDAL_83KI.ListarFinalizadas`. Solo lista comisiones; si no hay, informa y termina. |
| Buscar alumnos de comisión | Acción independiente `Buscar` → BLL `Buscar` → DAL `Buscar`. Consulta inscripciones con exactamente dos evaluaciones, ordenadas por fecha; cada una debe ser >=7 para aprobar. Promedio informativo, sin mezclar inscripciones. |
| Seleccionar inscripción | UI `Seleccionar` → BLL `Seleccionar` → DAL `RevalidarEnTransaccion`. Obtiene situación actual, todas las cuotas con saldo, observaciones y certificado existente; BLL conserva motivos vistos. UI `Mostrar` elige emisión, retención o certificado original. |
| Generar | UI `Generar` → BLL `Generar`: primero `Autorizar` y `Validar` los datos de entrada; luego `RevalidarEnTransaccion`. Dentro de esta transacción se bloquea/reconsulta la situación y la BLL decide con esos datos; solo entonces se permiten escrituras. |
| Persistir e integridad | La DAL inserta snapshot; invoca el enlace configurado a `IntegridadDAL_83KI.ActualizarCertificado`, con la misma conexión/transacción; `AccesoDAL_83KI.EjecutarTransaccion` realiza commit. En el DSS representar estas responsabilidades como persistencia e integridad dentro de la unidad transaccional de `KI68BaseDeDatos`, sin inventar una llamada pública a BeginTransaction en la BLL. |
| Registrar emisión / informar | **Después del commit**, BLL `Registrar` → `IBitacoraManager_83KI.RegistrarEvento`; luego UI informa éxito o advertencia de auditoría. Si ya existía certificado, se devuelve el original sin insertar ni reasignar folio. |
| Retener | UI `Retener` → BLL `Retener` → `RevalidarEnTransaccion`. BLL compara motivos actuales con los vistos. Si cambiaron: devuelve actualización sin evento. Si se confirman: registra bitácora mientras siguen los bloqueos de elegibilidad; no inserta certificado ni retención. |
| Guardar PDF | Acción posterior independiente UI `Guardar` → `SaveFileDialog` → `CertificadoPdfExporter_83KI.Exportar`, utilizando `Contenido` del snapshot ya emitido. Sin nueva transacción de negocio, folio ni envío de correo. |

**Por qué se conserva el callback:** mantiene la decisión en la BLL mientras la DAL conserva la transacción y sus bloqueos. `RevalidarEnTransaccion` reemplaza el nombre inespecífico `Procesar`; devolver snapshot solicita emisión, devolver `null` no escribe negocio. Separarlo en «consultar» y «guardar» sin mantener esa unidad reabriría la carrera. El DSS expresa las responsabilidades y su orden, no necesita dibujar `Func` ni la clase privada del estado de retención.

**Estado de retención:** una entrada por inscripción recuerda `MotivosVistos` y `Registrada`. Así se explica una sola transición: revisar situación → reconfirmar si cambió → registrar una vez. No agrega tablas ni un nuevo estado académico del alumno.

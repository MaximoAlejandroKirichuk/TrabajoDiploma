# Matrícula separada y financiación del curso

La inscripción genera una matrícula de pago único y todas las cuotas del curso. El plan determina la cantidad de cuotas y aplica su recargo una sola vez al precio total del curso.

## Aplicar en la base de la aplicación

1. Detener la aplicación y abrir una terminal en la raíz de `Solucion`.
2. Ejecutar el instalador contra la instancia y base correspondientes:

   ```powershell
   sqlcmd -S "INSTANCIA" -d "BASE" -E -b -i "Scripts/Sql/financiacion-cuotas/aplicar.sql"
   ```

3. Recompilar y abrir la aplicación. En **Pago de inscripción** se cobra únicamente la matrícula; en **Pago de cuota** aparecen todas las cuotas del curso, pagadas y pendientes.

El instalador requiere el esquema existente CUN03/CUN05/CUN06/CUN07 y se ejecuta en una transacción. Las inclusiones usan `$(RepoRoot)`, con valor predeterminado `.` para ejecutarlo desde la raíz de cualquier clon. En SSMS, activar **Consulta → Modo SQLCMD** y reemplazar únicamente la línea `:setvar RepoRoot "."` por la ruta absoluta de la raíz de ese clon, conservando las comillas. No ejecutar los fixtures ni los archivos `*_pruebas*.sql` en la base de la aplicación.

## Reglas económicas

| Dato | Significado |
|---|---|
| `Comision.ArancelBase` | Precio total del curso, excluida la matrícula |
| `Comision.MontoMatricula` | Pago único independiente, sin recargo del plan |
| `PlanesDePago.CantidadCuotas` | 1 para Contado, 3 o 6 según el plan |
| `RecargoPorcentaje` | Porcentaje aplicado al precio total, sin capitalización mensual |
| Snapshots de inscripción | Precio, cantidad, recargo, nombre del plan, total y primer vencimiento acordados |

Para un curso de $120.000 con recargo de 5,125 %, el total financiado es $126.150 y se generan tres cuotas de $42.050. La matrícula se cobra aparte. Los centavos indivisibles se asignan a la última cuota.

La primera cuota vence en `FechaInicio`. Las siguientes se calculan sumando meses **a esa fecha original**: 31 de enero → 28/29 de febrero → 31 de marzo. La duración académica no determina cuántas cuotas tiene el plan. Cada cuota guarda su período mensual, importe, saldo y estado.

La matrícula conserva el número interno histórico 1. Las cuotas del curso usan internamente 2..N+1 y se presentan al usuario como 1..N; `TipoConcepto` distingue ambos conceptos. Los pagos y sus claves históricas se conservan. CUN07 requiere matrícula pagada, liquida una cuota completa por operación y rechaza cuotas con anteriores impagas. El estado financiero se calcula desde las cuotas; el estado académico `activa` sigue disponible para quórum y cancelación.

## Inscripciones existentes

- Se identifican matrícula y cuotas existentes sin cambiar importes, saldos, vencimientos ni pagos registrados.
- Si faltan condiciones históricas del curso, se toma **el precio y la fecha de inicio actuales de la comisión**, junto con el recargo ya guardado en la inscripción. Desde esa migración quedan congelados como snapshots.
- Se generan las cuotas faltantes de inscripciones vigentes. Las canceladas/anuladas no reciben nuevas deudas.
- Si una cuota del curso existente no coincide con el calendario o los importes calculados, el instalador informa el conflicto y revierte la transacción; requiere revisar esa inscripción, sin sobrescribir sus cobros.
- Los nombres legacy Contado/3 cuotas/6 cuotas se traducen una sola vez a cantidades explícitas. Para otros nombres hay que completar `CantidadCuotas`; el cálculo posterior nunca interpreta nombres.
- Se actualizan los DVH/DVV con las columnas nuevas. Las ejecuciones posteriores no recalculan indiscriminadamente hashes de datos ajenos a la migración.

## Verificación aislada

Después de compilar la solución en una carpeta separada:

```powershell
dotnet build Solucion.sln --no-restore -p:OutputPath="C:\Users\User\AppData\Local\Temp\opencode\financiacion-build\"
& ".\Scripts\Tests\FinanciacionCuotas.Tests.ps1"
```

La suite crea una base `FinanceTests_<GUID>` en LocalDB, prueba el modelo, generación, migración, pagos y quórum, verifica los hashes contra la implementación .NET y elimina la base al finalizar. No usa la configuración de conexión de la aplicación.

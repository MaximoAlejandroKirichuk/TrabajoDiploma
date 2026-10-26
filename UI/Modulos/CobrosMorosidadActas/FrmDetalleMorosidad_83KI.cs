using BLL;
using BE.DTOs.Pagos;
using Service.DTOs;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UI.Modulos.CobrosMorosidadActas
{
    // Vista de los datos del reporte: no consulta ni modifica pagos y no envía mensajes.
    public sealed class FrmDetalleMorosidad_83KI : FormConAyuda_83KI
    {
        public FrmDetalleMorosidad_83KI(ReporteMorosidadFila_83KI fila, DateTime fechaReporte)
        {
            if (fila == null) throw new ArgumentNullException(nameof(fila));
            Text = T("DetalleTitulo");
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(820, 650);
            Size = new Size(1020, 760);
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoScaleMode = AutoScaleMode.Font;

            var cuotas = KI68ReporteMorosidadBLL.ObtenerDetalleDeuda(fila);
            decimal vencida = cuotas.Where(c => KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(c, fechaReporte)).Sum(c => c.BalanceAdeudado);
            decimal total = cuotas.Sum(c => c.BalanceAdeudado);
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 7
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            Controls.Add(layout);

            var contacto = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = T("Contacto", fila.ApellidoYNombre, Dato(fila.DNI), Dato(fila.Email), Dato(fila.Telefono),
                    Dato(fila.CodigoInscripcion), Dato(fila.CodigoComision), Dato(fila.Curso), Dato(fila.PlanDePago))
            };
            layout.Controls.Add(contacto, 0, 0);
            layout.Controls.Add(new Label { Text = T("DetalleAviso", fechaReporte.ToString("dd/MM/yyyy HH:mm")), Dock = DockStyle.Fill }, 0, 1);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                AllowUserToOrderColumns = true, BackgroundColor = SystemColors.Window
            };
            AgregarColumna(grid, "IdCuota", "IdCuota");
            AgregarColumna(grid, "Numero", "NumeroRegistro");
            AgregarColumna(grid, "Concepto", "Concepto");
            AgregarColumna(grid, "Periodo", "Periodo");
            AgregarColumna(grid, "Vencimiento", "Vencimiento", "dd/MM/yyyy");
            AgregarColumna(grid, "Monto", "MontoOriginal", "N2");
            AgregarColumna(grid, "Saldo", "Saldo", "N2");
            AgregarColumna(grid, "Situacion", "Situacion");
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            foreach (var cuota in cuotas)
            {
                string concepto = cuota.EsMatricula ? T("Matricula") : T("Mensualidad", cuota.NumeroMensualidad);
                string situacion = KI68ReporteMorosidadBLL.EsCuotaVencidaImpaga(cuota, fechaReporte)
                    ? T("Vencida") : T(cuota.FechaVencimiento.Date == fechaReporte.Date ? "VenceHoy" : "Futura");
                grid.Rows.Add(cuota.IdCuota, cuota.NumeroCuota, concepto,
                    cuota.Periodo.HasValue ? cuota.Periodo.Value.ToString("MM/yyyy") : T("SinPeriodo"),
                    cuota.FechaVencimiento, cuota.MontoOriginal, cuota.BalanceAdeudado, situacion);
            }
            layout.Controls.Add(grid, 0, 2);
            layout.Controls.Add(new Label { Dock = DockStyle.Fill, Text = T("DetalleTotales", vencida.ToString("N2"), total.ToString("N2")) }, 0, 3);
            layout.Controls.Add(new Label { Dock = DockStyle.Fill, Text = T("MensajeAviso") }, 0, 4);

            var mensaje = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, AcceptsReturn = true,
                ScrollBars = ScrollBars.Vertical,
                Text = T("MensajePropuesto", fila.ApellidoYNombre, fila.CodigoInscripcion, fila.Curso,
                    fila.CodigoComision, fechaReporte.ToString("dd/MM/yyyy"), vencida.ToString("N2"))
            };
            layout.Controls.Add(mensaje, 0, 5);
            var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var cerrar = new Button { Text = T("CerrarDetalle"), AutoSize = true, DialogResult = DialogResult.Cancel };
            var copiar = new Button { Text = T("CopiarMensaje"), AutoSize = true };
            copiar.Click += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(mensaje.Text)) return;
                try { Clipboard.SetText(mensaje.Text); }
                catch (ExternalException)
                {
                    MessageBox.Show(this, T("ErrorCopiar"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            botones.Controls.Add(cerrar);
            botones.Controls.Add(copiar);
            layout.Controls.Add(botones, 0, 6);
            CancelButton = cerrar;
        }

        private static void AgregarColumna(DataGridView grid, string nombre, string clave, string formato = null)
        {
            var columna = new DataGridViewTextBoxColumn { Name = nombre, HeaderText = T(clave) };
            if (formato != null) columna.DefaultCellStyle.Format = formato;
            grid.Columns.Add(columna);
        }

        private static string Dato(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? T("SinDato") : valor;
        }

        private static string T(string clave, params object[] argumentos)
        {
            return IdiomaUiHelper_83KI.Texto("ReporteMorosidad." + clave, argumentos);
        }
    }
}

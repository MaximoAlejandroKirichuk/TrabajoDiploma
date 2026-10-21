using Service.DTOs;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;

namespace UI.Modulos.PreInscripcion
{
    internal sealed class FacturaPagoPdfExporter_83KI
    {
        private const string NombreImpresoraPdf = "Microsoft Print to PDF";
        private PagoInscripcionResultado_83KI _resultado;

        public void Exportar(PagoInscripcionResultado_83KI resultado, string rutaArchivo)
        {
            if (resultado == null) throw new ArgumentNullException(nameof(resultado));
            if (string.IsNullOrWhiteSpace(rutaArchivo)) throw new ArgumentException("La ruta de la factura es obligatoria.", nameof(rutaArchivo));
            if (!ExisteImpresoraPdf()) throw new InvalidOperationException("No está disponible la impresora Microsoft Print to PDF.");

            _resultado = resultado;
            using (var documento = new PrintDocument())
            {
                documento.DocumentName = "Factura " + resultado.NumeroReciboInstitucional;
                documento.PrinterSettings.PrinterName = NombreImpresoraPdf;
                documento.PrinterSettings.PrintToFile = true;
                documento.PrinterSettings.PrintFileName = rutaArchivo;
                documento.PrintController = new StandardPrintController();
                documento.PrintPage += DibujarFactura;
                documento.Print();
            }

            var informacion = new FileInfo(rutaArchivo);
            if (!informacion.Exists || informacion.Length == 0)
                throw new IOException("La impresora no generó el archivo PDF.");
        }

        private void DibujarFactura(object sender, PrintPageEventArgs e)
        {
            RectangleF area = e.MarginBounds;
            using (var titulo = new Font("Segoe UI", 24, FontStyle.Bold))
            using (var subtitulo = new Font("Segoe UI", 10, FontStyle.Regular))
            using (var etiqueta = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var valor = new Font("Segoe UI", 12, FontStyle.Regular))
            using (var separador = new Pen(Color.FromArgb(45, 94, 130), 2))
            using (var colorTitulo = new SolidBrush(Color.FromArgb(32, 60, 82)))
            {
                float y = area.Top + 18;
                e.Graphics.DrawString("Factura", titulo, colorTitulo, area.Left, y);
                y += 42;
                e.Graphics.DrawString("Comprobante de pago de cuota del curso", subtitulo, Brushes.DimGray, area.Left, y);
                y += 28;
                e.Graphics.DrawLine(separador, area.Left, y, area.Right, y);
                y += 28;
                DibujarCampo(e.Graphics, etiqueta, valor, "N.º de recibo institucional", _resultado.NumeroReciboInstitucional, area.Left, ref y);
                DibujarCampo(e.Graphics, etiqueta, valor, "Concepto abonado", _resultado.FacturaConcepto, area.Left, ref y);
                DibujarCampo(e.Graphics, etiqueta, valor, "Total facturado", "$ " + _resultado.FacturaTotal.ToString("N2", CultureInfo.GetCultureInfo("es-AR")), area.Left, ref y);
                DibujarCampo(e.Graphics, etiqueta, valor, "Fecha de la transacción", _resultado.FacturaFecha.ToString("dd/MM/yyyy HH:mm"), area.Left, ref y);
                if (_resultado.Alumno != null)
                {
                    DibujarCampo(e.Graphics, etiqueta, valor, "Alumno", (_resultado.Alumno.Nombre + " " + _resultado.Alumno.Apellido).Trim(), area.Left, ref y);
                    DibujarCampo(e.Graphics, etiqueta, valor, "DNI", _resultado.Alumno.DNI, area.Left, ref y);
                }
                if (!string.IsNullOrWhiteSpace(_resultado.Curso) || !string.IsNullOrWhiteSpace(_resultado.CodigoComision))
                    DibujarCampo(e.Graphics, etiqueta, valor, "Curso / comisión", (_resultado.Curso + " / " + _resultado.CodigoComision).Trim(' ', '/'), area.Left, ref y);
                e.HasMorePages = false;
            }
        }

        private static void DibujarCampo(Graphics graphics, Font etiqueta, Font valor, string nombre, string contenido, float x, ref float y)
        {
            graphics.DrawString(nombre, etiqueta, Brushes.DimGray, x, y);
            y += 18;
            graphics.DrawString(contenido ?? string.Empty, valor, Brushes.Black, x, y);
            y += 34;
        }

        private static bool ExisteImpresoraPdf()
        {
            foreach (string impresora in PrinterSettings.InstalledPrinters)
                if (string.Equals(impresora, NombreImpresoraPdf, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    // Exportación a PDF compartida por los reportes: recibe grillas ya cargadas y las imprime como tablas.
    internal static class ExportadorTablasPdf_83KI
    {
        private const string NombreImpresoraPdf = "Microsoft Print to PDF";

        private sealed class Seccion
        {
            public string Titulo;
            public string[] Cabeceras;
            public List<string[]> Filas;
        }

        // Flujo completo desde una pantalla: pide dónde guardar, exporta y avisa el resultado.
        // Cada tabla es el título de la sección y la grilla de donde se toman columnas visibles y filas.
        public static async Task ExportarConDialogoAsync(Form pantalla, Button boton, string nombreArchivo, string titulo,
            string filtros, string nota, params KeyValuePair<string, DataGridView>[] tablas)
        {
            try
            {
                using (var dialogo = new SaveFileDialog { Title = IdiomaUiHelper_83KI.Texto("Comun.ExportarPdf"),
                    Filter = "PDF (*.pdf)|*.pdf", DefaultExt = "pdf", AddExtension = true,
                    FileName = nombreArchivo + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf" })
                {
                    if (dialogo.ShowDialog(pantalla) != DialogResult.OK) return;
                    boton.Enabled = false;
                    pantalla.UseWaitCursor = true;
                    await ExportarAsync(dialogo.FileName, titulo, filtros, nota, IdiomaUiHelper_83KI.Texto("Comun.SinDatosPdf"), tablas);
                    // Si la pantalla se cerró mientras se generaba el PDF, ya no hay ventana sobre la cual avisar.
                    if (!pantalla.IsDisposed) IdiomaUiHelper_83KI.MostrarInformacion(pantalla, "Comun.PdfExportado", "Comun.Informacion");
                }
            }
            catch (Exception ex)
            {
                if (!pantalla.IsDisposed) IdiomaUiHelper_83KI.MostrarError(pantalla, ex, "Comun.Validacion", MessageBoxIcon.Warning);
            }
            finally
            {
                if (!pantalla.IsDisposed)
                {
                    pantalla.UseWaitCursor = false;
                    boton.Enabled = true;
                }
            }
        }

        public static async Task ExportarAsync(string ruta, string titulo, string filtros, string nota, string textoSinDatos,
            IList<KeyValuePair<string, DataGridView>> tablas)
        {
            if (!PrinterSettings.InstalledPrinters.Cast<string>().Any(p =>
                string.Equals(p, NombreImpresoraPdf, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Errores.ImpresoraPdfNoDisponible");
            VerificarArchivoDisponible(ruta);

            // Las grillas se leen en el hilo de la pantalla; la impresión se hace aparte para no congelarla.
            List<Seccion> secciones = CrearSecciones(tablas);
            var anterior = new FileInfo(ruta);
            DateTime? modificacionAnterior = anterior.Exists ? anterior.LastWriteTimeUtc : (DateTime?)null;
            await Task.Run(() => Imprimir(ruta, titulo, filtros, nota, textoSinDatos, secciones));
            await EsperarArchivoAsync(ruta, modificacionAnterior);
        }

        // Si el PDF elegido está abierto en otro programa, la impresora no puede escribirlo: se avisa antes de imprimir.
        private static void VerificarArchivoDisponible(string ruta)
        {
            if (!File.Exists(ruta)) return;
            try
            {
                using (new FileStream(ruta, FileMode.Open, FileAccess.Write, FileShare.None)) { }
            }
            catch (IOException)
            {
                throw new InvalidOperationException("Errores.ArchivoPdfEnUso");
            }
        }

        private static List<Seccion> CrearSecciones(IList<KeyValuePair<string, DataGridView>> tablas)
        {
            var secciones = new List<Seccion>();
            foreach (var tabla in tablas)
            {
                var grilla = tabla.Value;
                var columnas = grilla.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible)
                    .OrderBy(c => c.DisplayIndex).ToArray();
                // Las tablas anchas se parten en grupos de columnas y cada grupo repite la columna que identifica la fila.
                for (int inicio = 1; inicio < Math.Max(2, columnas.Length); inicio += 5)
                {
                    var grupo = columnas.Take(1).Concat(columnas.Skip(inicio).Take(5)).ToArray();
                    secciones.Add(new Seccion
                    {
                        Titulo = tabla.Key,
                        Cabeceras = grupo.Select(c => c.HeaderText).ToArray(),
                        Filas = grilla.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow)
                            .Select(r => grupo.Select(c => LimitarTexto(Convert.ToString(r.Cells[c.Index].FormattedValue))).ToArray()).ToList()
                    });
                }
            }
            return secciones;
        }

        // GDI+ falla al medir textos de decenas de miles de caracteres; una celda nunca necesita tanto en el PDF.
        private static string LimitarTexto(string texto)
        {
            const int maximo = 2000;
            return texto.Length <= maximo ? texto : texto.Substring(0, maximo) + "...";
        }

        private static void Imprimir(string ruta, string titulo, string filtros, string nota, string textoSinDatos, List<Seccion> secciones)
        {
            int seccion = 0, fila = 0, numeroPagina = 0;
            using (var documento = new PrintDocument())
            using (var fuenteTitulo = new Font("Segoe UI", 14, FontStyle.Bold))
            using (var fuenteCabecera = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var fuente = new Font("Segoe UI", 9))
            using (var formato = new StringFormat { Trimming = StringTrimming.Word })
            using (var formatoRecortado = new StringFormat { Trimming = StringTrimming.EllipsisWord, FormatFlags = StringFormatFlags.LineLimit })
            {
                documento.DocumentName = titulo;
                documento.PrinterSettings.PrinterName = NombreImpresoraPdf;
                documento.PrinterSettings.PrintToFile = true;
                documento.PrinterSettings.PrintFileName = ruta;
                documento.DefaultPageSettings.Landscape = true;
                documento.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
                documento.PrintController = new StandardPrintController();
                documento.PrintPage += (s, e) =>
                {
                    var actual = secciones[seccion];
                    RectangleF area = e.MarginBounds;
                    float y = area.Top;
                    DibujarTexto(e.Graphics, titulo, fuenteTitulo, formato, area, ref y);
                    DibujarTexto(e.Graphics, filtros, fuente, formato, area, ref y);
                    DibujarTexto(e.Graphics, nota, fuente, formato, area, ref y);
                    DibujarTexto(e.Graphics, actual.Titulo, fuenteCabecera, formato, area, ref y);
                    float ancho = area.Width / Math.Max(1, actual.Cabeceras.Length);
                    float altoCabecera = AltoFila(e.Graphics, actual.Cabeceras, fuenteCabecera, formato, ancho);
                    DibujarFila(e.Graphics, actual.Cabeceras, fuenteCabecera, formato, area.Left, y, ancho, altoCabecera, true);
                    y += altoCabecera;
                    float limite = area.Bottom - 25;
                    float inicioFilas = y;
                    float altoMaximo = limite - inicioFilas;
                    while (fila < actual.Filas.Count)
                    {
                        string[] valores = actual.Filas[fila];
                        float alto = AltoFila(e.Graphics, valores, fuente, formato, ancho);
                        // Una fila más alta que la página se recorta con puntos suspensivos en vez de cortar la exportación.
                        bool recortada = alto > altoMaximo;
                        if (recortada) alto = altoMaximo;
                        if (y + alto > limite && y > inicioFilas) break;
                        DibujarFila(e.Graphics, valores, fuente, recortada ? formatoRecortado : formato, area.Left, y, ancho, alto, false);
                        y += alto;
                        fila++;
                    }
                    if (actual.Filas.Count == 0)
                        e.Graphics.DrawString(textoSinDatos, fuente, Brushes.Black, area.Left, y + 8);
                    numeroPagina++;
                    e.Graphics.DrawString(numeroPagina.ToString(), fuente, Brushes.DimGray, area.Right - 35, area.Bottom - 15);
                    if (fila == actual.Filas.Count) { seccion++; fila = 0; }
                    e.HasMorePages = seccion < secciones.Count;
                };
                documento.Print();
            }
        }

        private static async Task EsperarArchivoAsync(string ruta, DateTime? modificacionAnterior)
        {
            var tiempo = Stopwatch.StartNew();
            while (tiempo.Elapsed < TimeSpan.FromSeconds(30))
            {
                try
                {
                    var archivo = new FileInfo(ruta);
                    if (archivo.Exists && (!modificacionAnterior.HasValue || archivo.LastWriteTimeUtc != modificacionAnterior.Value))
                    {
                        // La cola de impresión puede seguir escribiendo después de que Print retorne.
                        using (var flujo = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            if (flujo.Length >= 5)
                            {
                                var cabecera = new byte[5];
                                flujo.Read(cabecera, 0, cabecera.Length);
                                flujo.Seek(Math.Max(0, flujo.Length - 1024), SeekOrigin.Begin);
                                using (var lector = new StreamReader(flujo, Encoding.ASCII))
                                {
                                    if (Encoding.ASCII.GetString(cabecera) == "%PDF-" && lector.ReadToEnd().Contains("%%EOF"))
                                        return;
                                }
                            }
                        }
                    }
                }
                catch (IOException) { }
                await Task.Delay(100).ConfigureAwait(false);
            }
            throw new IOException("Errores.PdfNoGenerado");
        }

        private static void DibujarTexto(Graphics g, string texto, Font fuente, StringFormat formato, RectangleF area, ref float y)
        {
            float alto = g.MeasureString(texto, fuente, (int)area.Width, formato).Height + 8;
            g.DrawString(texto, fuente, Brushes.Black, new RectangleF(area.Left, y, area.Width, alto), formato);
            y += alto;
        }

        private static float AltoFila(Graphics g, string[] valores, Font fuente, StringFormat formato, float ancho)
        {
            return Math.Max(26, valores.Select(v => g.MeasureString(v, fuente, (int)(ancho - 8), formato).Height + 10)
                .DefaultIfEmpty(26).Max());
        }

        private static void DibujarFila(Graphics g, string[] valores, Font fuente, StringFormat formato,
            float x, float y, float ancho, float alto, bool cabecera)
        {
            for (int i = 0; i < valores.Length; i++)
            {
                var celda = new RectangleF(x + i * ancho, y, ancho, alto);
                if (cabecera) g.FillRectangle(Brushes.Gainsboro, celda);
                g.DrawRectangle(Pens.LightGray, celda.X, celda.Y, celda.Width, celda.Height);
                g.DrawString(valores[i], fuente, Brushes.Black,
                    new RectangleF(celda.X + 4, y + 4, ancho - 8, alto - 8), formato);
            }
        }
    }
}

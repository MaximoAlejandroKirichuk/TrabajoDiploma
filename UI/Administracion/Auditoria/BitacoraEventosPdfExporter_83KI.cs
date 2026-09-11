using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace UI.Exportacion
{
    public class BitacoraEventosPdfExporter_83KI : IBitacoraEventosExporter_83KI
    {
        private const string NombreImpresoraPdf = "Microsoft Print to PDF";
        private readonly List<BitacoraEventoVista_83KI> _eventos = new List<BitacoraEventoVista_83KI>();
        private int _filaActual;

        public bool PuedeExportar(out string mensaje)
        {
            if (!ExisteImpresoraPdf())
            {
                mensaje = "No se encontro la impresora Microsoft Print to PDF. Verifique que este instalada y habilitada.";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

        public void Exportar(IEnumerable<BitacoraEventoVista_83KI> eventos, string rutaArchivo)
        {
            if (eventos == null)
            {
                throw new ArgumentNullException(nameof(eventos));
            }

            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException("La ruta de exportacion es obligatoria.", nameof(rutaArchivo));
            }

            string mensaje;
            if (!PuedeExportar(out mensaje))
            {
                throw new InvalidOperationException(mensaje);
            }

            _eventos.Clear();
            _eventos.AddRange(eventos);
            _filaActual = 0;

            using (PrintDocument documento = new PrintDocument())
            {
                documento.DocumentName = "Bitacora de Eventos";
                documento.DefaultPageSettings.Landscape = true;
                documento.PrinterSettings.PrinterName = NombreImpresoraPdf;
                documento.PrinterSettings.PrintToFile = true;
                documento.PrinterSettings.PrintFileName = rutaArchivo;
                documento.PrintController = new StandardPrintController();
                documento.PrintPage += Documento_PrintPage;
                documento.Print();
            }
        }

        private void Documento_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (Font fuenteTitulo = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Font fuenteCabecera = new Font("Segoe UI", 9, FontStyle.Bold))
            using (Font fuenteDetalle = new Font("Segoe UI", 8))
            using (StringFormat formato = CrearFormato())
            using (StringFormat formatoCabecera = CrearFormatoCabecera())
            {
                RectangleF area = e.MarginBounds;
                float y = area.Top;

                e.Graphics.DrawString("Bitacora de Eventos", fuenteTitulo, Brushes.Black, area.Left, y);
                y += 30;
                e.Graphics.DrawString("Fecha de impresion: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fuenteDetalle, Brushes.Black, area.Left, y);
                y += 35;

                Columna[] columnas = CrearColumnas(area);
                DibujarCabecera(e.Graphics, fuenteCabecera, formatoCabecera, columnas, y);
                y += 26;

                float primerYDetalle = y;
                float altoDisponiblePagina = area.Bottom - primerYDetalle;

                while (_filaActual < _eventos.Count)
                {
                    BitacoraEventoVista_83KI evento = _eventos[_filaActual];
                    string[] valores = CrearValores(evento);
                    float altoFila = CalcularAltoFila(e.Graphics, fuenteDetalle, formato, columnas, valores);

                    if (altoFila > altoDisponiblePagina)
                    {
                        altoFila = altoDisponiblePagina;
                        formato.Trimming = StringTrimming.EllipsisWord;
                        formato.FormatFlags |= StringFormatFlags.LineLimit;
                    }

                    if (y + altoFila > area.Bottom && y > primerYDetalle)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    DibujarFila(e.Graphics, fuenteDetalle, formato, columnas, valores, y, altoFila);
                    y += altoFila;
                    _filaActual++;
                    formato.Trimming = StringTrimming.Word;
                    formato.FormatFlags &= ~StringFormatFlags.LineLimit;
                }

                e.HasMorePages = false;
            }
        }

        private Columna[] CrearColumnas(RectangleF area)
        {
            Columna[] columnas = new[]
            {
                new Columna("Fecha", area.Left, area.Width * 0.14f),
                new Columna("Username", 0, area.Width * 0.14f),
                new Columna("Nombre", 0, area.Width * 0.12f),
                new Columna("Apellido", 0, area.Width * 0.12f),
                new Columna("Modulo", 0, area.Width * 0.10f),
                new Columna("Crit.", 0, area.Width * 0.08f),
                new Columna("Evento", 0, area.Width * 0.30f)
            };

            float x = area.Left;
            foreach (Columna columna in columnas)
            {
                columna.X = x;
                x += columna.Ancho;
            }

            return columnas;
        }

        private void DibujarCabecera(Graphics grafico, Font fuente, StringFormat formato, Columna[] columnas, float y)
        {
            foreach (Columna columna in columnas)
            {
                RectangleF rectangulo = new RectangleF(columna.X, y, columna.Ancho, 24);
                grafico.DrawString(columna.Titulo, fuente, Brushes.Black, rectangulo, formato);
            }

            grafico.DrawLine(Pens.Black, columnas[0].X, y + 23, columnas[columnas.Length - 1].X + columnas[columnas.Length - 1].Ancho, y + 23);
        }

        private void DibujarFila(Graphics grafico, Font fuente, StringFormat formato, Columna[] columnas, string[] valores, float y, float alto)
        {
            for (int i = 0; i < columnas.Length; i++)
            {
                RectangleF rectangulo = new RectangleF(columnas[i].X, y, columnas[i].Ancho - 4, alto);
                grafico.DrawString(valores[i], fuente, Brushes.Black, rectangulo, formato);
            }
        }

        private float CalcularAltoFila(Graphics grafico, Font fuente, StringFormat formato, Columna[] columnas, string[] valores)
        {
            float alto = 24;

            for (int i = 0; i < columnas.Length; i++)
            {
                SizeF medida = grafico.MeasureString(valores[i], fuente, new SizeF(columnas[i].Ancho - 4, 1000), formato);
                alto = Math.Max(alto, medida.Height + 8);
            }

            return alto;
        }

        private string[] CrearValores(BitacoraEventoVista_83KI evento)
        {
            return new[]
            {
                evento.Fecha.ToString("dd/MM/yyyy HH:mm"),
                evento.Username,
                evento.Nombre,
                evento.Apellido,
                evento.Modulo.ToString(),
                evento.Criticidad.ToString(),
                evento.Evento
            };
        }

        private StringFormat CrearFormato()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.Word
            };
        }

        private StringFormat CrearFormatoCabecera()
        {
            StringFormat formato = CrearFormato();
            formato.Trimming = StringTrimming.EllipsisCharacter;
            return formato;
        }

        private bool ExisteImpresoraPdf()
        {
            foreach (string impresora in PrinterSettings.InstalledPrinters)
            {
                if (string.Equals(impresora, NombreImpresoraPdf, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private class Columna
        {
            public Columna(string titulo, float x, float ancho)
            {
                Titulo = titulo;
                X = x;
                Ancho = ancho;
            }

            public string Titulo { get; private set; }
            public float X { get; set; }
            public float Ancho { get; private set; }
        }
    }
}

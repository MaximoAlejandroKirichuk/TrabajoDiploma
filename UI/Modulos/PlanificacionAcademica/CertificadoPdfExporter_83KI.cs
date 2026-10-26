using BE.Entidades;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace UI.Modulos.PlanificacionAcademica
{
    public sealed class CertificadoPdfExporter_83KI
    {
        public static string Contenido(CertificadoFinalizacion_83KI c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            return "CERTIFICADO DE FINALIZACIÓN\r\n\r\n" + c.Nombre + " " + c.Apellido + "\r\n\r\nCurso: " + c.Curso +
                "\r\nPromedio: " + c.Promedio.ToString("0.################", CultureInfo.InvariantCulture) + " — Aprobado\r\nFecha: " + c.Fecha.ToString("dd/MM/yyyy") +
                "\r\nFolio: " + c.Folio + "\r\n\r\nFirmante: " + c.Firmante + "\r\nCargo: " + c.Cargo +
                "\r\nModalidad: " + c.Modalidad + "\r\nEmail registrado: " + c.Email + "\r\n\r\nMención: " + c.Mencion + "\r\n\r\nObservaciones: " + c.Observaciones;
        }
        public static string NombreArchivo(CertificadoFinalizacion_83KI c)
        {
            string name = c.Nombre + " " + c.Apellido + " - " + c.Curso;
            var invalid = Path.GetInvalidFileNameChars();
            name = new string(name.Select(ch => invalid.Contains(ch) || char.IsControl(ch) ? '_' : ch).ToArray()).Trim().TrimEnd('.');
            if (name.Length > 150) name = name.Substring(0, 150).TrimEnd('.', ' ');
            return "Certificado - " + name + ".pdf";
        }
        public void Exportar(CertificadoFinalizacion_83KI c, string path)
        {
            if (!string.Equals(Path.GetExtension(path), ".pdf", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Cun09.ErrorGuardar");
            if (c == null) throw new ArgumentNullException(nameof(c));
            string directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!Directory.Exists(directory)) throw new InvalidOperationException("Cun09.ErrorGuardar");
            string temp = Path.Combine(directory, ".certificado-" + Guid.NewGuid().ToString("N") + ".pdf");
            try
            {
                using (var doc = new PrintDocument())
                {
                    doc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                    if (!doc.PrinterSettings.IsValid) throw new InvalidOperationException("Cun09.Impresora");
                    doc.DocumentName = "Certificado de finalización";
                    doc.DefaultPageSettings.Landscape = true;
                    doc.DefaultPageSettings.Margins = new Margins(32, 32, 30, 30);
                    doc.PrinterSettings.PrintToFile = true;
                    doc.PrinterSettings.PrintFileName = temp;
                    doc.PrintController = new StandardPrintController();
                    doc.PrintPage += (sender, e) => DibujarCertificado(e.Graphics, e.MarginBounds, c);
                    doc.Print();
                }
                // El servicio de cola de impresión de Windows puede finalizar después de que retorne Print. Nunca afirmar que un trabajo en cola es un PDF guardado.
                bool copied = false;
                for (int attempt = 0; attempt < 100 && !copied; attempt++)
                {
                    try
                    {
                        using (var input = new FileStream(temp, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            if (input.Length < 5 || input.ReadByte() != '%' || input.ReadByte() != 'P' || input.ReadByte() != 'D' || input.ReadByte() != 'F') throw new IOException();
                            input.Position = 0;
                            using (var output = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)) input.CopyTo(output);
                            copied = true;
                        }
                    }
                    catch (IOException) { if (attempt == 99) throw; Thread.Sleep(100); }
                }
            }
            finally { try { if (File.Exists(temp)) File.Delete(temp); } catch (IOException) { } }
        }

        private static void DibujarCertificado(Graphics graphics, Rectangle bounds, CertificadoFinalizacion_83KI c)
        {
            Color navy = Color.FromArgb(29, 50, 76);
            Color teal = Color.FromArgb(49, 119, 124);
            Color gold = Color.FromArgb(190, 151, 78);
            Color muted = Color.FromArgb(95, 108, 120);
            Color pale = Color.FromArgb(244, 247, 248);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            float left = bounds.Left + 14;
            float right = bounds.Right - 14;
            float width = right - left;
            float y = bounds.Top + 14;
            using (var border = new Pen(Color.FromArgb(205, 213, 218), 1.2f))
            using (var accent = new Pen(gold, 3f))
            using (var eyebrowFont = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var titleFont = new Font("Georgia", 23f, FontStyle.Regular))
            using (var nameFont = new Font("Georgia", 20f, FontStyle.Bold))
            using (var bodyFont = new Font("Segoe UI", 11f, FontStyle.Regular))
            using (var labelFont = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (var valueFont = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (var smallFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var signatureFont = new Font("Georgia", 12f, FontStyle.Regular))
            using (var signatureTitleFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var rightAlign = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
            {
                graphics.DrawRectangle(border, bounds.Left + 5, bounds.Top + 5, bounds.Width - 10, bounds.Height - 10);
                graphics.DrawLine(accent, left, y, right, y);
                y += 17;
                DrawText(graphics, "CERTIFICADO DE FINALIZACIÓN", eyebrowFont, teal, new RectangleF(left, y, width, 18), center);

                DrawText(graphics, "FOLIO  " + c.Folio, smallFont, muted, new RectangleF(left, y, width, 18), rightAlign);
                y += 36;
                DrawText(graphics, "Certificado de finalización", titleFont, navy, new RectangleF(left, y, width, 42), center);
                y += 52;
                DrawText(graphics, "Se certifica que", bodyFont, muted, new RectangleF(left, y, width, 20), center);
                y += 23;
                DrawText(graphics, (c.Nombre + " " + c.Apellido).Trim(), nameFont, navy, new RectangleF(left, y, width, 38), center);
                y += 45;

                string completion = "ha completado satisfactoriamente el curso";
                DrawText(graphics, completion, bodyFont, muted, new RectangleF(left, y, width, 20), center);
                y += 25;

                float cardGap = 16;
                float cardWidth = (width - cardGap) / 2;
                float cardHeight = 62;
                DrawCard(graphics, new RectangleF(left, y, cardWidth, cardHeight), "CURSO", c.Curso, pale, teal, navy, labelFont, valueFont);
                DrawCard(graphics, new RectangleF(left + cardWidth + cardGap, y, cardWidth, cardHeight), "PROMEDIO FINAL", c.Promedio.ToString("0.################", CultureInfo.InvariantCulture) + "  ·  APROBADO", pale, teal, navy, labelFont, valueFont);
                y += cardHeight + 16;

                float detailGap = 24;
                float detailWidth = (width - detailGap) / 2;
                float detailHeight = DrawDetail(graphics, "MODALIDAD", c.Modalidad, left, y, detailWidth, labelFont, smallFont, teal, navy);
                detailHeight = Math.Max(detailHeight, DrawDetail(graphics, "EMAIL REGISTRADO", c.Email, left + detailWidth + detailGap,
                    y, detailWidth, labelFont, smallFont, teal, navy));
                y += detailHeight + 12;

                float noteWidth = width * 0.62f;
                DrawOptionalBlock(graphics, "Mención", c.Mencion, left, y, noteWidth, bodyFont, labelFont, navy, muted, out float mentionHeight);
                DrawText(graphics, "Fecha de emisión\r\n" + c.Fecha.ToString("dd/MM/yyyy"), bodyFont, muted,
                    new RectangleF(left + noteWidth + 24, y + 2, width - noteWidth - 24, Math.Max(42, mentionHeight)), rightAlign);
                y += Math.Max(mentionHeight, 28) + 8;
                DrawOptionalBlock(graphics, "Observaciones", c.Observaciones, left, y, width, bodyFont, labelFont, navy, muted, out float observationsHeight);
                y += observationsHeight + 8;

                float signatureY = Math.Max(y + 2, bounds.Bottom - 96);
                float signatureWidth = Math.Min(330, width * 0.42f);
                float signatureX = left + (width - signatureWidth) / 2;
                using (var rule = new Pen(Color.FromArgb(150, 160, 168), 1f))
                using (var signerBrush = new SolidBrush(navy))
                using (var roleBrush = new SolidBrush(muted))
                {
                    graphics.DrawLine(rule, signatureX, signatureY, signatureX + signatureWidth, signatureY);
                    graphics.DrawString(c.Firmante, signatureFont, signerBrush, new RectangleF(signatureX, signatureY + 5, signatureWidth, 23), center);
                    graphics.DrawString(c.Cargo, signatureTitleFont, roleBrush, new RectangleF(signatureX, signatureY + 29, signatureWidth, 18), center);
                }
                graphics.DrawLine(accent, left, bounds.Bottom - 18, right, bounds.Bottom - 18);
            }
        }

        private static void DrawCard(Graphics graphics, RectangleF bounds, string label, string value, Color background,
            Color labelColor, Color valueColor, Font labelFont, Font valueFont)
        {
            using (var fill = new SolidBrush(background))
            using (var rule = new Pen(Color.FromArgb(220, 226, 229), 1f))
            using (var labelBrush = new SolidBrush(labelColor))
            using (var valueBrush = new SolidBrush(valueColor))
            using (var format = new StringFormat { Trimming = StringTrimming.EllipsisWord, LineAlignment = StringAlignment.Center })
            {
                graphics.FillRectangle(fill, bounds);
                graphics.DrawRectangle(rule, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                graphics.DrawString(label, labelFont, labelBrush, new RectangleF(bounds.X + 12, bounds.Y + 7, bounds.Width - 24, 13));
                graphics.DrawString(value, valueFont, valueBrush, new RectangleF(bounds.X + 12, bounds.Y + 23, bounds.Width - 24, bounds.Height - 28), format);
            }
        }

        private static void DrawText(Graphics graphics, string text, Font font, Color color, RectangleF bounds, StringFormat format)
        {
            using (var brush = new SolidBrush(color)) graphics.DrawString(text, font, brush, bounds, format);
        }

        private static float DrawDetail(Graphics graphics, string label, string value, float x, float y, float width,
            Font labelFont, Font valueFont, Color labelColor, Color valueColor)
        {
            using (var labelBrush = new SolidBrush(labelColor))
            using (var valueBrush = new SolidBrush(valueColor))
            using (var format = new StringFormat { FormatFlags = StringFormatFlags.LineLimit })
            {
                graphics.DrawString(label, labelFont, labelBrush, new RectangleF(x, y, width, 14));
                float valueHeight = Math.Max(18, graphics.MeasureString(value ?? "", valueFont, new SizeF(width, 160), format).Height + 2);
                graphics.DrawString(value ?? "", valueFont, valueBrush, new RectangleF(x, y + 16, width, valueHeight), format);
                return 16 + valueHeight;
            }
        }

        private static void DrawOptionalBlock(Graphics graphics, string label, string value, float x, float y, float width,
            Font bodyFont, Font labelFont, Color labelColor, Color valueColor, out float height)
        {
            if (string.IsNullOrWhiteSpace(value)) { height = 18; return; }
            using (var labelBrush = new SolidBrush(labelColor))
            using (var valueBrush = new SolidBrush(valueColor))
            using (var format = new StringFormat { Trimming = StringTrimming.Word, FormatFlags = StringFormatFlags.LineLimit })
            {
                graphics.DrawString(label.ToUpperInvariant(), labelFont, labelBrush, new RectangleF(x, y, width, 14));
                float textHeight = Math.Max(18, graphics.MeasureString(value, bodyFont, new SizeF(width, 200), format).Height + 2);
                var textBounds = new RectangleF(x, y + 17, width, textHeight);
                graphics.DrawString(value, bodyFont, valueBrush, textBounds, format);
                height = 17 + textHeight;
            }
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    internal static class DisenoAdaptable_83KI
    {
        public static void Aplicar(Form formulario, Size tamanoContenido)
        {
            formulario.SuspendLayout();
            var controles = formulario.Controls.Cast<Control>().ToArray();
            var barras = controles.OfType<ToolStrip>().Where(c => c.Dock != DockStyle.None).ToArray();
            int altoBarras = barras.Where(c => c.Dock == DockStyle.Top || c.Dock == DockStyle.Bottom).Sum(c => c.Height);
            var minimo = new Size(Math.Max(1, tamanoContenido.Width), Math.Max(1, tamanoContenido.Height - altoBarras));

            // El lienzo conserva el diseño mínimo; el viewport desplaza también
            // controles anclados a la derecha/abajo y contenedores con Dock.Fill.
            var viewport = new Panel
            {
                Name = "areaDesplazable", Dock = DockStyle.Fill, AutoScroll = true,
                TabStop = false, Size = minimo, AutoScrollMinSize = minimo
            };
            var contenido = new Panel
            {
                Name = "contenidoAdaptable", Size = minimo,
                Anchor = AnchorStyles.Top | AnchorStyles.Left, TabStop = false
            };
            contenido.SuspendLayout();
            foreach (var control in controles.Except(barras.Cast<Control>())) contenido.Controls.Add(control);
            int indice = 0;
            foreach (var control in controles.Except(barras.Cast<Control>())) contenido.Controls.SetChildIndex(control, indice++);
            viewport.Controls.Add(contenido);
            formulario.Controls.Add(viewport);
            viewport.BringToFront();
            contenido.ResumeLayout(true);

            bool ajustando = false;
            EventHandler ajustarContenido = (s, e) =>
            {
                if (ajustando) return;
                ajustando = true;
                try
                {
                    // Calcular ambas barras desde el área completa evita conservar
                    // una barra sobrante por el tamaño del lienzo del layout anterior.
                    bool horizontal = minimo.Width > viewport.Width;
                    bool vertical = minimo.Height > viewport.Height - (horizontal ? SystemInformation.HorizontalScrollBarHeight : 0);
                    horizontal = minimo.Width > viewport.Width - (vertical ? SystemInformation.VerticalScrollBarWidth : 0);
                    vertical = minimo.Height > viewport.Height - (horizontal ? SystemInformation.HorizontalScrollBarHeight : 0);
                    contenido.Size = new Size(
                        Math.Max(minimo.Width, viewport.Width - (vertical ? SystemInformation.VerticalScrollBarWidth : 0)),
                        Math.Max(minimo.Height, viewport.Height - (horizontal ? SystemInformation.HorizontalScrollBarHeight : 0)));
                }
                finally { ajustando = false; }
            };
            viewport.ClientSizeChanged += ajustarContenido;
            viewport.Layout += (s, e) => ajustarContenido(s, e);
            formulario.FormBorderStyle = FormBorderStyle.Sizable;
            formulario.MaximizeBox = true;
            formulario.MaximumSize = Size.Empty;
            formulario.MinimumSize = Size.Empty;
            formulario.ResumeLayout(true);
            ajustarContenido(null, EventArgs.Empty);

            AjustarAlMonitor(formulario);
            formulario.Shown += (s, e) => AjustarAlMonitor(formulario);
            formulario.ResizeEnd += (s, e) => AjustarAlMonitor(formulario);
        }

        private static void AjustarAlMonitor(Form formulario)
        {
            if (formulario.WindowState != FormWindowState.Normal) return;
            var area = Screen.FromControl(formulario).WorkingArea;
            formulario.MinimumSize = new Size(Math.Min(360, area.Width), Math.Min(240, area.Height));
            int ancho = Math.Min(formulario.Width, area.Width);
            int alto = Math.Min(formulario.Height, area.Height);
            int izquierda = Math.Max(area.Left, Math.Min(formulario.Left, area.Right - ancho));
            int arriba = Math.Max(area.Top, Math.Min(formulario.Top, area.Bottom - alto));
            formulario.Bounds = new Rectangle(izquierda, arriba, ancho, alto);
        }
    }
}

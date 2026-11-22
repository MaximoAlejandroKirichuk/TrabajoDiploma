using Service.Entidades;
using Service.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public class FormConAyuda_83KI : Form
    {
        private AccesoAyuda _acceso;
        private Size _tamanoInicial;
        private bool _disenoAdaptado;

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Windows puede limitar la ventana al monitor antes de crear el handle.
            // Guardar el tamaño solicitado evita perder el extremo del contenido.
            if (!_disenoAdaptado && !IsHandleCreated)
            {
                var borde = SizeFromClientSize(Size.Empty);
                if ((specified & BoundsSpecified.Width) != 0) _tamanoInicial.Width = Math.Max(1, width - borde.Width);
                if ((specified & BoundsSpecified.Height) != 0) _tamanoInicial.Height = Math.Max(1, height - borde.Height);
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (!_disenoAdaptado) _tamanoInicial = new Size(Math.Max(_tamanoInicial.Width, ClientSize.Width),
                Math.Max(_tamanoInicial.Height, ClientSize.Height));
            base.OnHandleCreated(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime || _disenoAdaptado) return;
            var tema = CatalogoAyuda_83KI.Buscar(GetType().Name);
            if (tema != null) _acceso = new AccesoAyuda(this, tema);
            _disenoAdaptado = true;
            var tamano = new Size(Math.Max(_tamanoInicial.Width, ClientSize.Width),
                Math.Max(_tamanoInicial.Height + (_acceso != null ? _acceso.AltoBarra : 0), ClientSize.Height));
            DisenoAdaptable_83KI.Aplicar(this, tamano);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1 && _acceso != null)
            {
                _acceso.Abrir();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _acceso != null) _acceso.Dispose();
            base.Dispose(disposing);
        }

        private sealed class AccesoAyuda : IObservadorIdioma, IDisposable
        {
            private readonly Form _form;
            private readonly TemaAyuda_83KI _tema;
            private readonly IGestorIdioma_83KI _idioma;
            private readonly ToolStrip _barra;
            private readonly ToolStripButton _boton;
            public int AltoBarra { get { return _barra.Height; } }

            public AccesoAyuda(Form form, TemaAyuda_83KI tema)
            {
                _form = form;
                _tema = tema;
                _idioma = ServiceFactory_83KI.GetGestorIdioma();
                _boton = new ToolStripButton { DisplayStyle = ToolStripItemDisplayStyle.Text, Alignment = ToolStripItemAlignment.Right };
                _boton.Click += (s, e) => Abrir();
                _barra = new ToolStrip { Dock = DockStyle.Bottom, GripStyle = ToolStripGripStyle.Hidden, TabStop = true };
                _barra.Items.Add(_boton);

                form.SuspendLayout();
                var controles = form.Controls.Cast<Control>().ToArray();
                var area = new Panel { Size = form.ClientSize, Dock = DockStyle.Fill };
                area.SuspendLayout();
                foreach (var control in controles) area.Controls.Add(control);
                for (int i = 0; i < controles.Length; i++) area.Controls.SetChildIndex(controles[i], i);
                form.ClientSize = new Size(form.ClientSize.Width, form.ClientSize.Height + _barra.Height);
                form.Controls.Add(area);
                form.Controls.Add(_barra);
                area.ResumeLayout();
                form.ResumeLayout(true);
                form.Activated += AlActivar;
                _idioma.Suscribir(this);
            }

            private void AlActivar(object sender, EventArgs e) { ActualizarIdioma(_idioma.IdiomaActual); }

            public void ActualizarIdioma(IIdioma idioma)
            {
                _boton.Text = IdiomaUiHelper_83KI.Texto("FrmPrincipal.Ayuda") + " (F1)";
                _boton.Enabled = _tema.Disponible();
            }

            public void Abrir()
            {
                if (!_tema.Disponible()) return;
                using (var ayuda = new FrmAyuda_83KI(_tema.Pantalla)) ayuda.ShowDialog(_form);
            }

            public void Dispose()
            {
                _form.Activated -= AlActivar;
                _idioma.Desuscribir(this);
            }
        }
    }
}

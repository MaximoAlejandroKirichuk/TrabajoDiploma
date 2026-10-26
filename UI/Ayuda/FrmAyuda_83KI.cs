using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    internal sealed class FrmAyuda_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorIdioma_83KI _idioma = ServiceFactory_83KI.GetGestorIdioma();
        private readonly TreeView _arbol = new TreeView { Dock = DockStyle.Fill, HideSelection = false };
        private readonly RichTextBox _contenido = new RichTextBox
        {
            Dock = DockStyle.Fill, ReadOnly = true, BackColor = SystemColors.Window,
            BorderStyle = BorderStyle.None, DetectUrls = false
        };
        private string _pantalla;
        private bool _arbolInicializado;

        public FrmAyuda_83KI(string pantalla)
        {
            _pantalla = pantalla;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(960, 620);
            MinimumSize = new Size(700, 420);
            Font = new Font("Segoe UI", 10F);
            var divisor = new SplitContainer { Dock = DockStyle.Fill, Size = ClientSize, SplitterDistance = 310 };
            divisor.Panel1.Controls.Add(_arbol);
            divisor.Panel2.Padding = new Padding(18);
            divisor.Panel2.Controls.Add(_contenido);
            Controls.Add(divisor);
            _arbol.AfterSelect += (s, e) =>
            {
                Mostrar(e.Node.Tag as TemaAyuda_83KI);
                e.Node.Expand();
            };
            Activated += (s, e) => ActualizarIdioma(_idioma.IdiomaActual);
            _idioma.Suscribir(this);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmPrincipal.Ayuda");
            _arbol.AccessibleName = IdiomaUiHelper_83KI.Texto("Ayuda.Temas");
            _contenido.AccessibleName = Text;
            _arbol.BeginUpdate();
            try
            {
                var expandidos = new HashSet<string>(Nodos(_arbol.Nodes).Where(n => n.IsExpanded).Select(n => n.Name));
                _arbol.Nodes.Clear();
                TreeNode seleccionado = null;
                var temas = CatalogoAyuda_83KI.Disponibles().ToArray();
                var nodos = temas.ToDictionary(t => t.Pantalla, t => new TreeNode(IdiomaUiHelper_83KI.Texto(t.Titulo))
                {
                    Name = t.Pantalla, Tag = t
                });
                foreach (var grupo in temas.GroupBy(t => t.Grupo))
                {
                    var raiz = _arbol.Nodes.Add(grupo.Key, IdiomaUiHelper_83KI.Texto(grupo.Key));
                    foreach (var tema in grupo)
                    {
                        var nodo = nodos[tema.Pantalla];
                        string contenedor = CatalogoAyuda_83KI.ContenedorDe(tema.Pantalla);
                        TreeNode padre;
                        if (contenedor != null && nodos.TryGetValue(contenedor, out padre)) padre.Nodes.Add(nodo);
                        else raiz.Nodes.Add(nodo);
                        if (tema.Pantalla == _pantalla) seleccionado = nodo;
                    }
                }
                foreach (var nodo in Nodos(_arbol.Nodes))
                    if ((!_arbolInicializado && nodo.Parent == null) || expandidos.Contains(nodo.Name)) nodo.Expand();
                _arbolInicializado = true;
                if (seleccionado != null)
                {
                    for (var padre = seleccionado.Parent; padre != null; padre = padre.Parent) padre.Expand();
                    _arbol.SelectedNode = seleccionado;
                    seleccionado.Expand();
                    seleccionado.EnsureVisible();
                    Mostrar(seleccionado.Tag as TemaAyuda_83KI);
                }
                else Mostrar(null);
            }
            finally { _arbol.EndUpdate(); }
        }

        private static IEnumerable<TreeNode> Nodos(TreeNodeCollection nodos)
        {
            foreach (TreeNode nodo in nodos)
            {
                yield return nodo;
                foreach (var hijo in Nodos(nodo.Nodes)) yield return hijo;
            }
        }

        private void Mostrar(TemaAyuda_83KI tema)
        {
            if (tema == null || !tema.Disponible())
            {
                _contenido.Text = IdiomaUiHelper_83KI.Texto("Ayuda.Seleccionar");
                return;
            }
            _pantalla = tema.Pantalla;
            _contenido.Text = IdiomaUiHelper_83KI.Texto(tema.Titulo) + "\n\n"
                + IdiomaUiHelper_83KI.Texto("Ayuda." + tema.Pantalla);
            _contenido.SelectionStart = 0;
            _contenido.SelectionLength = 0;
            _contenido.ScrollToCaret();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _idioma.Desuscribir(this);
            base.Dispose(disposing);
        }
    }
}

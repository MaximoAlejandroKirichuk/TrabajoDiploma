using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Service;
using Service.Entidades;
using UI;

namespace AyudaTests
{
    public sealed class FrmCrearCurso_83KI : FormConAyuda_83KI
    {
        public readonly Button Original = new Button
        {
            Bounds = new Rectangle(110, 220, 120, 30),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        public FrmCrearCurso_83KI()
        {
            ClientSize = new Size(400, 300);
            Controls.Add(Original);
        }
        public bool F1()
        {
            var message = new Message();
            return base.ProcessCmdKey(ref message, Keys.F1);
        }
    }

    internal static class AyudaUiTests
    {
        private static readonly Assembly Ui = typeof(FormConAyuda_83KI).Assembly;
        private static readonly Type Catalog = Ui.GetType("UI.CatalogoAyuda_83KI", true);
        private static readonly Type Help = Ui.GetType("UI.FrmAyuda_83KI", true);
        private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly SessionManager_83KI Session = SessionManager_83KI.Instancia;

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
        }

        private static object Topic(string name)
        {
            return Catalog.GetMethod("Buscar").Invoke(null, new object[] { name });
        }

        private static bool Available(string name)
        {
            var topic = Topic(name);
            return (bool)topic.GetType().GetMethod("Disponible").Invoke(topic, null);
        }

        private static void SignIn(params PermisoSistema_83KI[] permissions)
        {
            Session.CerrarSesion();
            var role = new Rol_83KI(900, "Help test role");
            foreach (var permission in permissions)
                role.AgregarPatente(new Patente_83KI((int)permission, permission.ToString()));
            Session.IniciarSesion(Usuario_83KI.CrearNuevo("Help", "Test", 99000001, "help@example.test", role, "test-hash"));
        }

        private static Form CreateHelp(string name)
        {
            return (Form)Activator.CreateInstance(Help, new object[] { name });
        }

        private static TreeView Tree(Form help) { return (TreeView)Help.GetField("_arbol", Private).GetValue(help); }
        private static RichTextBox Content(Form help) { return (RichTextBox)Help.GetField("_contenido", Private).GetValue(help); }
        private static string Screen(TreeNode node)
        {
            return (string)node.Tag.GetType().GetProperty("Pantalla").GetValue(node.Tag, null);
        }
        private static int ObserverCount()
        {
            return ((ICollection)typeof(GestorIdioma_83KI).GetField("_observadores", Private)
                .GetValue(ServiceFactory_83KI.GetGestorIdioma())).Count;
        }

        [STAThread]
        public static int Main()
        {
            try
            {
                Application.EnableVisualStyles();
                var language = ServiceFactory_83KI.GetGestorIdioma();
                var all = (PermisoSistema_83KI[])Enum.GetValues(typeof(PermisoSistema_83KI));
                SignIn(all);
                var topics = Ui.GetTypes().Where(t => t.IsSubclassOf(typeof(FormConAyuda_83KI)))
                    .Select(t => Topic(t.Name)).ToArray();
                Check(topics.All(t => t != null), "An application form has no help topic");
                foreach (string id in new[] { "es-AR", "en-US" })
                {
                    language.CambiarIdioma(id);
                    foreach (var topic in topics)
                    {
                        string name = (string)topic.GetType().GetProperty("Pantalla").GetValue(topic, null);
                        foreach (string key in new[] {
                            "Ayuda." + name,
                            (string)topic.GetType().GetProperty("Titulo").GetValue(topic, null),
                            (string)topic.GetType().GetProperty("Grupo").GetValue(topic, null) })
                            Check(language.ObtenerTexto(key) != key, "Missing " + id + " translation: " + key);
                    }
                }
                foreach (var type in Ui.GetTypes().Where(t => t.IsSubclassOf(typeof(FormConAyuda_83KI))))
                    Check(Topic(type.Name) != null, "Missing help for " + type.FullName);
                using (var help = CreateHelp("FrmPrincipal"))
                {
                    help.Show();
                    Application.DoEvents();
                    var tree = Tree(help);
                    foreach (var pair in new[] {
                        new[] { "FrmCrearUsuario", "FrmGestionUsuarios" },
                        new[] { "FrmModificarUsuario", "FrmGestionUsuarios" },
                        new[] { "FrmConfirmarEstadoUsuario", "FrmGestionUsuarios" },
                        new[] { "FrmCrearProfesor_83KI", "FrmGestionProfesores_83KI" },
                        new[] { "FrmModificarProfesor_83KI", "FrmGestionProfesores_83KI" },
                        new[] { "FrmCrearCurso_83KI", "FrmGestionCursos_83KI" },
                        new[] { "FrmModificarCurso_83KI", "FrmGestionCursos_83KI" },
                        new[] { "FrmEditarComision_83KI", "FrmGestionComisiones_83KI" },
                        new[] { "FrmDetalleMorosidad_83KI", "FrmReporteMorosidad_83KI" } })
                    {
                        var child = tree.Nodes.Find(pair[0], true).Single();
                        Check(Screen(child.Parent) == pair[1], "Incorrect help parent for " + pair[0]);
                    }
                    var users = tree.Nodes.Find("FrmGestionUsuarios", true).Single();
                    var courses = tree.Nodes.Find("FrmGestionCursos_83KI", true).Single();
                    Check(users.Parent.IsExpanded && !users.IsExpanded && !courses.IsExpanded,
                        "Management branches should start collapsed under expanded modules");
                    tree.SelectedNode = users;
                    Check(users.IsExpanded, "Selecting user management did not expand its actions");
                    Check(Content(help).Text.Contains(language.ObtenerTexto("FrmGestionUsuarios.Titulo")),
                        "Management selection did not display its help");
                    tree.SelectedNode = users.Nodes.Find("FrmCrearUsuario", false).Single();
                    language.CambiarIdioma("es-AR");
                    Check(Screen(tree.SelectedNode) == "FrmCrearUsuario" && Screen(tree.SelectedNode.Parent) == "FrmGestionUsuarios",
                        "Language change lost the nested selection");
                    Check(tree.SelectedNode.Parent.IsExpanded, "Context path collapsed after language change");
                    Check(!tree.Nodes.Find("FrmGestionCursos_83KI", true).Single().IsExpanded,
                        "Language change expanded an unrelated management branch");
                    help.Close();
                }
                Session.CerrarSesion();
                foreach (string name in new[] { "Login", "FrmConfiguracionBD_83KI" })
                {
                    Check(Available(name), "Public help unavailable: " + name);
                    Check(language.ObtenerTexto("Ayuda." + name) != "Ayuda." + name, "Missing public content");
                }
                Check(!Available("FrmGestionUsuarios"), "Anonymous access to protected help");
                SignIn(PermisoSistema_83KI.VerCursos);
                Check(!Available("FrmGestionCursos_83KI"), "Help permission was not enforced");
                SignIn(PermisoSistema_83KI.Ayuda, PermisoSistema_83KI.VerCursos);
                Check(Available("FrmGestionCursos_83KI"), "Read-only course help unavailable");
                Check(!Available("FrmCrearCurso_83KI"), "Read-only role can see create help");
                using (var help = CreateHelp("FrmGestionCursos_83KI"))
                    Check(Tree(help).Nodes.Find("FrmCrearCurso_83KI", true).Length == 0,
                        "Read-only management branch exposes a restricted child");
                Check(!Available("FrmGestionUsuarios"), "Course role can see user management");
                SignIn(PermisoSistema_83KI.Ayuda, PermisoSistema_83KI.CrearCurso);
                Check(!Available("FrmCrearCurso_83KI"), "Unreachable secondary form is listed");
                SignIn(PermisoSistema_83KI.Ayuda, PermisoSistema_83KI.GestionUsuarios);
                Check(!Available("FrmGestionUsuarios"), "Admin container permission was not enforced");
                SignIn(PermisoSistema_83KI.Ayuda, PermisoSistema_83KI.VerCursos, PermisoSistema_83KI.CrearCurso);
                int observers = ObserverCount();
                using (var help = CreateHelp("FrmCrearCurso_83KI"))
                {
                    var tree = Tree(help);
                    Check(Screen(tree.SelectedNode) == "FrmCrearCurso_83KI", "Context topic was not selected");
                    Check(Screen(tree.SelectedNode.Parent) == "FrmGestionCursos_83KI" && tree.SelectedNode.Parent.IsExpanded,
                        "Contextual access did not expand the nested topic path");
                    Check(tree.Nodes.Count == 2, "Empty or restricted modules are visible");
                    Check(!tree.Nodes.Cast<TreeNode>().SelectMany(n => n.Nodes.Cast<TreeNode>())
                        .Any(n => Screen(n) == "FrmGestionUsuarios"), "Restricted node found");
                    language.CambiarIdioma("es-AR");
                    Check(Content(help).Text.Contains("PROPÓSITO"), "Spanish content missing");
                    language.CambiarIdioma("en-US");
                    Check(Screen(tree.SelectedNode) == "FrmCrearCurso_83KI", "Language change lost selection");
                    Check(Content(help).Text.Contains("PURPOSE"), "English content missing");
                    Help.GetMethod("Mostrar", Private).Invoke(help, new[] { Topic("FrmGestionUsuarios") });
                    Check(Content(help).Text == language.ObtenerTexto("Ayuda.Seleccionar"), "Direct access bypasses permissions");
                    Session.EstablecerRecuperacionIntegridad(new[] { "Usuario" });
                    ((Service.Interfaces.IObservadorIdioma)help).ActualizarIdioma(language.IdiomaActual);
                    Check(tree.Nodes.Count == 0, "Recovery still exposes help topics");
                    Session.LimpiarRecuperacionIntegridad();
                    Session.ReinicioRequeridoDespuesDeRestore = true;
                    Check(!Available("FrmCrearCurso_83KI"), "Restore restart did not block help");
                    Session.ReinicioRequeridoDespuesDeRestore = false;
                }
                Check(ObserverCount() == observers, "Help window leaked its language observer");

                using (var screen = new FrmCrearCurso_83KI())
                {
                    screen.Show();
                    Application.DoEvents();
                    Check(screen.Original.Bounds == new Rectangle(110, 220, 120, 30), "Original control bounds changed");
                    Check(screen.Original.Parent.ClientSize == new Size(400, 300), "Original client area changed");
                    var bar = screen.Controls.OfType<ToolStrip>().Single();
                    Check(bar.Bottom == screen.ClientSize.Height, "Help bar is not at the bottom");
                    screen.ClientSize = new Size(screen.ClientSize.Width + 100, screen.ClientSize.Height + 80);
                    Check(screen.Original.Bounds == new Rectangle(210, 300, 120, 30), "Resizing broke existing anchors");
                    language.CambiarIdioma("es-AR");
                    Check(bar.Items[0].Text == "Ayuda (F1)", "Help button not localized");
                    language.CambiarIdioma("en-US");
                    Check(bar.Items[0].Text == "Help (F1)", "English button not localized");
                    int opened = 0;
                    using (var timer = new Timer { Interval = 40 })
                    {
                        timer.Tick += (s, e) =>
                        {
                            var help = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.GetType() == Help);
                            if (help == null) return;
                            Check(Screen(Tree(help).SelectedNode) == "FrmCrearCurso_83KI", "Shortcut opened wrong topic");
                            opened++;
                            help.Close();
                        };
                        timer.Start();
                        ((ToolStripButton)bar.Items[0]).PerformClick();
                        Check(screen.F1(), "F1 was not handled");
                        timer.Stop();
                    }
                    Check(opened == 2, "Button and F1 did not both open contextual help");
                    Session.CerrarSesion();
                    Check(screen.F1(), "Blocked F1 was not handled");
                    Check(!Application.OpenForms.Cast<Form>().Any(f => f.GetType() == Help), "F1 bypasses logout");
                    screen.Close();
                }
                Check(ObserverCount() == observers, "Context access leaked its language observer");
                Console.WriteLine("PASS: hierarchy, branch navigation, nested selection, translations, role filtering, direct access, recovery, layout, Help/F1 and observer cleanup.");
                return 0;
            }
            catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
            finally { Session.CerrarSesion(); }
        }
    }
}

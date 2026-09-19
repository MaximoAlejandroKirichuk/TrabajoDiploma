using BE.Entidades;
using Service;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    public partial class FrmRegistrarPreaperturaComision_83KI : Form, IObservadorIdioma
    {
        private readonly IGestorComision_83KI _gestor;
        private readonly IGestorIdioma_83KI _gestorIdioma;
        private bool _actualizandoProfesores;

        public FrmRegistrarPreaperturaComision_83KI(IGestorComision_83KI gestor)
        {
            _gestor = gestor;
            _gestorIdioma = ServiceFactory_83KI.GetGestorIdioma();
            InitializeComponent();
            InicializarValoresPredeterminados();
            CargarCursos();
            ConfigurarEventos();
            _gestorIdioma.Suscribir(this);
            ActualizarProfesoresDisponibles();
        }

        private void InicializarValoresPredeterminados()
        {
            CargarDias();
            dtpHoraInicio.Value = DateTime.Today.AddHours(18);
            dtpHoraFin.Value = DateTime.Today.AddHours(20);
            dtpFechaLimitePago.MinDate = DateTime.Today;
        }

        private void CargarDias()
        {
            DayOfWeek seleccionado = DiaSeleccionado;
            cmbDia.Items.Clear();
            foreach (DayOfWeek dia in Enum.GetValues(typeof(DayOfWeek)))
            {
                cmbDia.Items.Add(new ComboItemIdioma_83KI(dia, IdiomaUiHelper_83KI.Texto("Dominio.DiaSemana." + dia)));
            }

            cmbDia.SelectedIndex = (int)seleccionado;
        }

        private void CargarCursos()
        {
            cmbCursos.DataSource = _gestor.ObtenerCursosActivosParaPreapertura().ToList();
            cmbCursos.DisplayMember = "Nombre";
            cmbCursos.ValueMember = "IdCurso";
        }

        private void ConfigurarEventos()
        {
            cmbCursos.SelectedIndexChanged += (s, e) => ActualizarProfesoresDisponibles();
            cmbDia.SelectedIndexChanged += (s, e) => ActualizarProfesoresDisponibles();
            dtpHoraInicio.ValueChanged += (s, e) => ActualizarProfesoresDisponibles();
            dtpHoraFin.ValueChanged += (s, e) => ActualizarProfesoresDisponibles();
            cmbProfesores.SelectedIndexChanged += (s, e) => ActualizarEstadoRegistrar();
        }

        private void ActualizarProfesoresDisponibles()
        {
            if (_actualizandoProfesores)
            {
                return;
            }

            _actualizandoProfesores = true;
            try
            {
                lblMensaje.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.ActualizandoProfesores");

                if (IdCursoSeleccionado <= 0)
                {
                    LimpiarProfesores(IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.SeleccioneCurso"));
                    return;
                }

                if (HoraInicio >= HoraFin)
                {
                    LimpiarProfesores(IdiomaUiHelper_83KI.Texto("Errores.HorarioInvalido"));
                    return;
                }

                var profesores = _gestor.ObtenerProfesoresDisponibles(IdCursoSeleccionado, DiaSeleccionado, HoraInicio, HoraFin).ToList();

                if (profesores.Count == 0)
                {
                    LimpiarProfesores(IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.SinProfesoresDisponibles"));
                    return;
                }

                cmbProfesores.DataSource = profesores;
                cmbProfesores.DisplayMember = "NombreCompleto";
                cmbProfesores.ValueMember = "IdProfesor";
                cmbProfesores.Enabled = true;
                lblMensaje.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Durante la actualizacion automatica no interrumpimos con MessageBox:
                // mostramos el estado en linea y reservamos los popups para el envio.
                LimpiarProfesores(IdiomaUiHelper_83KI.TraducirExcepcion(ex));
            }
            finally
            {
                _actualizandoProfesores = false;
                ActualizarEstadoRegistrar();
            }
        }

        private void LimpiarProfesores(string mensaje)
        {
            cmbProfesores.DataSource = null;
            cmbProfesores.Enabled = false;
            lblMensaje.Text = mensaje;
            ActualizarEstadoRegistrar();
        }

        private void ActualizarEstadoRegistrar()
        {
            btnRegistrar.Enabled = IdProfesorSeleccionado > 0;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (IdCursoSeleccionado <= 0)
                {
                    MostrarValidacion("Errores.CursoObligatorio");
                    return;
                }

                if (IdProfesorSeleccionado <= 0)
                {
                    MostrarValidacion("Errores.ProfesorObligatorio");
                    return;
                }

                if (HoraInicio >= HoraFin)
                {
                    MostrarValidacion("Errores.HorarioInvalido");
                    return;
                }

                string codigo = _gestor.RegistrarPreapertura(IdCursoSeleccionado, IdProfesorSeleccionado, DiaSeleccionado, HoraInicio, HoraFin, (int)nudCupoMinimo.Value, (int)nudCupoMaximo.Value, dtpFechaLimitePago.Value.Date);

                MessageBox.Show(this, IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.Registrada", codigo), IdiomaUiHelper_83KI.Texto("Comun.Informacion"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private int IdCursoSeleccionado { get { return cmbCursos.SelectedValue is int ? (int)cmbCursos.SelectedValue : 0; } }
        private int IdProfesorSeleccionado { get { return cmbProfesores.SelectedValue is int ? (int)cmbProfesores.SelectedValue : 0; } }

        private DayOfWeek DiaSeleccionado
        {
            get
            {
                ComboItemIdioma_83KI item = cmbDia.SelectedItem as ComboItemIdioma_83KI;
                return item != null && item.Valor is DayOfWeek ? (DayOfWeek)item.Valor : DayOfWeek.Monday;
            }
        }

        private TimeSpan HoraInicio { get { return dtpHoraInicio.Value.TimeOfDay; } }
        private TimeSpan HoraFin { get { return dtpHoraFin.Value.TimeOfDay; } }

        private void MostrarValidacion(string clave)
        {
            lblMensaje.Text = IdiomaUiHelper_83KI.Texto(clave);
        }

        private void MostrarError(Exception ex)
        {
            lblMensaje.Text = IdiomaUiHelper_83KI.TraducirExcepcion(ex);
            IdiomaUiHelper_83KI.MostrarError(this, ex, "Comun.Validacion", MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma(IIdioma idioma)
        {
            Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.Titulo");
            lblCurso.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.Curso");
            lblDia.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.Dia");
            lblHoraInicio.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.HoraInicio");
            lblHoraFin.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.HoraFin");
            lblCupoMinimo.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.CupoMinimo");
            lblCupoMaximo.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.CupoMaximo");
            lblFechaLimitePago.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.FechaLimitePago");
            lblProfesor.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.Profesor");
            btnRegistrar.Text = IdiomaUiHelper_83KI.Texto("FrmRegistrarPreaperturaComision.Registrar");
            CargarDias();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gestorIdioma.Desuscribir(this);
            base.OnFormClosed(e);
        }
    }
}

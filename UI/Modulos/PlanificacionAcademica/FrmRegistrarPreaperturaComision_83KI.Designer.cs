using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    partial class FrmRegistrarPreaperturaComision_83KI
    {
        private System.ComponentModel.IContainer components = null;
        private FlowLayoutPanel flpContenido;
        private TableLayoutPanel pnlDatosComision;
        private Panel pnlArancel;
        private TableLayoutPanel tblArancel;
        private Panel pnlPlanes;
        private TableLayoutPanel tblPlanes;
        private Label lblCurso;
        private Label lblDia;
        private Label lblHoraInicio;
        private Label lblHoraFin;
        private Label lblCupoMinimo;
        private Label lblCupoMaximo;
        private Label lblFechaLimitePago;
        private Label lblProfesor;
        private Label lblArancelBase;
        private Label lblPlanes;
        private ComboBox cmbCursos;
        private ComboBox cmbDia;
        private DateTimePicker dtpHoraInicio;
        private DateTimePicker dtpHoraFin;
        private NumericUpDown nudCupoMinimo;
        private NumericUpDown nudCupoMaximo;
        private DateTimePicker dtpFechaLimitePago;
        private ComboBox cmbProfesores;
        private NumericUpDown nudArancelBase;
        private CheckedListBox clbPlanes;
        private FlowLayoutPanel pnlAcciones;
        private Button btnRegistrar;
        private Label lblMensaje;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flpContenido = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlDatosComision = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurso = new System.Windows.Forms.Label();
            this.lblDia = new System.Windows.Forms.Label();
            this.lblHoraInicio = new System.Windows.Forms.Label();
            this.lblHoraFin = new System.Windows.Forms.Label();
            this.lblCupoMinimo = new System.Windows.Forms.Label();
            this.lblCupoMaximo = new System.Windows.Forms.Label();
            this.lblFechaLimitePago = new System.Windows.Forms.Label();
            this.lblProfesor = new System.Windows.Forms.Label();
            this.cmbCursos = new System.Windows.Forms.ComboBox();
            this.cmbDia = new System.Windows.Forms.ComboBox();
            this.dtpHoraInicio = new System.Windows.Forms.DateTimePicker();
            this.dtpHoraFin = new System.Windows.Forms.DateTimePicker();
            this.nudCupoMinimo = new System.Windows.Forms.NumericUpDown();
            this.nudCupoMaximo = new System.Windows.Forms.NumericUpDown();
            this.dtpFechaLimitePago = new System.Windows.Forms.DateTimePicker();
            this.cmbProfesores = new System.Windows.Forms.ComboBox();
            this.pnlArancel = new System.Windows.Forms.Panel();
            this.tblArancel = new System.Windows.Forms.TableLayoutPanel();
            this.lblArancelBase = new System.Windows.Forms.Label();
            this.nudArancelBase = new System.Windows.Forms.NumericUpDown();
            this.pnlPlanes = new System.Windows.Forms.Panel();
            this.tblPlanes = new System.Windows.Forms.TableLayoutPanel();
            this.lblPlanes = new System.Windows.Forms.Label();
            this.clbPlanes = new System.Windows.Forms.CheckedListBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.pnlAcciones = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.flpContenido.SuspendLayout();
            this.pnlDatosComision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMinimo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMaximo)).BeginInit();
            this.pnlArancel.SuspendLayout();
            this.tblArancel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudArancelBase)).BeginInit();
            this.pnlPlanes.SuspendLayout();
            this.tblPlanes.SuspendLayout();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpContenido
            // 
            this.flpContenido.AutoScroll = true;
            this.flpContenido.Controls.Add(this.pnlDatosComision);
            this.flpContenido.Controls.Add(this.pnlArancel);
            this.flpContenido.Controls.Add(this.pnlPlanes);
            this.flpContenido.Controls.Add(this.lblMensaje);
            this.flpContenido.Controls.Add(this.pnlAcciones);
            this.flpContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContenido.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpContenido.Location = new System.Drawing.Point(0, 0);
            this.flpContenido.Name = "flpContenido";
            this.flpContenido.Padding = new System.Windows.Forms.Padding(16);
            this.flpContenido.Size = new System.Drawing.Size(704, 500);
            this.flpContenido.TabIndex = 0;
            this.flpContenido.WrapContents = false;
            // 
            // pnlDatosComision
            // 
            this.pnlDatosComision.ColumnCount = 2;
            this.pnlDatosComision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.pnlDatosComision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlDatosComision.Controls.Add(this.lblCurso, 0, 0);
            this.pnlDatosComision.Controls.Add(this.lblDia, 0, 1);
            this.pnlDatosComision.Controls.Add(this.lblHoraInicio, 0, 2);
            this.pnlDatosComision.Controls.Add(this.lblHoraFin, 0, 3);
            this.pnlDatosComision.Controls.Add(this.lblCupoMinimo, 0, 4);
            this.pnlDatosComision.Controls.Add(this.lblCupoMaximo, 0, 5);
            this.pnlDatosComision.Controls.Add(this.lblFechaLimitePago, 0, 6);
            this.pnlDatosComision.Controls.Add(this.lblProfesor, 0, 7);
            this.pnlDatosComision.Controls.Add(this.cmbCursos, 1, 0);
            this.pnlDatosComision.Controls.Add(this.cmbDia, 1, 1);
            this.pnlDatosComision.Controls.Add(this.dtpHoraInicio, 1, 2);
            this.pnlDatosComision.Controls.Add(this.dtpHoraFin, 1, 3);
            this.pnlDatosComision.Controls.Add(this.nudCupoMinimo, 1, 4);
            this.pnlDatosComision.Controls.Add(this.nudCupoMaximo, 1, 5);
            this.pnlDatosComision.Controls.Add(this.dtpFechaLimitePago, 1, 6);
            this.pnlDatosComision.Controls.Add(this.cmbProfesores, 1, 7);
            this.pnlDatosComision.Location = new System.Drawing.Point(19, 19);
            this.pnlDatosComision.Name = "pnlDatosComision";
            this.pnlDatosComision.RowCount = 8;
            for (int i = 0; i < 8; i++) this.pnlDatosComision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.pnlDatosComision.Size = new System.Drawing.Size(650, 304);
            this.pnlDatosComision.TabIndex = 0;
            // 
            // labels and scheduling controls
            // 
            this.lblCurso.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblCurso.AutoSize = true; this.lblCurso.Text = "Curso";
            this.lblDia.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblDia.AutoSize = true; this.lblDia.Text = "Día";
            this.lblHoraInicio.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblHoraInicio.AutoSize = true; this.lblHoraInicio.Text = "Hora inicio";
            this.lblHoraFin.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblHoraFin.AutoSize = true; this.lblHoraFin.Text = "Hora fin";
            this.lblCupoMinimo.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblCupoMinimo.AutoSize = true; this.lblCupoMinimo.Text = "Cupo mínimo";
            this.lblCupoMaximo.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblCupoMaximo.AutoSize = true; this.lblCupoMaximo.Text = "Cupo máximo";
            this.lblFechaLimitePago.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblFechaLimitePago.AutoSize = true; this.lblFechaLimitePago.Text = "Fecha límite de pago";
            this.lblProfesor.Anchor = System.Windows.Forms.AnchorStyles.Left; this.lblProfesor.AutoSize = true; this.lblProfesor.Text = "Profesor";
            this.cmbCursos.Dock = System.Windows.Forms.DockStyle.Fill; this.cmbCursos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbCursos.FormattingEnabled = true;
            this.cmbDia.Dock = System.Windows.Forms.DockStyle.Fill; this.cmbDia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbDia.FormattingEnabled = true;
            this.dtpHoraInicio.Dock = System.Windows.Forms.DockStyle.Left; this.dtpHoraInicio.Format = System.Windows.Forms.DateTimePickerFormat.Time; this.dtpHoraInicio.ShowUpDown = true; this.dtpHoraInicio.Size = new System.Drawing.Size(200, 25);
            this.dtpHoraFin.Dock = System.Windows.Forms.DockStyle.Left; this.dtpHoraFin.Format = System.Windows.Forms.DateTimePickerFormat.Time; this.dtpHoraFin.ShowUpDown = true; this.dtpHoraFin.Size = new System.Drawing.Size(200, 25);
            this.nudCupoMinimo.Dock = System.Windows.Forms.DockStyle.Left; this.nudCupoMinimo.Maximum = new decimal(new int[] { 999, 0, 0, 0 }); this.nudCupoMinimo.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCupoMinimo.Size = new System.Drawing.Size(120, 25); this.nudCupoMinimo.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCupoMaximo.Dock = System.Windows.Forms.DockStyle.Left; this.nudCupoMaximo.Maximum = new decimal(new int[] { 999, 0, 0, 0 }); this.nudCupoMaximo.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCupoMaximo.Size = new System.Drawing.Size(120, 25); this.nudCupoMaximo.Value = new decimal(new int[] { 30, 0, 0, 0 });
            this.dtpFechaLimitePago.Dock = System.Windows.Forms.DockStyle.Left; this.dtpFechaLimitePago.Format = System.Windows.Forms.DateTimePickerFormat.Short; this.dtpFechaLimitePago.Size = new System.Drawing.Size(200, 25);
            this.cmbProfesores.Dock = System.Windows.Forms.DockStyle.Fill; this.cmbProfesores.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbProfesores.FormattingEnabled = true;
            // 
            // pnlArancel
            // 
            this.pnlArancel.Controls.Add(this.tblArancel);
            this.pnlArancel.Location = new System.Drawing.Point(19, 329);
            this.pnlArancel.Name = "pnlArancel";
            this.pnlArancel.Size = new System.Drawing.Size(650, 44);
            this.pnlArancel.TabIndex = 1;
            this.tblArancel.ColumnCount = 2;
            this.tblArancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.tblArancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblArancel.Controls.Add(this.lblArancelBase, 0, 0);
            this.tblArancel.Controls.Add(this.nudArancelBase, 1, 0);
            this.tblArancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblArancel.RowCount = 1;
            this.tblArancel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lblArancelBase.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblArancelBase.AutoSize = true;
            this.lblArancelBase.Text = "Base tuition";
            this.nudArancelBase.DecimalPlaces = 2;
            this.nudArancelBase.Dock = System.Windows.Forms.DockStyle.Left;
            this.nudArancelBase.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudArancelBase.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            this.nudArancelBase.Name = "nudArancelBase";
            this.nudArancelBase.Size = new System.Drawing.Size(160, 25);
            // 
            // pnlPlanes
            // 
            this.pnlPlanes.Controls.Add(this.tblPlanes);
            this.pnlPlanes.Location = new System.Drawing.Point(19, 379);
            this.pnlPlanes.Name = "pnlPlanes";
            this.pnlPlanes.Size = new System.Drawing.Size(650, 110);
            this.pnlPlanes.TabIndex = 2;
            this.pnlPlanes.Visible = false;
            this.tblPlanes.ColumnCount = 2;
            this.tblPlanes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.tblPlanes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblPlanes.Controls.Add(this.lblPlanes, 0, 0);
            this.tblPlanes.Controls.Add(this.clbPlanes, 1, 0);
            this.tblPlanes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblPlanes.RowCount = 1;
            this.tblPlanes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lblPlanes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPlanes.AutoSize = true;
            this.lblPlanes.Text = "Payment plan";
            this.clbPlanes.CheckOnClick = true;
            this.clbPlanes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbPlanes.FormattingEnabled = true;
            this.clbPlanes.Name = "clbPlanes";
            // 
            // lblMensaje
            // 
            this.lblMensaje.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblMensaje.Location = new System.Drawing.Point(19, 492);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(650, 42);
            this.lblMensaje.TabIndex = 3;
            // 
            // pnlAcciones
            // 
            this.pnlAcciones.AutoSize = true;
            this.pnlAcciones.Controls.Add(this.btnRegistrar);
            this.pnlAcciones.Location = new System.Drawing.Point(19, 537);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(650, 35);
            this.pnlAcciones.TabIndex = 4;
            this.btnRegistrar.AutoSize = true;
            this.btnRegistrar.Enabled = false;
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(149, 29);
            this.btnRegistrar.Text = "Registrar preapertura";
            this.btnRegistrar.UseVisualStyleBackColor = true;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            // 
            // FrmRegistrarPreaperturaComision_83KI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 500);
            this.Controls.Add(this.flpContenido);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "FrmRegistrarPreaperturaComision_83KI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registro de preapertura de comisión";
            this.flpContenido.ResumeLayout(false);
            this.flpContenido.PerformLayout();
            this.pnlDatosComision.ResumeLayout(false);
            this.pnlDatosComision.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMinimo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMaximo)).EndInit();
            this.pnlArancel.ResumeLayout(false);
            this.tblArancel.ResumeLayout(false);
            this.tblArancel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudArancelBase)).EndInit();
            this.pnlPlanes.ResumeLayout(false);
            this.tblPlanes.ResumeLayout(false);
            this.tblPlanes.PerformLayout();
            this.pnlAcciones.ResumeLayout(false);
            this.pnlAcciones.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}

using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.PlanificacionAcademica
{
    partial class FrmRegistrarPreaperturaComision_83KI
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel panelContenido;
        private Label lblCurso;
        private Label lblDia;
        private Label lblHoraInicio;
        private Label lblHoraFin;
        private Label lblCupoMinimo;
        private Label lblCupoMaximo;
        private Label lblFechaLimitePago;
        private Label lblProfesor;
        private ComboBox cmbCursos;
        private ComboBox cmbDia;
        private DateTimePicker dtpHoraInicio;
        private DateTimePicker dtpHoraFin;
        private NumericUpDown nudCupoMinimo;
        private NumericUpDown nudCupoMaximo;
        private DateTimePicker dtpFechaLimitePago;
        private ComboBox cmbProfesores;
        private FlowLayoutPanel pnlAcciones;
        private Button btnRegistrar;
        private Label lblMensaje;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelContenido = new System.Windows.Forms.TableLayoutPanel();
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
            this.lblMensaje = new System.Windows.Forms.Label();
            this.pnlAcciones = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.panelContenido.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMinimo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMaximo)).BeginInit();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContenido
            // 
            this.panelContenido.ColumnCount = 2;
            this.panelContenido.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.panelContenido.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelContenido.Controls.Add(this.lblCurso, 0, 0);
            this.panelContenido.Controls.Add(this.lblDia, 0, 1);
            this.panelContenido.Controls.Add(this.lblHoraInicio, 0, 2);
            this.panelContenido.Controls.Add(this.lblHoraFin, 0, 3);
            this.panelContenido.Controls.Add(this.lblCupoMinimo, 0, 4);
            this.panelContenido.Controls.Add(this.lblCupoMaximo, 0, 5);
            this.panelContenido.Controls.Add(this.lblFechaLimitePago, 0, 6);
            this.panelContenido.Controls.Add(this.lblProfesor, 0, 7);
            this.panelContenido.Controls.Add(this.cmbCursos, 1, 0);
            this.panelContenido.Controls.Add(this.cmbDia, 1, 1);
            this.panelContenido.Controls.Add(this.dtpHoraInicio, 1, 2);
            this.panelContenido.Controls.Add(this.dtpHoraFin, 1, 3);
            this.panelContenido.Controls.Add(this.nudCupoMinimo, 1, 4);
            this.panelContenido.Controls.Add(this.nudCupoMaximo, 1, 5);
            this.panelContenido.Controls.Add(this.dtpFechaLimitePago, 1, 6);
            this.panelContenido.Controls.Add(this.cmbProfesores, 1, 7);
            this.panelContenido.Controls.Add(this.lblMensaje, 0, 8);
            this.panelContenido.Controls.Add(this.pnlAcciones, 1, 8);
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Location = new System.Drawing.Point(0, 0);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Padding = new System.Windows.Forms.Padding(16);
            this.panelContenido.RowCount = 9;
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.panelContenido.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelContenido.Size = new System.Drawing.Size(704, 391);
            this.panelContenido.TabIndex = 0;
            // 
            // lblCurso
            // 
            this.lblCurso.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurso.AutoSize = true;
            this.lblCurso.Location = new System.Drawing.Point(19, 25);
            this.lblCurso.Name = "lblCurso";
            this.lblCurso.Size = new System.Drawing.Size(45, 19);
            this.lblCurso.TabIndex = 0;
            this.lblCurso.Text = "Curso";
            // 
            // lblDia
            // 
            this.lblDia.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDia.AutoSize = true;
            this.lblDia.Location = new System.Drawing.Point(19, 63);
            this.lblDia.Name = "lblDia";
            this.lblDia.Size = new System.Drawing.Size(29, 19);
            this.lblDia.TabIndex = 1;
            this.lblDia.Text = "Día";
            // 
            // lblHoraInicio
            // 
            this.lblHoraInicio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHoraInicio.AutoSize = true;
            this.lblHoraInicio.Location = new System.Drawing.Point(19, 101);
            this.lblHoraInicio.Name = "lblHoraInicio";
            this.lblHoraInicio.Size = new System.Drawing.Size(74, 19);
            this.lblHoraInicio.TabIndex = 2;
            this.lblHoraInicio.Text = "Hora inicio";
            // 
            // lblHoraFin
            // 
            this.lblHoraFin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHoraFin.AutoSize = true;
            this.lblHoraFin.Location = new System.Drawing.Point(19, 139);
            this.lblHoraFin.Name = "lblHoraFin";
            this.lblHoraFin.Size = new System.Drawing.Size(58, 19);
            this.lblHoraFin.TabIndex = 3;
            this.lblHoraFin.Text = "Hora fin";
            // 
            // lblCupoMinimo
            // 
            this.lblCupoMinimo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCupoMinimo.AutoSize = true;
            this.lblCupoMinimo.Location = new System.Drawing.Point(19, 177);
            this.lblCupoMinimo.Name = "lblCupoMinimo";
            this.lblCupoMinimo.Size = new System.Drawing.Size(92, 19);
            this.lblCupoMinimo.TabIndex = 4;
            this.lblCupoMinimo.Text = "Cupo mínimo";
            // 
            // lblCupoMaximo
            // 
            this.lblCupoMaximo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCupoMaximo.AutoSize = true;
            this.lblCupoMaximo.Location = new System.Drawing.Point(19, 215);
            this.lblCupoMaximo.Name = "lblCupoMaximo";
            this.lblCupoMaximo.Size = new System.Drawing.Size(94, 19);
            this.lblCupoMaximo.TabIndex = 5;
            this.lblCupoMaximo.Text = "Cupo máximo";
            // 
            // lblFechaLimitePago
            // 
            this.lblFechaLimitePago.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFechaLimitePago.AutoSize = true;
            this.lblFechaLimitePago.Location = new System.Drawing.Point(19, 253);
            this.lblFechaLimitePago.Name = "lblFechaLimitePago";
            this.lblFechaLimitePago.Size = new System.Drawing.Size(135, 19);
            this.lblFechaLimitePago.TabIndex = 6;
            this.lblFechaLimitePago.Text = "Fecha límite de pago";
            // 
            // lblProfesor
            // 
            this.lblProfesor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProfesor.AutoSize = true;
            this.lblProfesor.Location = new System.Drawing.Point(19, 291);
            this.lblProfesor.Name = "lblProfesor";
            this.lblProfesor.Size = new System.Drawing.Size(60, 19);
            this.lblProfesor.TabIndex = 7;
            this.lblProfesor.Text = "Profesor";
            // 
            // cmbCursos
            // 
            this.cmbCursos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCursos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCursos.FormattingEnabled = true;
            this.cmbCursos.Location = new System.Drawing.Point(209, 19);
            this.cmbCursos.Name = "cmbCursos";
            this.cmbCursos.Size = new System.Drawing.Size(476, 25);
            this.cmbCursos.TabIndex = 8;
            // 
            // cmbDia
            // 
            this.cmbDia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDia.FormattingEnabled = true;
            this.cmbDia.Location = new System.Drawing.Point(209, 57);
            this.cmbDia.Name = "cmbDia";
            this.cmbDia.Size = new System.Drawing.Size(476, 25);
            this.cmbDia.TabIndex = 9;
            // 
            // dtpHoraInicio
            // 
            this.dtpHoraInicio.Dock = System.Windows.Forms.DockStyle.Left;
            this.dtpHoraInicio.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraInicio.Location = new System.Drawing.Point(209, 95);
            this.dtpHoraInicio.Name = "dtpHoraInicio";
            this.dtpHoraInicio.ShowUpDown = true;
            this.dtpHoraInicio.Size = new System.Drawing.Size(200, 25);
            this.dtpHoraInicio.TabIndex = 10;
            // 
            // dtpHoraFin
            // 
            this.dtpHoraFin.Dock = System.Windows.Forms.DockStyle.Left;
            this.dtpHoraFin.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraFin.Location = new System.Drawing.Point(209, 133);
            this.dtpHoraFin.Name = "dtpHoraFin";
            this.dtpHoraFin.ShowUpDown = true;
            this.dtpHoraFin.Size = new System.Drawing.Size(200, 25);
            this.dtpHoraFin.TabIndex = 11;
            // 
            // nudCupoMinimo
            // 
            this.nudCupoMinimo.Dock = System.Windows.Forms.DockStyle.Left;
            this.nudCupoMinimo.Location = new System.Drawing.Point(209, 171);
            this.nudCupoMinimo.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudCupoMinimo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCupoMinimo.Name = "nudCupoMinimo";
            this.nudCupoMinimo.Size = new System.Drawing.Size(120, 25);
            this.nudCupoMinimo.TabIndex = 12;
            this.nudCupoMinimo.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudCupoMaximo
            // 
            this.nudCupoMaximo.Dock = System.Windows.Forms.DockStyle.Left;
            this.nudCupoMaximo.Location = new System.Drawing.Point(209, 209);
            this.nudCupoMaximo.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudCupoMaximo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCupoMaximo.Name = "nudCupoMaximo";
            this.nudCupoMaximo.Size = new System.Drawing.Size(120, 25);
            this.nudCupoMaximo.TabIndex = 13;
            this.nudCupoMaximo.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // dtpFechaLimitePago
            // 
            this.dtpFechaLimitePago.Dock = System.Windows.Forms.DockStyle.Left;
            this.dtpFechaLimitePago.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaLimitePago.Location = new System.Drawing.Point(209, 247);
            this.dtpFechaLimitePago.Name = "dtpFechaLimitePago";
            this.dtpFechaLimitePago.Size = new System.Drawing.Size(200, 25);
            this.dtpFechaLimitePago.TabIndex = 14;
            // 
            // cmbProfesores
            // 
            this.cmbProfesores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbProfesores.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProfesores.FormattingEnabled = true;
            this.cmbProfesores.Location = new System.Drawing.Point(209, 285);
            this.cmbProfesores.Name = "cmbProfesores";
            this.cmbProfesores.Size = new System.Drawing.Size(476, 25);
            this.cmbProfesores.TabIndex = 15;
            // 
            // lblMensaje
            // 
            this.lblMensaje.AutoSize = true;
            this.lblMensaje.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMensaje.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblMensaje.Location = new System.Drawing.Point(19, 320);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(184, 55);
            this.lblMensaje.TabIndex = 16;
            // 
            // pnlAcciones
            // 
            this.pnlAcciones.AutoSize = true;
            this.pnlAcciones.Controls.Add(this.btnRegistrar);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAcciones.Location = new System.Drawing.Point(209, 323);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(476, 49);
            this.pnlAcciones.TabIndex = 17;
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.AutoSize = true;
            this.btnRegistrar.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRegistrar.Enabled = false;
            this.btnRegistrar.Location = new System.Drawing.Point(3, 3);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(149, 29);
            this.btnRegistrar.TabIndex = 0;
            this.btnRegistrar.Text = "Registrar preapertura";
            this.btnRegistrar.UseVisualStyleBackColor = true;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            // 
            // FrmRegistrarPreaperturaComision_83KI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 391);
            this.Controls.Add(this.panelContenido);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "FrmRegistrarPreaperturaComision_83KI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registro de preapertura de comisión";
            this.panelContenido.ResumeLayout(false);
            this.panelContenido.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMinimo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCupoMaximo)).EndInit();
            this.pnlAcciones.ResumeLayout(false);
            this.pnlAcciones.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}

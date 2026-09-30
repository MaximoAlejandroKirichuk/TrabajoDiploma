namespace UI.Modulos.PlanificacionAcademica
{
    partial class FrmRegistrarEstadoDefinitivoComision_83KI
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            this.dgvComisiones = new System.Windows.Forms.DataGridView();
            this.grpActa = new System.Windows.Forms.GroupBox();
            this.lblNumeroActa = new System.Windows.Forms.Label();
            this.txtNumeroActa = new System.Windows.Forms.TextBox();
            this.lblFechaActa = new System.Windows.Forms.Label();
            this.dtpFechaActa = new System.Windows.Forms.DateTimePicker();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnRegistrarAlta = new System.Windows.Forms.Button();
            this.btnRegistrarCierre = new System.Windows.Forms.Button();
            this.lblEvaluacion = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComisiones)).BeginInit();
            this.grpActa.SuspendLayout();
            this.SuspendLayout();
            this.dgvComisiones.AllowUserToAddRows = false; this.dgvComisiones.AllowUserToDeleteRows = false; this.dgvComisiones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; this.dgvComisiones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize; this.dgvComisiones.Location = new System.Drawing.Point(12, 12); this.dgvComisiones.MultiSelect = false; this.dgvComisiones.Name = "dgvComisiones"; this.dgvComisiones.ReadOnly = true; this.dgvComisiones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect; this.dgvComisiones.Size = new System.Drawing.Size(860, 240); this.dgvComisiones.TabIndex = 0; this.dgvComisiones.SelectionChanged += new System.EventHandler(this.dgvComisiones_SelectionChanged);
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "IdComision", HeaderText = "Id", Visible = false });
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Codigo", HeaderText = "Comisión" });
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Curso", HeaderText = "Curso" });
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Profesor", HeaderText = "Profesor" });
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "CupoMinimo", HeaderText = "Quórum" });
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "VacantesRegularizadas", HeaderText = "Regularizadas" });
            this.dgvComisiones.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Resultado", HeaderText = "Resultado" });
            this.grpActa.Controls.Add(this.lblNumeroActa); this.grpActa.Controls.Add(this.txtNumeroActa); this.grpActa.Controls.Add(this.lblFechaActa); this.grpActa.Controls.Add(this.dtpFechaActa); this.grpActa.Controls.Add(this.lblMotivo); this.grpActa.Controls.Add(this.txtMotivo); this.grpActa.Controls.Add(this.lblObservaciones); this.grpActa.Controls.Add(this.txtObservaciones); this.grpActa.Location = new System.Drawing.Point(12, 287); this.grpActa.Name = "grpActa"; this.grpActa.Size = new System.Drawing.Size(860, 150); this.grpActa.TabIndex = 1; this.grpActa.TabStop = false; this.grpActa.Text = "Datos del acta";
            this.lblNumeroActa.Location = new System.Drawing.Point(16, 28); this.lblNumeroActa.Size = new System.Drawing.Size(120, 23); this.lblNumeroActa.Text = "Número de acta";
            this.txtNumeroActa.Location = new System.Drawing.Point(150, 25); this.txtNumeroActa.Size = new System.Drawing.Size(220, 20);
            this.lblFechaActa.Location = new System.Drawing.Point(400, 28); this.lblFechaActa.Size = new System.Drawing.Size(100, 23); this.lblFechaActa.Text = "Fecha";
            this.dtpFechaActa.Location = new System.Drawing.Point(510, 25); this.dtpFechaActa.Size = new System.Drawing.Size(220, 20);
            this.lblMotivo.Location = new System.Drawing.Point(16, 62); this.lblMotivo.Size = new System.Drawing.Size(120, 23); this.lblMotivo.Text = "Motivo";
            this.txtMotivo.Location = new System.Drawing.Point(150, 59); this.txtMotivo.Size = new System.Drawing.Size(580, 20);
            this.lblObservaciones.Location = new System.Drawing.Point(16, 96); this.lblObservaciones.Size = new System.Drawing.Size(120, 23); this.lblObservaciones.Text = "Observaciones";
            this.txtObservaciones.Location = new System.Drawing.Point(150, 93); this.txtObservaciones.Size = new System.Drawing.Size(580, 20);
            this.lblEvaluacion.Location = new System.Drawing.Point(12, 259); this.lblEvaluacion.Size = new System.Drawing.Size(860, 20);
            this.btnActualizar.Location = new System.Drawing.Point(12, 452); this.btnActualizar.Size = new System.Drawing.Size(120, 32); this.btnActualizar.Text = "Actualizar"; this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            this.btnRegistrarAlta.Location = new System.Drawing.Point(580, 452); this.btnRegistrarAlta.Size = new System.Drawing.Size(140, 32); this.btnRegistrarAlta.Text = "Registrar alta"; this.btnRegistrarAlta.Click += new System.EventHandler(this.btnRegistrarAlta_Click);
            this.btnRegistrarCierre.Location = new System.Drawing.Point(732, 452); this.btnRegistrarCierre.Size = new System.Drawing.Size(140, 32); this.btnRegistrarCierre.Text = "Registrar cierre"; this.btnRegistrarCierre.Click += new System.EventHandler(this.btnRegistrarCierre_Click);
            this.lblMensaje.Location = new System.Drawing.Point(12, 494); this.lblMensaje.Size = new System.Drawing.Size(860, 40);
            this.ClientSize = new System.Drawing.Size(884, 541); this.Controls.Add(this.dgvComisiones); this.Controls.Add(this.lblEvaluacion); this.Controls.Add(this.grpActa); this.Controls.Add(this.btnActualizar); this.Controls.Add(this.btnRegistrarAlta); this.Controls.Add(this.btnRegistrarCierre); this.Controls.Add(this.lblMensaje); this.Name = "FrmRegistrarEstadoDefinitivoComision_83KI"; this.Text = "Registrar estado definitivo de comisión";
            ((System.ComponentModel.ISupportInitialize)(this.dgvComisiones)).EndInit(); this.grpActa.ResumeLayout(false); this.grpActa.PerformLayout(); this.ResumeLayout(false);
        }
        private System.Windows.Forms.DataGridView dgvComisiones; private System.Windows.Forms.GroupBox grpActa; private System.Windows.Forms.Label lblNumeroActa; private System.Windows.Forms.TextBox txtNumeroActa; private System.Windows.Forms.Label lblFechaActa; private System.Windows.Forms.DateTimePicker dtpFechaActa; private System.Windows.Forms.Label lblMotivo; private System.Windows.Forms.TextBox txtMotivo; private System.Windows.Forms.Label lblObservaciones; private System.Windows.Forms.TextBox txtObservaciones; private System.Windows.Forms.Button btnActualizar; private System.Windows.Forms.Button btnRegistrarAlta; private System.Windows.Forms.Button btnRegistrarCierre; private System.Windows.Forms.Label lblEvaluacion; private System.Windows.Forms.Label lblMensaje;
    }
}

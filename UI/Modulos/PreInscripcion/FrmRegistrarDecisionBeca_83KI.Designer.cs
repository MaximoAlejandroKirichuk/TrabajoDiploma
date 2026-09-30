namespace UI.Modulos.PreInscripcion
{
    partial class FrmRegistrarDecisionBeca_83KI
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpBusqueda = new System.Windows.Forms.GroupBox();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.lblCodigoComision = new System.Windows.Forms.Label();
            this.cmbComisiones = new System.Windows.Forms.ComboBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblAlumno = new System.Windows.Forms.Label();
            this.lblSolicitud = new System.Windows.Forms.Label();
            this.grpResolucion = new System.Windows.Forms.GroupBox();
            this.lblTipoBeneficio = new System.Windows.Forms.Label();
            this.txtTipoBeneficio = new System.Windows.Forms.TextBox();
            this.lblFechaSolicitud = new System.Windows.Forms.Label();
            this.dtpFechaSolicitud = new System.Windows.Forms.DateTimePicker();
            this.lblEstadoBeca = new System.Windows.Forms.Label();
            this.cmbEstadoBeca = new System.Windows.Forms.ComboBox();
            this.lblMotivoDecision = new System.Windows.Forms.Label();
            this.txtMotivoDecision = new System.Windows.Forms.TextBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.grpBusqueda.SuspendLayout();
            this.grpResolucion.SuspendLayout();
            this.SuspendLayout();
            this.grpBusqueda.Controls.Add(this.lblDni); this.grpBusqueda.Controls.Add(this.txtDni); this.grpBusqueda.Controls.Add(this.lblCodigoComision); this.grpBusqueda.Controls.Add(this.cmbComisiones); this.grpBusqueda.Controls.Add(this.btnBuscar); this.grpBusqueda.Controls.Add(this.lblAlumno); this.grpBusqueda.Controls.Add(this.lblSolicitud);
            this.grpBusqueda.Location = new System.Drawing.Point(16, 15); this.grpBusqueda.Name = "grpBusqueda"; this.grpBusqueda.Size = new System.Drawing.Size(560, 140); this.grpBusqueda.TabStop = false; this.grpBusqueda.Text = "Búsqueda";
            this.lblDni.AutoSize = true; this.lblDni.Location = new System.Drawing.Point(18, 30); this.lblDni.Text = "DNI";
            this.txtDni.Location = new System.Drawing.Point(145, 27); this.txtDni.Name = "txtDni"; this.txtDni.Size = new System.Drawing.Size(140, 20);
            this.lblCodigoComision.AutoSize = true; this.lblCodigoComision.Location = new System.Drawing.Point(18, 62); this.lblCodigoComision.Text = "Código comisión";
            this.cmbComisiones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbComisiones.Location = new System.Drawing.Point(145, 59); this.cmbComisiones.Name = "cmbComisiones"; this.cmbComisiones.Size = new System.Drawing.Size(270, 21);
            this.btnBuscar.Location = new System.Drawing.Point(425, 55); this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(110, 28); this.btnBuscar.Text = "Buscar"; this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.lblAlumno.AutoSize = true; this.lblAlumno.Location = new System.Drawing.Point(18, 94); this.lblAlumno.Size = new System.Drawing.Size(0, 13);
            this.lblSolicitud.AutoSize = true; this.lblSolicitud.Location = new System.Drawing.Point(18, 116); this.lblSolicitud.Size = new System.Drawing.Size(0, 13);
            this.grpResolucion.Controls.Add(this.lblTipoBeneficio); this.grpResolucion.Controls.Add(this.txtTipoBeneficio); this.grpResolucion.Controls.Add(this.lblFechaSolicitud); this.grpResolucion.Controls.Add(this.dtpFechaSolicitud); this.grpResolucion.Controls.Add(this.lblEstadoBeca); this.grpResolucion.Controls.Add(this.cmbEstadoBeca); this.grpResolucion.Controls.Add(this.lblMotivoDecision); this.grpResolucion.Controls.Add(this.txtMotivoDecision);
            this.grpResolucion.Location = new System.Drawing.Point(16, 170); this.grpResolucion.Name = "grpResolucion"; this.grpResolucion.Size = new System.Drawing.Size(560, 190); this.grpResolucion.TabStop = false; this.grpResolucion.Text = "Resolución";
            this.lblTipoBeneficio.AutoSize = true; this.lblTipoBeneficio.Location = new System.Drawing.Point(18, 30); this.lblTipoBeneficio.Text = "Tipo beneficio";
            this.txtTipoBeneficio.Location = new System.Drawing.Point(145, 27); this.txtTipoBeneficio.Name = "txtTipoBeneficio"; this.txtTipoBeneficio.Size = new System.Drawing.Size(250, 20);
            this.lblFechaSolicitud.AutoSize = true; this.lblFechaSolicitud.Location = new System.Drawing.Point(18, 62); this.lblFechaSolicitud.Text = "Fecha solicitud";
            this.dtpFechaSolicitud.Format = System.Windows.Forms.DateTimePickerFormat.Short; this.dtpFechaSolicitud.Location = new System.Drawing.Point(145, 59); this.dtpFechaSolicitud.Name = "dtpFechaSolicitud"; this.dtpFechaSolicitud.Size = new System.Drawing.Size(140, 20);
            this.lblEstadoBeca.AutoSize = true; this.lblEstadoBeca.Location = new System.Drawing.Point(18, 94); this.lblEstadoBeca.Text = "Estado";
            this.cmbEstadoBeca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbEstadoBeca.Location = new System.Drawing.Point(145, 91); this.cmbEstadoBeca.Name = "cmbEstadoBeca"; this.cmbEstadoBeca.Size = new System.Drawing.Size(140, 21);
            this.lblMotivoDecision.AutoSize = true; this.lblMotivoDecision.Location = new System.Drawing.Point(18, 126); this.lblMotivoDecision.Text = "Motivo";
            this.txtMotivoDecision.Location = new System.Drawing.Point(145, 123); this.txtMotivoDecision.Multiline = true; this.txtMotivoDecision.Name = "txtMotivoDecision"; this.txtMotivoDecision.Size = new System.Drawing.Size(390, 50);
            this.btnGuardar.Location = new System.Drawing.Point(376, 376); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(95, 28); this.btnGuardar.Text = "Guardar"; this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnLimpiar.Location = new System.Drawing.Point(481, 376); this.btnLimpiar.Name = "btnLimpiar"; this.btnLimpiar.Size = new System.Drawing.Size(95, 28); this.btnLimpiar.Text = "Limpiar"; this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.lblMensaje.AutoSize = true; this.lblMensaje.ForeColor = System.Drawing.Color.Firebrick; this.lblMensaje.Location = new System.Drawing.Point(18, 420); this.lblMensaje.Size = new System.Drawing.Size(0, 13);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; this.ClientSize = new System.Drawing.Size(595, 455);
            this.Controls.Add(this.grpBusqueda); this.Controls.Add(this.grpResolucion); this.Controls.Add(this.btnGuardar); this.Controls.Add(this.btnLimpiar); this.Controls.Add(this.lblMensaje);
            this.Name = "FrmRegistrarDecisionBeca_83KI"; this.Text = "Registrar decisión de beca";
            this.grpBusqueda.ResumeLayout(false); this.grpBusqueda.PerformLayout(); this.grpResolucion.ResumeLayout(false); this.grpResolucion.PerformLayout(); this.ResumeLayout(false); this.PerformLayout();
        }

        private System.Windows.Forms.GroupBox grpBusqueda; private System.Windows.Forms.Label lblDni; private System.Windows.Forms.TextBox txtDni; private System.Windows.Forms.Label lblCodigoComision; private System.Windows.Forms.ComboBox cmbComisiones; private System.Windows.Forms.Button btnBuscar; private System.Windows.Forms.Label lblAlumno; private System.Windows.Forms.Label lblSolicitud; private System.Windows.Forms.GroupBox grpResolucion; private System.Windows.Forms.Label lblTipoBeneficio; private System.Windows.Forms.TextBox txtTipoBeneficio; private System.Windows.Forms.Label lblFechaSolicitud; private System.Windows.Forms.DateTimePicker dtpFechaSolicitud; private System.Windows.Forms.Label lblEstadoBeca; private System.Windows.Forms.ComboBox cmbEstadoBeca; private System.Windows.Forms.Label lblMotivoDecision; private System.Windows.Forms.TextBox txtMotivoDecision; private System.Windows.Forms.Button btnGuardar; private System.Windows.Forms.Button btnLimpiar; private System.Windows.Forms.Label lblMensaje;
    }
}

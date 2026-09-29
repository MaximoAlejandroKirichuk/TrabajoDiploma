namespace UI.Modulos.PreInscripcion
{
    partial class FrmRegistrarSolicitudInscripcion_83KI
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblBusqueda = new System.Windows.Forms.Label();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lstResultados = new System.Windows.Forms.ListBox();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblComision = new System.Windows.Forms.Label();
            this.cmbComisiones = new System.Windows.Forms.ComboBox();
            this.lblPlan = new System.Windows.Forms.Label();
            this.cmbPlanes = new System.Windows.Forms.ComboBox();
            this.lblPreview = new System.Windows.Forms.Label();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.lblBusqueda.AutoSize = true; this.lblBusqueda.Location = new System.Drawing.Point(16, 18); this.lblBusqueda.Name = "lblBusqueda"; this.lblBusqueda.Size = new System.Drawing.Size(58, 13); this.lblBusqueda.Text = "Busqueda";
            this.txtBusqueda.Location = new System.Drawing.Point(138, 15); this.txtBusqueda.Name = "txtBusqueda"; this.txtBusqueda.Size = new System.Drawing.Size(236, 20);
            this.btnBuscar.Location = new System.Drawing.Point(390, 13); this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(90, 24); this.btnBuscar.Text = "Buscar"; this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.lstResultados.FormattingEnabled = true; this.lstResultados.Location = new System.Drawing.Point(19, 49); this.lstResultados.Name = "lstResultados"; this.lstResultados.Size = new System.Drawing.Size(461, 82); this.lstResultados.SelectedIndexChanged += new System.EventHandler(this.lstResultados_SelectedIndexChanged);
            this.lblDni.AutoSize = true; this.lblDni.Location = new System.Drawing.Point(16, 150); this.lblDni.Text = "DNI";
            this.txtDni.Location = new System.Drawing.Point(138, 147); this.txtDni.Size = new System.Drawing.Size(160, 20);
            this.lblNombre.AutoSize = true; this.lblNombre.Location = new System.Drawing.Point(16, 181); this.lblNombre.Text = "Nombre";
            this.txtNombre.Location = new System.Drawing.Point(138, 178); this.txtNombre.Size = new System.Drawing.Size(236, 20);
            this.lblApellido.AutoSize = true; this.lblApellido.Location = new System.Drawing.Point(16, 212); this.lblApellido.Text = "Apellido";
            this.txtApellido.Location = new System.Drawing.Point(138, 209); this.txtApellido.Size = new System.Drawing.Size(236, 20);
            this.lblEmail.AutoSize = true; this.lblEmail.Location = new System.Drawing.Point(16, 243); this.lblEmail.Text = "Email";
            this.txtEmail.Location = new System.Drawing.Point(138, 240); this.txtEmail.Size = new System.Drawing.Size(236, 20);
            this.lblTelefono.AutoSize = true; this.lblTelefono.Location = new System.Drawing.Point(16, 274); this.lblTelefono.Text = "Telefono";
            this.txtTelefono.Location = new System.Drawing.Point(138, 271); this.txtTelefono.Size = new System.Drawing.Size(236, 20);
            this.lblComision.AutoSize = true; this.lblComision.Location = new System.Drawing.Point(16, 308); this.lblComision.Text = "Comision";
            this.cmbComisiones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbComisiones.Location = new System.Drawing.Point(138, 305); this.cmbComisiones.Size = new System.Drawing.Size(342, 21); this.cmbComisiones.SelectedIndexChanged += new System.EventHandler(this.cmbComisiones_SelectedIndexChanged);
            this.lblPlan.AutoSize = true; this.lblPlan.Location = new System.Drawing.Point(16, 339); this.lblPlan.Text = "Plan";
            this.cmbPlanes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbPlanes.Location = new System.Drawing.Point(138, 336); this.cmbPlanes.Size = new System.Drawing.Size(342, 21); this.cmbPlanes.SelectedIndexChanged += new System.EventHandler(this.cmbPlanes_SelectedIndexChanged);
            this.lblPreview.AutoSize = true; this.lblPreview.Location = new System.Drawing.Point(138, 366); this.lblPreview.Size = new System.Drawing.Size(0, 13);
            this.lblObservaciones.AutoSize = true; this.lblObservaciones.Location = new System.Drawing.Point(16, 395); this.lblObservaciones.Text = "Observaciones";
            this.txtObservaciones.Location = new System.Drawing.Point(138, 392); this.txtObservaciones.Multiline = true; this.txtObservaciones.Size = new System.Drawing.Size(342, 58);
            this.btnRegistrar.Location = new System.Drawing.Point(288, 469); this.btnRegistrar.Size = new System.Drawing.Size(92, 28); this.btnRegistrar.Text = "Registrar"; this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            this.btnLimpiar.Location = new System.Drawing.Point(388, 469); this.btnLimpiar.Size = new System.Drawing.Size(92, 28); this.btnLimpiar.Text = "Limpiar"; this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.lblMensaje.AutoSize = true; this.lblMensaje.ForeColor = System.Drawing.Color.Firebrick; this.lblMensaje.Location = new System.Drawing.Point(16, 510); this.lblMensaje.Size = new System.Drawing.Size(0, 13);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; this.ClientSize = new System.Drawing.Size(503, 540);
            this.Controls.Add(this.lblBusqueda); this.Controls.Add(this.txtBusqueda); this.Controls.Add(this.btnBuscar); this.Controls.Add(this.lstResultados); this.Controls.Add(this.lblDni); this.Controls.Add(this.txtDni); this.Controls.Add(this.lblNombre); this.Controls.Add(this.txtNombre); this.Controls.Add(this.lblApellido); this.Controls.Add(this.txtApellido); this.Controls.Add(this.lblEmail); this.Controls.Add(this.txtEmail); this.Controls.Add(this.lblTelefono); this.Controls.Add(this.txtTelefono); this.Controls.Add(this.lblComision); this.Controls.Add(this.cmbComisiones); this.Controls.Add(this.lblPlan); this.Controls.Add(this.cmbPlanes); this.Controls.Add(this.lblPreview); this.Controls.Add(this.lblObservaciones); this.Controls.Add(this.txtObservaciones); this.Controls.Add(this.btnRegistrar); this.Controls.Add(this.btnLimpiar); this.Controls.Add(this.lblMensaje);
            this.Name = "FrmRegistrarSolicitudInscripcion_83KI"; this.Text = "Registrar solicitud de inscripción"; this.ResumeLayout(false); this.PerformLayout();
        }

        private System.Windows.Forms.Label lblBusqueda; private System.Windows.Forms.TextBox txtBusqueda; private System.Windows.Forms.Button btnBuscar; private System.Windows.Forms.ListBox lstResultados; private System.Windows.Forms.Label lblDni; private System.Windows.Forms.TextBox txtDni; private System.Windows.Forms.Label lblNombre; private System.Windows.Forms.TextBox txtNombre; private System.Windows.Forms.Label lblApellido; private System.Windows.Forms.TextBox txtApellido; private System.Windows.Forms.Label lblEmail; private System.Windows.Forms.TextBox txtEmail; private System.Windows.Forms.Label lblTelefono; private System.Windows.Forms.TextBox txtTelefono; private System.Windows.Forms.Label lblComision; private System.Windows.Forms.ComboBox cmbComisiones; private System.Windows.Forms.Label lblPlan; private System.Windows.Forms.ComboBox cmbPlanes; private System.Windows.Forms.Label lblPreview; private System.Windows.Forms.Label lblObservaciones; private System.Windows.Forms.TextBox txtObservaciones; private System.Windows.Forms.Button btnRegistrar; private System.Windows.Forms.Button btnLimpiar; private System.Windows.Forms.Label lblMensaje;
    }
}

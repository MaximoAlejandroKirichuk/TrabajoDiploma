namespace UI.Modulos.PreInscripcion
{
    partial class FrmRegistrarConsultaLead_83KI
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.lblDni = new System.Windows.Forms.Label(); this.txtDni = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label(); this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label(); this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label(); this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label(); this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblComision = new System.Windows.Forms.Label(); this.cmbComisiones = new System.Windows.Forms.ComboBox();
            this.lblMedioContacto = new System.Windows.Forms.Label(); this.txtMedioContacto = new System.Windows.Forms.TextBox();
            this.lblMotivo = new System.Windows.Forms.Label(); this.txtMotivo = new System.Windows.Forms.TextBox();
            this.lblObservaciones = new System.Windows.Forms.Label(); this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button(); this.btnLimpiar = new System.Windows.Forms.Button(); this.btnRegistrar = new System.Windows.Forms.Button(); this.lblMensaje = new System.Windows.Forms.Label();
            this.SuspendLayout();
            int y = 20; AddRow(this.lblDni, this.txtDni, "DNI", y); y += 34; AddRow(this.lblNombre, this.txtNombre, "Nombre", y); y += 34; AddRow(this.lblApellido, this.txtApellido, "Apellido", y); y += 34; AddRow(this.lblEmail, this.txtEmail, "Email", y); y += 34; AddRow(this.lblTelefono, this.txtTelefono, "Telefono", y); y += 34;
            this.btnBuscar.Location = new System.Drawing.Point(360, 20); this.btnBuscar.Size = new System.Drawing.Size(120, 28); this.btnBuscar.Text = "Buscar"; this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.btnLimpiar.Location = new System.Drawing.Point(490, 20); this.btnLimpiar.Size = new System.Drawing.Size(160, 28); this.btnLimpiar.Text = "Limpiar / Nueva búsqueda"; this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.lblComision.Location = new System.Drawing.Point(20, y); this.lblComision.Size = new System.Drawing.Size(130, 23); this.lblComision.Text = "Comision"; this.cmbComisiones.Location = new System.Drawing.Point(160, y); this.cmbComisiones.Size = new System.Drawing.Size(300, 24); this.cmbComisiones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; y += 34;
            AddRow(this.lblMedioContacto, this.txtMedioContacto, "Medio", y); y += 34; AddRow(this.lblMotivo, this.txtMotivo, "Motivo", y); y += 34;
            this.lblObservaciones.Location = new System.Drawing.Point(20, y); this.lblObservaciones.Size = new System.Drawing.Size(130, 23); this.lblObservaciones.Text = "Observaciones"; this.txtObservaciones.Location = new System.Drawing.Point(160, y); this.txtObservaciones.Size = new System.Drawing.Size(300, 70); this.txtObservaciones.Multiline = true; y += 84;
            this.btnRegistrar.Location = new System.Drawing.Point(160, y); this.btnRegistrar.Size = new System.Drawing.Size(140, 32); this.btnRegistrar.Text = "Registrar"; this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            this.lblMensaje.Location = new System.Drawing.Point(20, y + 42); this.lblMensaje.Size = new System.Drawing.Size(520, 50);
            this.Controls.AddRange(new System.Windows.Forms.Control[] { lblDni, txtDni, lblNombre, txtNombre, lblApellido, txtApellido, lblEmail, txtEmail, lblTelefono, txtTelefono, lblComision, cmbComisiones, lblMedioContacto, txtMedioContacto, lblMotivo, txtMotivo, lblObservaciones, txtObservaciones, btnBuscar, btnLimpiar, btnRegistrar, lblMensaje });
            this.ClientSize = new System.Drawing.Size(680, y + 105); this.Name = "FrmRegistrarConsultaLead_83KI"; this.Text = "Registrar consulta"; this.ResumeLayout(false); this.PerformLayout();
        }

        private void AddRow(System.Windows.Forms.Label label, System.Windows.Forms.TextBox textBox, string text, int y)
        { label.Location = new System.Drawing.Point(20, y); label.Size = new System.Drawing.Size(130, 23); label.Text = text; textBox.Location = new System.Drawing.Point(160, y); textBox.Size = new System.Drawing.Size(180, 22); }

        private System.Windows.Forms.Label lblDni, lblNombre, lblApellido, lblEmail, lblTelefono, lblComision, lblMedioContacto, lblMotivo, lblObservaciones, lblMensaje;
        private System.Windows.Forms.TextBox txtDni, txtNombre, txtApellido, txtEmail, txtTelefono, txtMedioContacto, txtMotivo, txtObservaciones;
        private System.Windows.Forms.ComboBox cmbComisiones;
        private System.Windows.Forms.Button btnBuscar, btnLimpiar, btnRegistrar;
    }
}

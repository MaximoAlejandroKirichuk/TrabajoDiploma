namespace UI.Modulos.PreInscripcion
{
    partial class FrmRegistrarPagoInscripcion_83KI
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
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblAlumno = new System.Windows.Forms.Label();
            this.lblSolicitud = new System.Windows.Forms.Label();
            this.dgvCuotas = new System.Windows.Forms.DataGridView();
            this.grpPago = new System.Windows.Forms.GroupBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.txtMonto = new System.Windows.Forms.TextBox();
            this.lblMetodoPago = new System.Windows.Forms.Label();
            this.cmbMetodoPago = new System.Windows.Forms.ComboBox();
            this.lblNumeroReferencia = new System.Windows.Forms.Label();
            this.txtNumeroReferencia = new System.Windows.Forms.TextBox();
            this.lblFechaPago = new System.Windows.Forms.Label();
            this.dtpFechaPago = new System.Windows.Forms.DateTimePicker();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.grpBusqueda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuotas)).BeginInit();
            this.grpPago.SuspendLayout();
            this.SuspendLayout();
            this.grpBusqueda.Controls.Add(this.lblDni); this.grpBusqueda.Controls.Add(this.txtDni); this.grpBusqueda.Controls.Add(this.btnBuscar); this.grpBusqueda.Controls.Add(this.lblAlumno); this.grpBusqueda.Controls.Add(this.lblSolicitud);
            this.grpBusqueda.Location = new System.Drawing.Point(16, 15); this.grpBusqueda.Name = "grpBusqueda"; this.grpBusqueda.Size = new System.Drawing.Size(760, 110); this.grpBusqueda.TabStop = false; this.grpBusqueda.Text = "Búsqueda";
            this.lblDni.AutoSize = true; this.lblDni.Location = new System.Drawing.Point(18, 30); this.lblDni.Text = "DNI";
            this.txtDni.Location = new System.Drawing.Point(145, 27); this.txtDni.Name = "txtDni"; this.txtDni.Size = new System.Drawing.Size(140, 20);
            this.btnBuscar.Location = new System.Drawing.Point(300, 24); this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(110, 28); this.btnBuscar.Text = "Buscar"; this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.lblAlumno.AutoSize = true; this.lblAlumno.Location = new System.Drawing.Point(18, 62); this.lblAlumno.Size = new System.Drawing.Size(0, 13);
            this.lblSolicitud.AutoSize = true; this.lblSolicitud.Location = new System.Drawing.Point(18, 84); this.lblSolicitud.Size = new System.Drawing.Size(0, 13);
            this.dgvCuotas.AllowUserToAddRows = false; this.dgvCuotas.AllowUserToDeleteRows = false; this.dgvCuotas.AllowUserToResizeRows = false;
            this.dgvCuotas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; this.dgvCuotas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCuotas.Location = new System.Drawing.Point(16, 140); this.dgvCuotas.MultiSelect = false; this.dgvCuotas.Name = "dgvCuotas"; this.dgvCuotas.ReadOnly = true;
            this.dgvCuotas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect; this.dgvCuotas.Size = new System.Drawing.Size(760, 160);
            this.dgvCuotas.TabIndex = 5; this.dgvCuotas.SelectionChanged += new System.EventHandler(this.dgvCuotas_SelectionChanged);
            this.grpPago.Controls.Add(this.lblMonto); this.grpPago.Controls.Add(this.txtMonto); this.grpPago.Controls.Add(this.lblMetodoPago); this.grpPago.Controls.Add(this.cmbMetodoPago);
            this.grpPago.Controls.Add(this.lblNumeroReferencia); this.grpPago.Controls.Add(this.txtNumeroReferencia); this.grpPago.Controls.Add(this.lblFechaPago); this.grpPago.Controls.Add(this.dtpFechaPago);
            this.grpPago.Location = new System.Drawing.Point(16, 320); this.grpPago.Name = "grpPago"; this.grpPago.Size = new System.Drawing.Size(760, 150); this.grpPago.TabStop = false; this.grpPago.Text = "Datos del pago";
            this.lblMonto.AutoSize = true; this.lblMonto.Location = new System.Drawing.Point(18, 30); this.lblMonto.Text = "Monto recibido";
            this.txtMonto.Location = new System.Drawing.Point(145, 27); this.txtMonto.Name = "txtMonto"; this.txtMonto.ReadOnly = true; this.txtMonto.Size = new System.Drawing.Size(140, 20);
            this.lblMetodoPago.AutoSize = true; this.lblMetodoPago.Location = new System.Drawing.Point(18, 62); this.lblMetodoPago.Text = "Método de pago";
            this.cmbMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbMetodoPago.Location = new System.Drawing.Point(145, 59); this.cmbMetodoPago.Name = "cmbMetodoPago"; this.cmbMetodoPago.Size = new System.Drawing.Size(200, 21);
            this.lblNumeroReferencia.AutoSize = true; this.lblNumeroReferencia.Location = new System.Drawing.Point(380, 30); this.lblNumeroReferencia.Text = "Número de referencia";
            this.txtNumeroReferencia.Location = new System.Drawing.Point(540, 27); this.txtNumeroReferencia.Name = "txtNumeroReferencia"; this.txtNumeroReferencia.Size = new System.Drawing.Size(200, 20);
            this.lblFechaPago.AutoSize = true; this.lblFechaPago.Location = new System.Drawing.Point(380, 62); this.lblFechaPago.Text = "Fecha de pago";
            this.dtpFechaPago.Format = System.Windows.Forms.DateTimePickerFormat.Short; this.dtpFechaPago.Location = new System.Drawing.Point(540, 59); this.dtpFechaPago.Name = "dtpFechaPago"; this.dtpFechaPago.Size = new System.Drawing.Size(140, 20);
            this.btnGuardar.Location = new System.Drawing.Point(576, 486); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(95, 28); this.btnGuardar.Text = "Guardar"; this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnLimpiar.Location = new System.Drawing.Point(681, 486); this.btnLimpiar.Name = "btnLimpiar"; this.btnLimpiar.Size = new System.Drawing.Size(95, 28); this.btnLimpiar.Text = "Limpiar"; this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.lblMensaje.AutoSize = true; this.lblMensaje.ForeColor = System.Drawing.Color.Firebrick; this.lblMensaje.Location = new System.Drawing.Point(18, 530); this.lblMensaje.Size = new System.Drawing.Size(0, 13);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; this.ClientSize = new System.Drawing.Size(795, 560);
            this.Controls.Add(this.grpBusqueda); this.Controls.Add(this.dgvCuotas); this.Controls.Add(this.grpPago); this.Controls.Add(this.btnGuardar); this.Controls.Add(this.btnLimpiar); this.Controls.Add(this.lblMensaje);
            this.Name = "FrmRegistrarPagoInscripcion_83KI"; this.Text = "Registrar pago de inscripción";
            this.grpBusqueda.ResumeLayout(false); this.grpBusqueda.PerformLayout(); ((System.ComponentModel.ISupportInitialize)(this.dgvCuotas)).EndInit(); this.grpPago.ResumeLayout(false); this.grpPago.PerformLayout(); this.ResumeLayout(false); this.PerformLayout();
        }

        private System.Windows.Forms.GroupBox grpBusqueda;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblAlumno;
        private System.Windows.Forms.Label lblSolicitud;
        private System.Windows.Forms.DataGridView dgvCuotas;
        private System.Windows.Forms.GroupBox grpPago;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.TextBox txtMonto;
        private System.Windows.Forms.Label lblMetodoPago;
        private System.Windows.Forms.ComboBox cmbMetodoPago;
        private System.Windows.Forms.Label lblNumeroReferencia;
        private System.Windows.Forms.TextBox txtNumeroReferencia;
        private System.Windows.Forms.Label lblFechaPago;
        private System.Windows.Forms.DateTimePicker dtpFechaPago;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label lblMensaje;
    }
}

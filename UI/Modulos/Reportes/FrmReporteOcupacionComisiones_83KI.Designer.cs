namespace UI.Modulos.Reportes
{
    partial class FrmReporteOcupacionComisiones_83KI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblEstado = new System.Windows.Forms.Label();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dgvReporte = new System.Windows.Forms.DataGridView();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProfesor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFechaLimite = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCupoMinimo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCupoMaximo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInscriptos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPagaron = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPendientes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacantes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFaltan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuotasReintegro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMontoReintegro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotales = new System.Windows.Forms.Label();
            this.pnlGrafico = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).BeginInit();
            this.SuspendLayout();
            //
            // lblEstado
            //
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(12, 16);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(40, 13);
            this.lblEstado.TabIndex = 0;
            this.lblEstado.Text = "Estado";
            //
            // cmbEstado
            //
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Location = new System.Drawing.Point(70, 12);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(220, 21);
            this.cmbEstado.TabIndex = 1;
            this.cmbEstado.SelectionChangeCommitted += new System.EventHandler(this.cmbEstado_SelectionChangeCommitted);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(305, 10);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 25);
            this.btnActualizar.TabIndex = 2;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // lblFecha
            //
            this.lblFecha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFecha.Location = new System.Drawing.Point(1088, 16);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(300, 13);
            this.lblFecha.TabIndex = 3;
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            // dgvReporte
            //
            this.dgvReporte.AllowUserToAddRows = false;
            this.dgvReporte.AllowUserToDeleteRows = false;
            this.dgvReporte.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReporte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReporte.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCodigo,
            this.colCurso,
            this.colProfesor,
            this.colFechaLimite,
            this.colCupoMinimo,
            this.colCupoMaximo,
            this.colInscriptos,
            this.colPagaron,
            this.colPendientes,
            this.colVacantes,
            this.colFaltan,
            this.colEstado,
            this.colActa,
            this.colCuotasReintegro,
            this.colMontoReintegro});
            this.dgvReporte.Location = new System.Drawing.Point(12, 45);
            this.dgvReporte.MultiSelect = false;
            this.dgvReporte.Name = "dgvReporte";
            this.dgvReporte.ReadOnly = true;
            this.dgvReporte.RowHeadersVisible = false;
            this.dgvReporte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReporte.Size = new System.Drawing.Size(1116, 440);
            this.dgvReporte.TabIndex = 4;
            //
            // colCodigo
            //
            this.colCodigo.HeaderText = "Comisión";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.ReadOnly = true;
            //
            // colCurso
            //
            this.colCurso.FillWeight = 150F;
            this.colCurso.HeaderText = "Curso";
            this.colCurso.Name = "colCurso";
            this.colCurso.ReadOnly = true;
            //
            // colProfesor
            //
            this.colProfesor.FillWeight = 150F;
            this.colProfesor.HeaderText = "Profesor";
            this.colProfesor.Name = "colProfesor";
            this.colProfesor.ReadOnly = true;
            //
            // colFechaLimite
            //
            this.colFechaLimite.HeaderText = "Límite de pago";
            this.colFechaLimite.Name = "colFechaLimite";
            this.colFechaLimite.ReadOnly = true;
            //
            // colCupoMinimo
            //
            this.colCupoMinimo.FillWeight = 70F;
            this.colCupoMinimo.HeaderText = "Cupo mín.";
            this.colCupoMinimo.Name = "colCupoMinimo";
            this.colCupoMinimo.ReadOnly = true;
            //
            // colCupoMaximo
            //
            this.colCupoMaximo.FillWeight = 70F;
            this.colCupoMaximo.HeaderText = "Cupo máx.";
            this.colCupoMaximo.Name = "colCupoMaximo";
            this.colCupoMaximo.ReadOnly = true;
            //
            // colInscriptos
            //
            this.colInscriptos.FillWeight = 70F;
            this.colInscriptos.HeaderText = "Inscriptos";
            this.colInscriptos.Name = "colInscriptos";
            this.colInscriptos.ReadOnly = true;
            //
            // colPagaron
            //
            this.colPagaron.FillWeight = 70F;
            this.colPagaron.HeaderText = "Pagaron";
            this.colPagaron.Name = "colPagaron";
            this.colPagaron.ReadOnly = true;
            //
            // colPendientes
            //
            this.colPendientes.FillWeight = 70F;
            this.colPendientes.HeaderText = "Sin pagar";
            this.colPendientes.Name = "colPendientes";
            this.colPendientes.ReadOnly = true;
            //
            // colVacantes
            //
            this.colVacantes.FillWeight = 70F;
            this.colVacantes.HeaderText = "Vacantes";
            this.colVacantes.Name = "colVacantes";
            this.colVacantes.ReadOnly = true;
            //
            // colFaltan
            //
            this.colFaltan.FillWeight = 80F;
            this.colFaltan.HeaderText = "Faltan p/ quórum";
            this.colFaltan.Name = "colFaltan";
            this.colFaltan.ReadOnly = true;
            //
            // colEstado
            //
            this.colEstado.FillWeight = 110F;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            //
            // colActa
            //
            this.colActa.FillWeight = 80F;
            this.colActa.HeaderText = "Acta";
            this.colActa.Name = "colActa";
            this.colActa.ReadOnly = true;
            //
            // colCuotasReintegro
            //
            this.colCuotasReintegro.FillWeight = 80F;
            this.colCuotasReintegro.HeaderText = "Cuotas a devolver";
            this.colCuotasReintegro.Name = "colCuotasReintegro";
            this.colCuotasReintegro.ReadOnly = true;
            //
            // colMontoReintegro
            //
            this.colMontoReintegro.HeaderText = "Monto a devolver";
            this.colMontoReintegro.Name = "colMontoReintegro";
            this.colMontoReintegro.ReadOnly = true;
            //
            // lblTotales
            //
            this.lblTotales.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotales.Location = new System.Drawing.Point(12, 495);
            this.lblTotales.Name = "lblTotales";
            this.lblTotales.Size = new System.Drawing.Size(1376, 20);
            this.lblTotales.TabIndex = 5;
            // 
            // pnlGrafico
            // 
            this.pnlGrafico.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGrafico.BackColor = System.Drawing.Color.White;
            this.pnlGrafico.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGrafico.Location = new System.Drawing.Point(1136, 45);
            this.pnlGrafico.Name = "pnlGrafico";
            this.pnlGrafico.Size = new System.Drawing.Size(252, 440);
            this.pnlGrafico.TabIndex = 6;
            this.pnlGrafico.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlGrafico_Paint);
            this.pnlGrafico.Resize += new System.EventHandler(this.pnlGrafico_Resize);
            //
            // FrmReporteOcupacionComisiones_83KI
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 524);
            this.Controls.Add(this.pnlGrafico);
            this.Controls.Add(this.lblTotales);
            this.Controls.Add(this.dgvReporte);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.cmbEstado);
            this.Controls.Add(this.lblEstado);
            this.MinimumSize = new System.Drawing.Size(900, 400);
            this.Name = "FrmReporteOcupacionComisiones_83KI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reporte de ocupación de comisiones";
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DataGridView dgvReporte;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurso;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProfesor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFechaLimite;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCupoMinimo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCupoMaximo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInscriptos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPagaron;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPendientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacantes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFaltan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActa;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuotasReintegro;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMontoReintegro;
        private System.Windows.Forms.Label lblTotales;
        private System.Windows.Forms.Panel pnlGrafico;
    }
}

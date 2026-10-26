namespace UI.Modulos.CobrosMorosidadActas
{
    partial class FrmReporteMorosidad_83KI
    {
        /// <summary>
        /// Variable requerida por el diseñador.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Libera los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true para liberar los recursos administrados; de lo contrario, false.</param>
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
        /// Método requerido para el soporte del diseñador; no modificar.
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblCurso = new System.Windows.Forms.Label();
            this.cmbCurso = new System.Windows.Forms.ComboBox();
            this.lblComision = new System.Windows.Forms.Label();
            this.cmbComision = new System.Windows.Forms.ComboBox();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.lblDiasMinimos = new System.Windows.Forms.Label();
            this.nudDiasMinimos = new System.Windows.Forms.NumericUpDown();
            this.lblDeudaMinima = new System.Windows.Forms.Label();
            this.nudDeudaMinima = new System.Windows.Forms.NumericUpDown();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dgvMorosidad = new System.Windows.Forms.DataGridView();
            this.colCodigoInscripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDni = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlumno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTelefono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPlanDePago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuotasVencidas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVencimientoMasAntiguo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiasAtraso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeudaVencida = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeudaTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chartAtraso = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblTotales = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiasMinimos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeudaMinima)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMorosidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartAtraso)).BeginInit();
            this.SuspendLayout();
            //
            // lblCurso
            //
            this.lblCurso.AutoEllipsis = true;
            this.lblCurso.Location = new System.Drawing.Point(12, 9);
            this.lblCurso.Name = "lblCurso";
            this.lblCurso.Size = new System.Drawing.Size(210, 15);
            this.lblCurso.TabIndex = 0;
            this.lblCurso.Text = "Curso";
            //
            // cmbCurso
            //
            this.cmbCurso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurso.FormattingEnabled = true;
            this.cmbCurso.Location = new System.Drawing.Point(12, 27);
            this.cmbCurso.Name = "cmbCurso";
            this.cmbCurso.Size = new System.Drawing.Size(210, 21);
            this.cmbCurso.TabIndex = 1;
            this.cmbCurso.SelectionChangeCommitted += new System.EventHandler(this.cmbCurso_SelectionChangeCommitted);
            //
            // lblComision
            //
            this.lblComision.AutoEllipsis = true;
            this.lblComision.Location = new System.Drawing.Point(234, 9);
            this.lblComision.Name = "lblComision";
            this.lblComision.Size = new System.Drawing.Size(200, 15);
            this.lblComision.TabIndex = 2;
            this.lblComision.Text = "Comisión";
            //
            // cmbComision
            //
            this.cmbComision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComision.FormattingEnabled = true;
            this.cmbComision.Location = new System.Drawing.Point(234, 27);
            this.cmbComision.Name = "cmbComision";
            this.cmbComision.Size = new System.Drawing.Size(200, 21);
            this.cmbComision.TabIndex = 3;
            //
            // lblDni
            //
            this.lblDni.AutoEllipsis = true;
            this.lblDni.Location = new System.Drawing.Point(446, 9);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(110, 15);
            this.lblDni.TabIndex = 4;
            this.lblDni.Text = "DNI";
            //
            // txtDni
            //
            this.txtDni.Location = new System.Drawing.Point(446, 27);
            this.txtDni.MaxLength = 20;
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(110, 20);
            this.txtDni.TabIndex = 5;
            //
            // lblDiasMinimos
            //
            this.lblDiasMinimos.AutoEllipsis = true;
            this.lblDiasMinimos.Location = new System.Drawing.Point(568, 9);
            this.lblDiasMinimos.Name = "lblDiasMinimos";
            this.lblDiasMinimos.Size = new System.Drawing.Size(130, 15);
            this.lblDiasMinimos.TabIndex = 6;
            this.lblDiasMinimos.Text = "Días de atraso mínimos";
            //
            // nudDiasMinimos
            //
            this.nudDiasMinimos.Location = new System.Drawing.Point(568, 27);
            this.nudDiasMinimos.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
            this.nudDiasMinimos.Name = "nudDiasMinimos";
            this.nudDiasMinimos.Size = new System.Drawing.Size(130, 20);
            this.nudDiasMinimos.TabIndex = 7;
            this.nudDiasMinimos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lblDeudaMinima
            //
            this.lblDeudaMinima.AutoEllipsis = true;
            this.lblDeudaMinima.Location = new System.Drawing.Point(710, 9);
            this.lblDeudaMinima.Name = "lblDeudaMinima";
            this.lblDeudaMinima.Size = new System.Drawing.Size(150, 15);
            this.lblDeudaMinima.TabIndex = 8;
            this.lblDeudaMinima.Text = "Deuda vencida mínima";
            //
            // nudDeudaMinima
            //
            this.nudDeudaMinima.DecimalPlaces = 2;
            this.nudDeudaMinima.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDeudaMinima.Location = new System.Drawing.Point(710, 27);
            this.nudDeudaMinima.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.nudDeudaMinima.Name = "nudDeudaMinima";
            this.nudDeudaMinima.Size = new System.Drawing.Size(150, 20);
            this.nudDeudaMinima.TabIndex = 9;
            this.nudDeudaMinima.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDeudaMinima.ThousandsSeparator = true;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(872, 24);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 26);
            this.btnBuscar.TabIndex = 10;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // btnLimpiar
            //
            this.btnLimpiar.Location = new System.Drawing.Point(980, 24);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(100, 26);
            this.btnLimpiar.TabIndex = 11;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            //
            // lblFecha
            //
            this.lblFecha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFecha.Location = new System.Drawing.Point(1088, 30);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(200, 15);
            this.lblFecha.TabIndex = 12;
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            // dgvMorosidad
            //
            this.dgvMorosidad.AllowUserToAddRows = false;
            this.dgvMorosidad.AllowUserToDeleteRows = false;
            this.dgvMorosidad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMorosidad.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMorosidad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMorosidad.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCodigoInscripcion,
            this.colDni,
            this.colAlumno,
            this.colEmail,
            this.colTelefono,
            this.colComision,
            this.colCurso,
            this.colPlanDePago,
            this.colCuotasVencidas,
            this.colVencimientoMasAntiguo,
            this.colDiasAtraso,
            this.colDeudaVencida,
            this.colDeudaTotal});
            this.dgvMorosidad.Location = new System.Drawing.Point(12, 60);
            this.dgvMorosidad.MultiSelect = false;
            this.dgvMorosidad.Name = "dgvMorosidad";
            this.dgvMorosidad.ReadOnly = true;
            this.dgvMorosidad.RowHeadersVisible = false;
            this.dgvMorosidad.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMorosidad.Size = new System.Drawing.Size(950, 520);
            this.dgvMorosidad.TabIndex = 13;
            //
            // colCodigoInscripcion
            //
            this.colCodigoInscripcion.FillWeight = 90F;
            this.colCodigoInscripcion.HeaderText = "Inscripción";
            this.colCodigoInscripcion.Name = "colCodigoInscripcion";
            this.colCodigoInscripcion.ReadOnly = true;
            //
            // colDni
            //
            this.colDni.FillWeight = 75F;
            this.colDni.HeaderText = "DNI";
            this.colDni.Name = "colDni";
            this.colDni.ReadOnly = true;
            //
            // colAlumno
            //
            this.colAlumno.FillWeight = 140F;
            this.colAlumno.HeaderText = "Apellido y nombre";
            this.colAlumno.Name = "colAlumno";
            this.colAlumno.ReadOnly = true;
            //
            // colEmail
            //
            this.colEmail.FillWeight = 140F;
            this.colEmail.HeaderText = "Email";
            this.colEmail.Name = "colEmail";
            this.colEmail.ReadOnly = true;
            //
            // colTelefono
            //
            this.colTelefono.FillWeight = 85F;
            this.colTelefono.HeaderText = "Teléfono";
            this.colTelefono.Name = "colTelefono";
            this.colTelefono.ReadOnly = true;
            //
            // colComision
            //
            this.colComision.FillWeight = 85F;
            this.colComision.HeaderText = "Comisión";
            this.colComision.Name = "colComision";
            this.colComision.ReadOnly = true;
            //
            // colCurso
            //
            this.colCurso.FillWeight = 120F;
            this.colCurso.HeaderText = "Curso";
            this.colCurso.Name = "colCurso";
            this.colCurso.ReadOnly = true;
            //
            // colPlanDePago
            //
            this.colPlanDePago.FillWeight = 85F;
            this.colPlanDePago.HeaderText = "Plan de pago";
            this.colPlanDePago.Name = "colPlanDePago";
            this.colPlanDePago.ReadOnly = true;
            //
            // colCuotasVencidas
            //
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colCuotasVencidas.DefaultCellStyle = dataGridViewCellStyle1;
            this.colCuotasVencidas.FillWeight = 70F;
            this.colCuotasVencidas.HeaderText = "Cuotas vencidas";
            this.colCuotasVencidas.Name = "colCuotasVencidas";
            this.colCuotasVencidas.ReadOnly = true;
            //
            // colVencimientoMasAntiguo
            //
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "dd/MM/yyyy";
            this.colVencimientoMasAntiguo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colVencimientoMasAntiguo.FillWeight = 85F;
            this.colVencimientoMasAntiguo.HeaderText = "Vencimiento más antiguo";
            this.colVencimientoMasAntiguo.Name = "colVencimientoMasAntiguo";
            this.colVencimientoMasAntiguo.ReadOnly = true;
            //
            // colDiasAtraso
            //
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colDiasAtraso.DefaultCellStyle = dataGridViewCellStyle3;
            this.colDiasAtraso.FillWeight = 65F;
            this.colDiasAtraso.HeaderText = "Días de atraso";
            this.colDiasAtraso.Name = "colDiasAtraso";
            this.colDiasAtraso.ReadOnly = true;
            //
            // colDeudaVencida
            //
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            this.colDeudaVencida.DefaultCellStyle = dataGridViewCellStyle4;
            this.colDeudaVencida.FillWeight = 90F;
            this.colDeudaVencida.HeaderText = "Deuda vencida";
            this.colDeudaVencida.Name = "colDeudaVencida";
            this.colDeudaVencida.ReadOnly = true;
            //
            // colDeudaTotal
            //
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            this.colDeudaTotal.DefaultCellStyle = dataGridViewCellStyle5;
            this.colDeudaTotal.FillWeight = 90F;
            this.colDeudaTotal.HeaderText = "Deuda total";
            this.colDeudaTotal.Name = "colDeudaTotal";
            this.colDeudaTotal.ReadOnly = true;
            //
            // chartAtraso
            //
            this.chartAtraso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartAtraso.BorderlineColor = System.Drawing.Color.Silver;
            this.chartAtraso.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "AreaAtraso";
            this.chartAtraso.ChartAreas.Add(chartArea1);
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "LeyendaAtraso";
            this.chartAtraso.Legends.Add(legend1);
            this.chartAtraso.Location = new System.Drawing.Point(974, 60);
            this.chartAtraso.Name = "chartAtraso";
            series1.ChartArea = "AreaAtraso";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.IsValueShownAsLabel = true;
            series1.Label = "#VALY (#PERCENT{P0})";
            series1.Legend = "LeyendaAtraso";
            series1.Name = "SerieAtraso";
            this.chartAtraso.Series.Add(series1);
            this.chartAtraso.Size = new System.Drawing.Size(314, 520);
            this.chartAtraso.TabIndex = 14;
            title1.Name = "TituloAtraso";
            title1.Text = "Inscripciones morosas por días de atraso";
            this.chartAtraso.Titles.Add(title1);
            //
            // lblTotales
            //
            this.lblTotales.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotales.AutoEllipsis = true;
            this.lblTotales.Location = new System.Drawing.Point(12, 590);
            this.lblTotales.Name = "lblTotales";
            this.lblTotales.Size = new System.Drawing.Size(1276, 20);
            this.lblTotales.TabIndex = 15;
            //
            // FrmReporteMorosidad_83KI
            //
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 620);
            this.Controls.Add(this.lblTotales);
            this.Controls.Add(this.chartAtraso);
            this.Controls.Add(this.dgvMorosidad);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.nudDeudaMinima);
            this.Controls.Add(this.lblDeudaMinima);
            this.Controls.Add(this.nudDiasMinimos);
            this.Controls.Add(this.lblDiasMinimos);
            this.Controls.Add(this.txtDni);
            this.Controls.Add(this.lblDni);
            this.Controls.Add(this.cmbComision);
            this.Controls.Add(this.lblComision);
            this.Controls.Add(this.cmbCurso);
            this.Controls.Add(this.lblCurso);
            this.MinimumSize = new System.Drawing.Size(1100, 500);
            this.Name = "FrmReporteMorosidad_83KI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Informe de morosidad por alumno y comisión";
            ((System.ComponentModel.ISupportInitialize)(this.nudDiasMinimos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeudaMinima)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMorosidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartAtraso)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurso;
        private System.Windows.Forms.ComboBox cmbCurso;
        private System.Windows.Forms.Label lblComision;
        private System.Windows.Forms.ComboBox cmbComision;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Label lblDiasMinimos;
        private System.Windows.Forms.NumericUpDown nudDiasMinimos;
        private System.Windows.Forms.Label lblDeudaMinima;
        private System.Windows.Forms.NumericUpDown nudDeudaMinima;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DataGridView dgvMorosidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigoInscripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDni;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlumno;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTelefono;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComision;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurso;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPlanDePago;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuotasVencidas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVencimientoMasAntiguo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiasAtraso;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeudaVencida;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeudaTotal;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartAtraso;
        private System.Windows.Forms.Label lblTotales;
    }
}

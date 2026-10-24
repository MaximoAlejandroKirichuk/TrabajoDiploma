namespace UI.Modulos.PlanificacionAcademica
{
    partial class FrmRegistrarResultadosEvaluacion_83KI
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label lblCurso, lblComision, lblFecha, lblNota, lblObservaciones, lblMensaje;
        private System.Windows.Forms.ComboBox cmbCurso, cmbComision;
        private System.Windows.Forms.TextBox txtNota, txtObservaciones;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Button btnBuscar, btnAgregar, btnQuitar, btnRegistrar;
        private System.Windows.Forms.DataGridView dgvAlumnos;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblCurso = new System.Windows.Forms.Label { Location = new System.Drawing.Point(16, 16), Size = new System.Drawing.Size(230, 22) };
            lblComision = new System.Windows.Forms.Label { Location = new System.Drawing.Point(256, 16), Size = new System.Drawing.Size(230, 22) };
            lblFecha = new System.Windows.Forms.Label { Location = new System.Drawing.Point(496, 16), Size = new System.Drawing.Size(230, 22) };
            cmbCurso = new System.Windows.Forms.ComboBox { Name = "cmbCurso", Location = new System.Drawing.Point(16, 42), Width = 220, DropDownWidth = 400, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, DisplayMember = "Curso", TabIndex = 0 };
            cmbComision = new System.Windows.Forms.ComboBox { Name = "cmbComision", Location = new System.Drawing.Point(256, 42), Width = 220, DropDownWidth = 400, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, FormattingEnabled = true, TabIndex = 1 };
            dtpFecha = new System.Windows.Forms.DateTimePicker { Name = "dtpFecha", Location = new System.Drawing.Point(496, 42), Width = 220, Format = System.Windows.Forms.DateTimePickerFormat.Short, TabIndex = 2 };
            btnBuscar = new System.Windows.Forms.Button { Name = "btnBuscar", Location = new System.Drawing.Point(736, 36), Size = new System.Drawing.Size(140, 34), TabIndex = 3 };
            dgvAlumnos = new System.Windows.Forms.DataGridView { Name = "dgvAlumnos", Location = new System.Drawing.Point(16, 88), Size = new System.Drawing.Size(860, 270),
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false, RowHeadersVisible = false,
                SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill, TabIndex = 4 };
            dgvAlumnos.Columns.Add("DNI", "DNI"); dgvAlumnos.Columns.Add("Nombre", "Nombre"); dgvAlumnos.Columns.Add("Apellido", "Apellido");
            dgvAlumnos.Columns.Add("Nota", "Nota"); dgvAlumnos.Columns.Add("Observaciones", "Observaciones");
            lblNota = new System.Windows.Forms.Label { Location = new System.Drawing.Point(16, 374), Size = new System.Drawing.Size(200, 22) };
            txtNota = new System.Windows.Forms.TextBox { Name = "txtNota", Location = new System.Drawing.Point(16, 400), Width = 140, Enabled = false, TabIndex = 5 };
            lblObservaciones = new System.Windows.Forms.Label { Location = new System.Drawing.Point(176, 374), AutoSize = true };
            txtObservaciones = new System.Windows.Forms.TextBox { Name = "txtObservaciones", Location = new System.Drawing.Point(176, 400), Size = new System.Drawing.Size(440, 76), Multiline = true, MaxLength = 500, Enabled = false, TabIndex = 6 };
            btnAgregar = new System.Windows.Forms.Button { Name = "btnAgregar", Location = new System.Drawing.Point(636, 396), Size = new System.Drawing.Size(240, 36), Enabled = false, TabIndex = 7 };
            btnQuitar = new System.Windows.Forms.Button { Name = "btnQuitar", Location = new System.Drawing.Point(636, 440), Size = new System.Drawing.Size(240, 36), Enabled = false, TabIndex = 8 };
            btnRegistrar = new System.Windows.Forms.Button { Name = "btnRegistrar", Location = new System.Drawing.Point(416, 492), Size = new System.Drawing.Size(460, 40), Enabled = false, TabIndex = 9 };
            lblMensaje = new System.Windows.Forms.Label { Location = new System.Drawing.Point(16, 548), Size = new System.Drawing.Size(860, 64) };
            SuspendLayout();
            Controls.AddRange(new System.Windows.Forms.Control[] { lblCurso, lblComision, lblFecha, cmbCurso, cmbComision, dtpFecha, btnBuscar, dgvAlumnos,
                lblNota, txtNota, lblObservaciones, txtObservaciones, btnAgregar, btnQuitar, btnRegistrar, lblMensaje });
            btnBuscar.Click += Buscar_Click; btnAgregar.Click += Agregar_Click; btnQuitar.Click += Quitar_Click; btnRegistrar.Click += Registrar_Click;
            cmbCurso.SelectedIndexChanged += CursoChanged; cmbComision.SelectedIndexChanged += CriteriosChanged; dtpFecha.ValueChanged += CriteriosChanged;
            cmbComision.Format += ComisionFormat;
            dgvAlumnos.SelectionChanged += SeleccionChanged;
            txtNota.KeyPress += Nota_KeyPress;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Font = new System.Drawing.Font("Segoe UI", 9F); ClientSize = new System.Drawing.Size(896, 630);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; MaximizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Name = "FrmRegistrarResultadosEvaluacion_83KI";
            ResumeLayout(false); PerformLayout();
        }
    }
}

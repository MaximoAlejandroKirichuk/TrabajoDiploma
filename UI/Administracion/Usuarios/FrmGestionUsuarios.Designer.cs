namespace UI
{
    partial class FrmGestionUsuarios
    {
        private System.ComponentModel.IContainer components = null;

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
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.btnCrearUsuario = new System.Windows.Forms.Button();
            this.btnCambiarEstadoUsuario = new System.Windows.Forms.Button();
            this.btnModificarUsuario = new System.Windows.Forms.Button();
            this.btnDesbloquearUsuario = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Location = new System.Drawing.Point(26, 44);
            this.dgvUsuarios.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(540, 334);
            this.dgvUsuarios.TabIndex = 0;
            this.dgvUsuarios.SelectionChanged += new System.EventHandler(this.dgvUsuarios_SelectionChanged);
            // 
            // btnCrearUsuario
            // 
            this.btnCrearUsuario.Location = new System.Drawing.Point(601, 136);
            this.btnCrearUsuario.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnCrearUsuario.Name = "btnCrearUsuario";
            this.btnCrearUsuario.Size = new System.Drawing.Size(187, 37);
            this.btnCrearUsuario.TabIndex = 1;
            this.btnCrearUsuario.Text = "Crear usuario";
            this.btnCrearUsuario.UseVisualStyleBackColor = true;
            this.btnCrearUsuario.Click += new System.EventHandler(this.btnCrearUsuario_Click);
            // 
            // btnCambiarEstadoUsuario
            // 
            this.btnCambiarEstadoUsuario.Location = new System.Drawing.Point(601, 227);
            this.btnCambiarEstadoUsuario.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnCambiarEstadoUsuario.Name = "btnCambiarEstadoUsuario";
            this.btnCambiarEstadoUsuario.Size = new System.Drawing.Size(187, 37);
            this.btnCambiarEstadoUsuario.TabIndex = 2;
            this.btnCambiarEstadoUsuario.Text = "Gestionar estado";
            this.btnCambiarEstadoUsuario.UseVisualStyleBackColor = true;
            this.btnCambiarEstadoUsuario.Click += new System.EventHandler(this.btnCambiarEstadoUsuario_Click);
            // 
            // btnModificarUsuario
            // 
            this.btnModificarUsuario.Location = new System.Drawing.Point(601, 182);
            this.btnModificarUsuario.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnModificarUsuario.Name = "btnModificarUsuario";
            this.btnModificarUsuario.Size = new System.Drawing.Size(187, 37);
            this.btnModificarUsuario.TabIndex = 3;
            this.btnModificarUsuario.Text = "Modificar usuario";
            this.btnModificarUsuario.UseVisualStyleBackColor = true;
            this.btnModificarUsuario.Click += new System.EventHandler(this.btnModificarUsuario_Click);
            // 
            // btnDesbloquearUsuario
            // 
            this.btnDesbloquearUsuario.Location = new System.Drawing.Point(601, 272);
            this.btnDesbloquearUsuario.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnDesbloquearUsuario.Name = "btnDesbloquearUsuario";
            this.btnDesbloquearUsuario.Size = new System.Drawing.Size(187, 37);
            this.btnDesbloquearUsuario.TabIndex = 4;
            this.btnDesbloquearUsuario.Text = "Desbloquear usuario";
            this.btnDesbloquearUsuario.UseVisualStyleBackColor = true;
            this.btnDesbloquearUsuario.Click += new System.EventHandler(this.btnDesbloquearUsuario_Click);
            // 
            // FrmGestionUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(823, 422);
            this.Controls.Add(this.btnDesbloquearUsuario);
            this.Controls.Add(this.btnModificarUsuario);
            this.Controls.Add(this.btnCambiarEstadoUsuario);
            this.Controls.Add(this.btnCrearUsuario);
            this.Controls.Add(this.dgvUsuarios);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "FrmGestionUsuarios";
            this.Text = "FrmGestionUsuarios";
            this.Load += new System.EventHandler(this.FrmGestionUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.Button btnCrearUsuario;
        private System.Windows.Forms.Button btnCambiarEstadoUsuario;
        private System.Windows.Forms.Button btnModificarUsuario;
        private System.Windows.Forms.Button btnDesbloquearUsuario;
    }
}

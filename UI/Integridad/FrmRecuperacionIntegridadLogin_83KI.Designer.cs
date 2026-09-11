namespace UI
{
    partial class FrmRecuperacionIntegridadLogin_83KI
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpEstado = new System.Windows.Forms.GroupBox();
            this.lblTablas = new System.Windows.Forms.Label();
            this.treeInconsistencias = new System.Windows.Forms.TreeView();
            this.lblEstadoSistema = new System.Windows.Forms.Label();
            this.grpAcciones = new System.Windows.Forms.GroupBox();
            this.lblRestoreDesc = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lblRecalcularDesc = new System.Windows.Forms.Label();
            this.btnRecalcular = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.grpEstado.SuspendLayout();
            this.grpAcciones.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(400, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Recuperacion de integridad";
            // 
            // grpEstado
            // 
            this.grpEstado.Controls.Add(this.lblTablas);
            this.grpEstado.Controls.Add(this.treeInconsistencias);
            this.grpEstado.Controls.Add(this.lblEstadoSistema);
            this.grpEstado.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.grpEstado.Location = new System.Drawing.Point(19, 65);
            this.grpEstado.Name = "grpEstado";
            this.grpEstado.Size = new System.Drawing.Size(800, 535);
            this.grpEstado.TabIndex = 1;
            this.grpEstado.TabStop = false;
            this.grpEstado.Text = "Estado del sistema";
            // 
            // lblEstadoSistema
            // 
            this.lblEstadoSistema.AutoSize = true;
            this.lblEstadoSistema.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblEstadoSistema.ForeColor = System.Drawing.Color.DarkRed;
            this.lblEstadoSistema.Location = new System.Drawing.Point(13, 33);
            this.lblEstadoSistema.Name = "lblEstadoSistema";
            this.lblEstadoSistema.Size = new System.Drawing.Size(350, 21);
            this.lblEstadoSistema.TabIndex = 0;
            this.lblEstadoSistema.Text = "La integridad de los datos esta comprometida.";
            // 
            // lblTablas
            // 
            this.lblTablas.AutoSize = true;
            this.lblTablas.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.lblTablas.Location = new System.Drawing.Point(14, 67);
            this.lblTablas.Name = "lblTablas";
            this.lblTablas.Size = new System.Drawing.Size(120, 20);
            this.lblTablas.TabIndex = 1;
            this.lblTablas.Text = "Tablas afectadas";
            // 
            // treeInconsistencias
            // 
            this.treeInconsistencias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeInconsistencias.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.treeInconsistencias.Location = new System.Drawing.Point(17, 97);
            this.treeInconsistencias.Name = "treeInconsistencias";
            this.treeInconsistencias.Size = new System.Drawing.Size(760, 420);
            this.treeInconsistencias.TabIndex = 2;
            // 
            // grpAcciones
            // 
            this.grpAcciones.Controls.Add(this.lblRestoreDesc);
            this.grpAcciones.Controls.Add(this.btnRestore);
            this.grpAcciones.Controls.Add(this.lblRecalcularDesc);
            this.grpAcciones.Controls.Add(this.btnRecalcular);
            this.grpAcciones.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.grpAcciones.Location = new System.Drawing.Point(19, 615);
            this.grpAcciones.Name = "grpAcciones";
            this.grpAcciones.Size = new System.Drawing.Size(800, 170);
            this.grpAcciones.TabIndex = 2;
            this.grpAcciones.TabStop = false;
            this.grpAcciones.Text = "Acciones de recuperacion";
            // 
            // btnRecalcular
            // 
            this.btnRecalcular.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRecalcular.ForeColor = System.Drawing.Color.Black;
            this.btnRecalcular.Location = new System.Drawing.Point(17, 37);
            this.btnRecalcular.Name = "btnRecalcular";
            this.btnRecalcular.Size = new System.Drawing.Size(200, 45);
            this.btnRecalcular.TabIndex = 0;
            this.btnRecalcular.Text = "Recalcular hashes";
            this.btnRecalcular.UseVisualStyleBackColor = true;
            this.btnRecalcular.Click += new System.EventHandler(this.btnRecalcular_Click);
            // 
            // lblRecalcularDesc
            // 
            this.lblRecalcularDesc.AutoSize = true;
            this.lblRecalcularDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRecalcularDesc.ForeColor = System.Drawing.Color.Black;
            this.lblRecalcularDesc.Location = new System.Drawing.Point(230, 50);
            this.lblRecalcularDesc.Name = "lblRecalcularDesc";
            this.lblRecalcularDesc.Size = new System.Drawing.Size(300, 19);
            this.lblRecalcularDesc.TabIndex = 1;
            this.lblRecalcularDesc.Text = "Recomputa los valores de integridad desde los datos actuales.";
            // 
            // btnRestore
            // 
            this.btnRestore.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRestore.ForeColor = System.Drawing.Color.Black;
            this.btnRestore.Location = new System.Drawing.Point(17, 100);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(200, 45);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Restaurar base de datos";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // lblRestoreDesc
            // 
            this.lblRestoreDesc.AutoSize = true;
            this.lblRestoreDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.lblRestoreDesc.ForeColor = System.Drawing.Color.Black;
            this.lblRestoreDesc.Location = new System.Drawing.Point(230, 113);
            this.lblRestoreDesc.Name = "lblRestoreDesc";
            this.lblRestoreDesc.Size = new System.Drawing.Size(300, 19);
            this.lblRestoreDesc.TabIndex = 3;
            this.lblRestoreDesc.Text = "Restaura la base de datos desde un archivo de backup. La aplicacion se cerrara.";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnCerrar.Location = new System.Drawing.Point(696, 800);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(123, 40);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmRecuperacionIntegridadLogin_83KI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(840, 855);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.grpAcciones);
            this.Controls.Add(this.grpEstado);
            this.Controls.Add(this.lblTitulo);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRecuperacionIntegridadLogin_83KI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recuperacion de integridad";
            this.Load += new System.EventHandler(this.FrmRecuperacionIntegridadLogin_Load);
            this.grpEstado.ResumeLayout(false);
            this.grpEstado.PerformLayout();
            this.grpAcciones.ResumeLayout(false);
            this.grpAcciones.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpEstado;
        private System.Windows.Forms.Label lblEstadoSistema;
        private System.Windows.Forms.Label lblTablas;
        private System.Windows.Forms.TreeView treeInconsistencias;
        private System.Windows.Forms.GroupBox grpAcciones;
        private System.Windows.Forms.Button btnRecalcular;
        private System.Windows.Forms.Label lblRecalcularDesc;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Label lblRestoreDesc;
        private System.Windows.Forms.Button btnCerrar;
    }
}

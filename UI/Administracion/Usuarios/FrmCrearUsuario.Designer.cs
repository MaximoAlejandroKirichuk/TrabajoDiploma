namespace UI
{
    partial class FrmCrearUsuario
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
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txt_Dni = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.frm_lbl_nombre = new System.Windows.Forms.Label();
            this.frm_lbl_apellido = new System.Windows.Forms.Label();
            this.frm_lbl_email = new System.Windows.Forms.Label();
            this.btnCrearUsuario = new System.Windows.Forms.Button();
            this.frm_lbl_dni = new System.Windows.Forms.Label();
            this.frm_lbl_rol = new System.Windows.Forms.Label();
            this.panelCrearUsu = new System.Windows.Forms.Panel();
            this.panelCrearUsu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(153, 190);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(244, 43);
            this.txtNombre.TabIndex = 0;
            // 
            // txtApellido
            // 
            this.txtApellido.Location = new System.Drawing.Point(153, 288);
            this.txtApellido.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(244, 43);
            this.txtApellido.TabIndex = 1;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(153, 386);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(244, 43);
            this.txtEmail.TabIndex = 2;
            // 
            // txt_Dni
            // 
            this.txt_Dni.Location = new System.Drawing.Point(153, 478);
            this.txt_Dni.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.txt_Dni.Name = "txt_Dni";
            this.txt_Dni.Size = new System.Drawing.Size(244, 43);
            this.txt_Dni.TabIndex = 3;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(153, 570);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(244, 45);
            this.comboBox1.TabIndex = 4;
            // 
            // frm_lbl_nombre
            // 
            this.frm_lbl_nombre.AutoSize = true;
            this.frm_lbl_nombre.Location = new System.Drawing.Point(146, 144);
            this.frm_lbl_nombre.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.frm_lbl_nombre.Name = "frm_lbl_nombre";
            this.frm_lbl_nombre.Size = new System.Drawing.Size(115, 37);
            this.frm_lbl_nombre.TabIndex = 5;
            this.frm_lbl_nombre.Text = "Nombre";
            // 
            // frm_lbl_apellido
            // 
            this.frm_lbl_apellido.AutoSize = true;
            this.frm_lbl_apellido.Location = new System.Drawing.Point(146, 242);
            this.frm_lbl_apellido.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.frm_lbl_apellido.Name = "frm_lbl_apellido";
            this.frm_lbl_apellido.Size = new System.Drawing.Size(117, 37);
            this.frm_lbl_apellido.TabIndex = 6;
            this.frm_lbl_apellido.Text = "Apellido";
            // 
            // frm_lbl_email
            // 
            this.frm_lbl_email.AutoSize = true;
            this.frm_lbl_email.Location = new System.Drawing.Point(146, 340);
            this.frm_lbl_email.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.frm_lbl_email.Name = "frm_lbl_email";
            this.frm_lbl_email.Size = new System.Drawing.Size(82, 37);
            this.frm_lbl_email.TabIndex = 7;
            this.frm_lbl_email.Text = "Email";
            // 
            // btnCrearUsuario
            // 
            this.btnCrearUsuario.Location = new System.Drawing.Point(137, 633);
            this.btnCrearUsuario.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.btnCrearUsuario.Name = "btnCrearUsuario";
            this.btnCrearUsuario.Size = new System.Drawing.Size(268, 82);
            this.btnCrearUsuario.TabIndex = 8;
            this.btnCrearUsuario.Text = "Crear Usuario";
            this.btnCrearUsuario.UseVisualStyleBackColor = true;
            this.btnCrearUsuario.Click += new System.EventHandler(this.btnCrearUsuario_Click);
            // 
            // frm_lbl_dni
            // 
            this.frm_lbl_dni.AutoSize = true;
            this.frm_lbl_dni.Location = new System.Drawing.Point(146, 432);
            this.frm_lbl_dni.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.frm_lbl_dni.Name = "frm_lbl_dni";
            this.frm_lbl_dni.Size = new System.Drawing.Size(58, 37);
            this.frm_lbl_dni.TabIndex = 9;
            this.frm_lbl_dni.Text = "Dni";
            // 
            // frm_lbl_rol
            // 
            this.frm_lbl_rol.AutoSize = true;
            this.frm_lbl_rol.Location = new System.Drawing.Point(146, 524);
            this.frm_lbl_rol.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.frm_lbl_rol.Name = "frm_lbl_rol";
            this.frm_lbl_rol.Size = new System.Drawing.Size(55, 37);
            this.frm_lbl_rol.TabIndex = 10;
            this.frm_lbl_rol.Text = "Rol";
            // 
            // panelCrearUsu
            // 
            this.panelCrearUsu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelCrearUsu.BackColor = System.Drawing.Color.White;
            this.panelCrearUsu.Controls.Add(this.btnCrearUsuario);
            this.panelCrearUsu.Controls.Add(this.frm_lbl_nombre);
            this.panelCrearUsu.Controls.Add(this.frm_lbl_rol);
            this.panelCrearUsu.Controls.Add(this.txtNombre);
            this.panelCrearUsu.Controls.Add(this.frm_lbl_dni);
            this.panelCrearUsu.Controls.Add(this.comboBox1);
            this.panelCrearUsu.Controls.Add(this.txtApellido);
            this.panelCrearUsu.Controls.Add(this.frm_lbl_apellido);
            this.panelCrearUsu.Controls.Add(this.frm_lbl_email);
            this.panelCrearUsu.Controls.Add(this.txt_Dni);
            this.panelCrearUsu.Controls.Add(this.txtEmail);
            this.panelCrearUsu.Location = new System.Drawing.Point(85, 40);
            this.panelCrearUsu.Name = "panelCrearUsu";
            this.panelCrearUsu.Size = new System.Drawing.Size(542, 735);
            this.panelCrearUsu.TabIndex = 11;
            // 
            // FrmCrearUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(713, 815);
            this.Controls.Add(this.panelCrearUsu);
            this.Font = new System.Drawing.Font("Segoe UI", 20.25F);
            this.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.Name = "FrmCrearUsuario";
            this.Text = "FrmCrearUsuario";
            this.Load += new System.EventHandler(this.FrmCrearUsuario_Load);
            this.panelCrearUsu.ResumeLayout(false);
            this.panelCrearUsu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txt_Dni;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label frm_lbl_nombre;
        private System.Windows.Forms.Label frm_lbl_apellido;
        private System.Windows.Forms.Label frm_lbl_email;
        private System.Windows.Forms.Button btnCrearUsuario;
        private System.Windows.Forms.Label frm_lbl_dni;
        private System.Windows.Forms.Label frm_lbl_rol;
        private System.Windows.Forms.Panel panelCrearUsu;
    }
}
namespace UI
{
    partial class Login
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLogin = new System.Windows.Forms.Button();
            this.txt_userName = new System.Windows.Forms.TextBox();
            this.txt_Contrasena = new System.Windows.Forms.TextBox();
            this.lbl_Email = new System.Windows.Forms.Label();
            this.lbl_Contrasena = new System.Windows.Forms.Label();
            this.panelLogin = new System.Windows.Forms.Panel();
            this.panelLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 20.25F);
            this.btnLogin.Location = new System.Drawing.Point(117, 391);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(139, 59);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "Ingresar";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txt_userName
            // 
            this.txt_userName.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_userName.Location = new System.Drawing.Point(55, 203);
            this.txt_userName.Name = "txt_userName";
            this.txt_userName.Size = new System.Drawing.Size(263, 33);
            this.txt_userName.TabIndex = 1;
            // 
            // txt_Contrasena
            // 
            this.txt_Contrasena.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Contrasena.Location = new System.Drawing.Point(55, 314);
            this.txt_Contrasena.Name = "txt_Contrasena";
            this.txt_Contrasena.Size = new System.Drawing.Size(263, 33);
            this.txt_Contrasena.TabIndex = 2;
            // 
            // lbl_Email
            // 
            this.lbl_Email.AutoSize = true;
            this.lbl_Email.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Email.Location = new System.Drawing.Point(49, 163);
            this.lbl_Email.Name = "lbl_Email";
            this.lbl_Email.Size = new System.Drawing.Size(146, 37);
            this.lbl_Email.TabIndex = 3;
            this.lbl_Email.Text = "Username";
            // 
            // lbl_Contrasena
            // 
            this.lbl_Contrasena.AutoSize = true;
            this.lbl_Contrasena.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold);
            this.lbl_Contrasena.Location = new System.Drawing.Point(49, 274);
            this.lbl_Contrasena.Name = "lbl_Contrasena";
            this.lbl_Contrasena.Size = new System.Drawing.Size(162, 37);
            this.lbl_Contrasena.TabIndex = 4;
            this.lbl_Contrasena.Text = "Contraseña";
            // 
            // panelLogin
            // 
            this.panelLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelLogin.BackColor = System.Drawing.Color.White;
            this.panelLogin.Controls.Add(this.lbl_Contrasena);
            this.panelLogin.Controls.Add(this.txt_Contrasena);
            this.panelLogin.Controls.Add(this.btnLogin);
            this.panelLogin.Controls.Add(this.txt_userName);
            this.panelLogin.Controls.Add(this.lbl_Email);
            this.panelLogin.Location = new System.Drawing.Point(79, 52);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(372, 503);
            this.panelLogin.TabIndex = 5;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(531, 606);
            this.Controls.Add(this.panelLogin);
            this.Name = "Login";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Login_Load);
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txt_userName;
        private System.Windows.Forms.TextBox txt_Contrasena;
        private System.Windows.Forms.Label lbl_Email;
        private System.Windows.Forms.Label lbl_Contrasena;
        private System.Windows.Forms.Panel panelLogin;
    }
}


namespace UI
{
    partial class FrmConfiguracionBD_83KI
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInstance = new System.Windows.Forms.Label();
            this.cmbInstance = new System.Windows.Forms.ComboBox();
            this.btnRetryDiscovery = new System.Windows.Forms.Button();
            this.lnkInstallGuide = new System.Windows.Forms.LinkLabel();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.btnValidate = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Configuracion Inicial de Base de Datos";
            // 
            // lblInstance
            // 
            this.lblInstance.AutoSize = true;
            this.lblInstance.Location = new System.Drawing.Point(34, 70);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(134, 13);
            this.lblInstance.TabIndex = 1;
            this.lblInstance.Text = "Instancia SQL Server:";
            // 
            // cmbInstance
            // 
            this.cmbInstance.FormattingEnabled = true;
            this.cmbInstance.Location = new System.Drawing.Point(37, 90);
            this.cmbInstance.Name = "cmbInstance";
            this.cmbInstance.Size = new System.Drawing.Size(350, 21);
            this.cmbInstance.TabIndex = 2;
            // 
            // btnRetryDiscovery
            // 
            this.btnRetryDiscovery.Location = new System.Drawing.Point(397, 88);
            this.btnRetryDiscovery.Name = "btnRetryDiscovery";
            this.btnRetryDiscovery.Size = new System.Drawing.Size(70, 25);
            this.btnRetryDiscovery.TabIndex = 9;
            this.btnRetryDiscovery.Text = "Reintentar";
            this.btnRetryDiscovery.UseVisualStyleBackColor = true;
            this.btnRetryDiscovery.Click += new System.EventHandler(this.btnRetryDiscovery_Click);
            // 
            // lnkInstallGuide
            // 
            this.lnkInstallGuide.AutoSize = true;
            this.lnkInstallGuide.Location = new System.Drawing.Point(37, 130);
            this.lnkInstallGuide.Name = "lnkInstallGuide";
            this.lnkInstallGuide.Size = new System.Drawing.Size(250, 13);
            this.lnkInstallGuide.TabIndex = 10;
            this.lnkInstallGuide.TabStop = true;
            this.lnkInstallGuide.Text = "¿No tenés SQL Server? Ver guía de instalación";
            this.lnkInstallGuide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkInstallGuide_LinkClicked);
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(34, 162);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(170, 13);
            this.lblDatabase.TabIndex = 3;
            this.lblDatabase.Text = "Nombre de la Base de Datos:";
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Location = new System.Drawing.Point(37, 182);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(430, 20);
            this.txtDatabaseName.TabIndex = 4;
            // 
            // btnValidate
            // 
            this.btnValidate.Location = new System.Drawing.Point(37, 232);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(130, 30);
            this.btnValidate.TabIndex = 5;
            this.btnValidate.Text = "Validar Conexion";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(247, 232);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(100, 30);
            this.btnConfirm.TabIndex = 6;
            this.btnConfirm.Text = "Confirmar";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(367, 232);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblStatus.Location = new System.Drawing.Point(34, 282);
            this.lblStatus.MaximumSize = new System.Drawing.Size(430, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 15);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Estado:";
            // 
            // FrmConfiguracionBD_83KI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 345);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.txtDatabaseName);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.lnkInstallGuide);
            this.Controls.Add(this.btnRetryDiscovery);
            this.Controls.Add(this.cmbInstance);
            this.Controls.Add(this.lblInstance);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfiguracionBD_83KI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuracion de Base de Datos";
            this.Load += new System.EventHandler(this.FrmConfiguracionBD_83KI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.ComboBox cmbInstance;
        private System.Windows.Forms.Button btnRetryDiscovery;
        private System.Windows.Forms.LinkLabel lnkInstallGuide;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStatus;
    }
}

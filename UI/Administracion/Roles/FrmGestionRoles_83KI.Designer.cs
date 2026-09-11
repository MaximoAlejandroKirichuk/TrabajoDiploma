namespace UI
{
    partial class FrmGestionRoles_83KI
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
            this.tabPrincipal = new System.Windows.Forms.TabControl();
            this.tabCrear = new System.Windows.Forms.TabPage();
            this.lblNombreRol = new System.Windows.Forms.Label();
            this.txtNombreRol = new System.Windows.Forms.TextBox();
            this.lblFamiliasCrear = new System.Windows.Forms.Label();
            this.clbFamiliasCrear = new System.Windows.Forms.CheckedListBox();
            this.lblPatentesCrear = new System.Windows.Forms.Label();
            this.clbPatentesCrear = new System.Windows.Forms.CheckedListBox();
            this.btnCrearRol = new System.Windows.Forms.Button();
            this.btnLimpiarCreacion = new System.Windows.Forms.Button();
            this.tabGestionar = new System.Windows.Forms.TabPage();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.lblFamiliasGestion = new System.Windows.Forms.Label();
            this.cmbFamiliaAgregar = new System.Windows.Forms.ComboBox();
            this.btnAgregarFamilia = new System.Windows.Forms.Button();
            this.btnQuitarFamilia = new System.Windows.Forms.Button();
            this.lblPatentesGestion = new System.Windows.Forms.Label();
            this.cmbPatenteAgregar = new System.Windows.Forms.ComboBox();
            this.btnAsignarPatente = new System.Windows.Forms.Button();
            this.btnQuitarPatente = new System.Windows.Forms.Button();
            this.btnEliminarRol = new System.Windows.Forms.Button();
            this.treeContenidoFamilia = new System.Windows.Forms.TreeView();
            this.lblPatentesFamilia = new System.Windows.Forms.Label();
            this.lstPatentesRol = new System.Windows.Forms.ListBox();
            this.lblPatentesRol = new System.Windows.Forms.Label();
            this.lstFamiliasRol = new System.Windows.Forms.ListBox();
            this.lblFamiliasRol = new System.Windows.Forms.Label();
            this.cmbRolExistente = new System.Windows.Forms.ComboBox();
            this.lblRoles = new System.Windows.Forms.Label();
            this.tabPrincipal.SuspendLayout();
            this.tabCrear.SuspendLayout();
            this.tabGestionar.SuspendLayout();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPrincipal
            // 
            this.tabPrincipal.Controls.Add(this.tabCrear);
            this.tabPrincipal.Controls.Add(this.tabGestionar);
            this.tabPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPrincipal.Location = new System.Drawing.Point(12, 12);
            this.tabPrincipal.Name = "tabPrincipal";
            this.tabPrincipal.SelectedIndex = 0;
            this.tabPrincipal.Size = new System.Drawing.Size(1066, 520);
            this.tabPrincipal.TabIndex = 0;
            // 
            // tabCrear
            // 
            this.tabCrear.Controls.Add(this.lblNombreRol);
            this.tabCrear.Controls.Add(this.txtNombreRol);
            this.tabCrear.Controls.Add(this.lblFamiliasCrear);
            this.tabCrear.Controls.Add(this.clbFamiliasCrear);
            this.tabCrear.Controls.Add(this.lblPatentesCrear);
            this.tabCrear.Controls.Add(this.clbPatentesCrear);
            this.tabCrear.Controls.Add(this.btnCrearRol);
            this.tabCrear.Controls.Add(this.btnLimpiarCreacion);
            this.tabCrear.Location = new System.Drawing.Point(4, 30);
            this.tabCrear.Name = "tabCrear";
            this.tabCrear.Padding = new System.Windows.Forms.Padding(12);
            this.tabCrear.Size = new System.Drawing.Size(1058, 486);
            this.tabCrear.TabIndex = 0;
            this.tabCrear.Text = "Crear rol";
            this.tabCrear.UseVisualStyleBackColor = true;
            // 
            // lblNombreRol
            // 
            this.lblNombreRol.AutoSize = true;
            this.lblNombreRol.Location = new System.Drawing.Point(16, 20);
            this.lblNombreRol.Name = "lblNombreRol";
            this.lblNombreRol.Size = new System.Drawing.Size(68, 21);
            this.lblNombreRol.TabIndex = 0;
            this.lblNombreRol.Text = "Nombre";
            // 
            // txtNombreRol
            // 
            this.txtNombreRol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNombreRol.Location = new System.Drawing.Point(90, 16);
            this.txtNombreRol.Name = "txtNombreRol";
            this.txtNombreRol.Size = new System.Drawing.Size(938, 29);
            this.txtNombreRol.TabIndex = 1;
            // 
            // lblFamiliasCrear
            // 
            this.lblFamiliasCrear.AutoSize = true;
            this.lblFamiliasCrear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFamiliasCrear.Location = new System.Drawing.Point(16, 60);
            this.lblFamiliasCrear.Name = "lblFamiliasCrear";
            this.lblFamiliasCrear.Size = new System.Drawing.Size(73, 21);
            this.lblFamiliasCrear.TabIndex = 2;
            this.lblFamiliasCrear.Text = "Familias";
            // 
            // clbFamiliasCrear
            // 
            this.clbFamiliasCrear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbFamiliasCrear.CheckOnClick = true;
            this.clbFamiliasCrear.FormattingEnabled = true;
            this.clbFamiliasCrear.Location = new System.Drawing.Point(16, 88);
            this.clbFamiliasCrear.Name = "clbFamiliasCrear";
            this.clbFamiliasCrear.Size = new System.Drawing.Size(1012, 148);
            this.clbFamiliasCrear.TabIndex = 3;
            // 
            // lblPatentesCrear
            // 
            this.lblPatentesCrear.AutoSize = true;
            this.lblPatentesCrear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPatentesCrear.Location = new System.Drawing.Point(16, 250);
            this.lblPatentesCrear.Name = "lblPatentesCrear";
            this.lblPatentesCrear.Size = new System.Drawing.Size(76, 21);
            this.lblPatentesCrear.TabIndex = 4;
            this.lblPatentesCrear.Text = "Patentes";
            // 
            // clbPatentesCrear
            // 
            this.clbPatentesCrear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbPatentesCrear.CheckOnClick = true;
            this.clbPatentesCrear.FormattingEnabled = true;
            this.clbPatentesCrear.Location = new System.Drawing.Point(16, 278);
            this.clbPatentesCrear.Name = "clbPatentesCrear";
            this.clbPatentesCrear.Size = new System.Drawing.Size(1012, 148);
            this.clbPatentesCrear.TabIndex = 5;
            // 
            // btnCrearRol
            // 
            this.btnCrearRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrearRol.Location = new System.Drawing.Point(778, 440);
            this.btnCrearRol.Name = "btnCrearRol";
            this.btnCrearRol.Size = new System.Drawing.Size(130, 34);
            this.btnCrearRol.TabIndex = 6;
            this.btnCrearRol.Text = "Crear rol";
            this.btnCrearRol.UseVisualStyleBackColor = true;
            this.btnCrearRol.Click += new System.EventHandler(this.btnCrearRol_Click);
            // 
            // btnLimpiarCreacion
            // 
            this.btnLimpiarCreacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimpiarCreacion.Location = new System.Drawing.Point(918, 440);
            this.btnLimpiarCreacion.Name = "btnLimpiarCreacion";
            this.btnLimpiarCreacion.Size = new System.Drawing.Size(130, 34);
            this.btnLimpiarCreacion.TabIndex = 7;
            this.btnLimpiarCreacion.Text = "Limpiar";
            this.btnLimpiarCreacion.UseVisualStyleBackColor = true;
            this.btnLimpiarCreacion.Click += new System.EventHandler(this.btnLimpiarCreacion_Click);
            // 
            // tabGestionar
            // 
            this.tabGestionar.Controls.Add(this.pnlAcciones);
            this.tabGestionar.Controls.Add(this.treeContenidoFamilia);
            this.tabGestionar.Controls.Add(this.lblPatentesFamilia);
            this.tabGestionar.Controls.Add(this.lstPatentesRol);
            this.tabGestionar.Controls.Add(this.lblPatentesRol);
            this.tabGestionar.Controls.Add(this.lstFamiliasRol);
            this.tabGestionar.Controls.Add(this.lblFamiliasRol);
            this.tabGestionar.Controls.Add(this.cmbRolExistente);
            this.tabGestionar.Controls.Add(this.lblRoles);
            this.tabGestionar.Location = new System.Drawing.Point(4, 30);
            this.tabGestionar.Name = "tabGestionar";
            this.tabGestionar.Padding = new System.Windows.Forms.Padding(12);
            this.tabGestionar.Size = new System.Drawing.Size(1058, 486);
            this.tabGestionar.TabIndex = 1;
            this.tabGestionar.Text = "Gestionar roles";
            this.tabGestionar.UseVisualStyleBackColor = true;
            // 
            // pnlAcciones
            // 
            this.pnlAcciones.Controls.Add(this.lblFamiliasGestion);
            this.pnlAcciones.Controls.Add(this.cmbFamiliaAgregar);
            this.pnlAcciones.Controls.Add(this.btnAgregarFamilia);
            this.pnlAcciones.Controls.Add(this.btnQuitarFamilia);
            this.pnlAcciones.Controls.Add(this.lblPatentesGestion);
            this.pnlAcciones.Controls.Add(this.cmbPatenteAgregar);
            this.pnlAcciones.Controls.Add(this.btnAsignarPatente);
            this.pnlAcciones.Controls.Add(this.btnQuitarPatente);
            this.pnlAcciones.Controls.Add(this.btnEliminarRol);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAcciones.Location = new System.Drawing.Point(12, 306);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(1034, 168);
            this.pnlAcciones.TabIndex = 8;
            // 
            // lblFamiliasGestion
            // 
            this.lblFamiliasGestion.AutoEllipsis = true;
            this.lblFamiliasGestion.Location = new System.Drawing.Point(4, 9);
            this.lblFamiliasGestion.Name = "lblFamiliasGestion";
            this.lblFamiliasGestion.Size = new System.Drawing.Size(220, 21);
            this.lblFamiliasGestion.TabIndex = 0;
            this.lblFamiliasGestion.Text = "Familias disponibles";
            // 
            // cmbFamiliaAgregar
            // 
            this.cmbFamiliaAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFamiliaAgregar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFamiliaAgregar.FormattingEnabled = true;
            this.cmbFamiliaAgregar.Location = new System.Drawing.Point(8, 33);
            this.cmbFamiliaAgregar.Name = "cmbFamiliaAgregar";
            this.cmbFamiliaAgregar.Size = new System.Drawing.Size(422, 29);
            this.cmbFamiliaAgregar.TabIndex = 1;
            // 
            // btnAgregarFamilia
            // 
            this.btnAgregarFamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarFamilia.Location = new System.Drawing.Point(436, 33);
            this.btnAgregarFamilia.Name = "btnAgregarFamilia";
            this.btnAgregarFamilia.Size = new System.Drawing.Size(150, 34);
            this.btnAgregarFamilia.TabIndex = 2;
            this.btnAgregarFamilia.Text = "Agregar";
            this.btnAgregarFamilia.UseVisualStyleBackColor = true;
            this.btnAgregarFamilia.Click += new System.EventHandler(this.btnAgregarFamilia_Click);
            // 
            // btnQuitarFamilia
            // 
            this.btnQuitarFamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuitarFamilia.Location = new System.Drawing.Point(594, 33);
            this.btnQuitarFamilia.Name = "btnQuitarFamilia";
            this.btnQuitarFamilia.Size = new System.Drawing.Size(150, 34);
            this.btnQuitarFamilia.TabIndex = 3;
            this.btnQuitarFamilia.Text = "Quitar";
            this.btnQuitarFamilia.UseVisualStyleBackColor = true;
            this.btnQuitarFamilia.Click += new System.EventHandler(this.btnQuitarFamilia_Click);
            // 
            // lblPatentesGestion
            // 
            this.lblPatentesGestion.AutoEllipsis = true;
            this.lblPatentesGestion.Location = new System.Drawing.Point(4, 107);
            this.lblPatentesGestion.Name = "lblPatentesGestion";
            this.lblPatentesGestion.Size = new System.Drawing.Size(220, 21);
            this.lblPatentesGestion.TabIndex = 4;
            this.lblPatentesGestion.Text = "Patentes disponibles";
            // 
            // cmbPatenteAgregar
            // 
            this.cmbPatenteAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPatenteAgregar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatenteAgregar.FormattingEnabled = true;
            this.cmbPatenteAgregar.Location = new System.Drawing.Point(8, 131);
            this.cmbPatenteAgregar.Name = "cmbPatenteAgregar";
            this.cmbPatenteAgregar.Size = new System.Drawing.Size(422, 29);
            this.cmbPatenteAgregar.TabIndex = 5;
            // 
            // btnAsignarPatente
            // 
            this.btnAsignarPatente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAsignarPatente.Location = new System.Drawing.Point(436, 131);
            this.btnAsignarPatente.Name = "btnAsignarPatente";
            this.btnAsignarPatente.Size = new System.Drawing.Size(150, 34);
            this.btnAsignarPatente.TabIndex = 6;
            this.btnAsignarPatente.Text = "Asignar";
            this.btnAsignarPatente.UseVisualStyleBackColor = true;
            this.btnAsignarPatente.Visible = false;
            this.btnAsignarPatente.Click += new System.EventHandler(this.btnAsignarPatente_Click);
            // 
            // btnQuitarPatente
            // 
            this.btnQuitarPatente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuitarPatente.Location = new System.Drawing.Point(594, 131);
            this.btnQuitarPatente.Name = "btnQuitarPatente";
            this.btnQuitarPatente.Size = new System.Drawing.Size(150, 34);
            this.btnQuitarPatente.TabIndex = 7;
            this.btnQuitarPatente.Text = "Quitar";
            this.btnQuitarPatente.UseVisualStyleBackColor = true;
            this.btnQuitarPatente.Visible = false;
            this.btnQuitarPatente.Click += new System.EventHandler(this.btnQuitarPatente_Click);
            // 
            // btnEliminarRol
            // 
            this.btnEliminarRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarRol.Location = new System.Drawing.Point(880, 28);
            this.btnEliminarRol.Name = "btnEliminarRol";
            this.btnEliminarRol.Size = new System.Drawing.Size(150, 34);
            this.btnEliminarRol.TabIndex = 8;
            this.btnEliminarRol.Text = "Eliminar rol";
            this.btnEliminarRol.UseVisualStyleBackColor = true;
            this.btnEliminarRol.Click += new System.EventHandler(this.btnEliminarRol_Click);
            // 
            // treeContenidoFamilia
            // 
            this.treeContenidoFamilia.Location = new System.Drawing.Point(290, 118);
            this.treeContenidoFamilia.Name = "treeContenidoFamilia";
            this.treeContenidoFamilia.Size = new System.Drawing.Size(240, 170);
            this.treeContenidoFamilia.TabIndex = 5;
            // 
            // lblPatentesFamilia
            // 
            this.lblPatentesFamilia.AutoSize = true;
            this.lblPatentesFamilia.Location = new System.Drawing.Point(290, 90);
            this.lblPatentesFamilia.Name = "lblPatentesFamilia";
            this.lblPatentesFamilia.Size = new System.Drawing.Size(170, 21);
            this.lblPatentesFamilia.TabIndex = 4;
            this.lblPatentesFamilia.Text = "Contenido de la familia";
            // 
            // lstPatentesRol
            // 
            this.lstPatentesRol.FormattingEnabled = true;
            this.lstPatentesRol.ItemHeight = 21;
            this.lstPatentesRol.Location = new System.Drawing.Point(560, 118);
            this.lstPatentesRol.Name = "lstPatentesRol";
            this.lstPatentesRol.Size = new System.Drawing.Size(240, 151);
            this.lstPatentesRol.TabIndex = 7;
            // 
            // lblPatentesRol
            // 
            this.lblPatentesRol.AutoSize = true;
            this.lblPatentesRol.Location = new System.Drawing.Point(560, 90);
            this.lblPatentesRol.Name = "lblPatentesRol";
            this.lblPatentesRol.Size = new System.Drawing.Size(116, 21);
            this.lblPatentesRol.TabIndex = 6;
            this.lblPatentesRol.Text = "Patentes del rol";
            // 
            // lstFamiliasRol
            // 
            this.lstFamiliasRol.FormattingEnabled = true;
            this.lstFamiliasRol.ItemHeight = 21;
            this.lstFamiliasRol.Location = new System.Drawing.Point(16, 118);
            this.lstFamiliasRol.Name = "lstFamiliasRol";
            this.lstFamiliasRol.Size = new System.Drawing.Size(240, 151);
            this.lstFamiliasRol.TabIndex = 3;
            this.lstFamiliasRol.SelectedIndexChanged += new System.EventHandler(this.lstFamiliasRol_SelectedIndexChanged);
            // 
            // lblFamiliasRol
            // 
            this.lblFamiliasRol.AutoSize = true;
            this.lblFamiliasRol.Location = new System.Drawing.Point(16, 90);
            this.lblFamiliasRol.Name = "lblFamiliasRol";
            this.lblFamiliasRol.Size = new System.Drawing.Size(114, 21);
            this.lblFamiliasRol.TabIndex = 2;
            this.lblFamiliasRol.Text = "Familias del rol";
            // 
            // cmbRolExistente
            // 
            this.cmbRolExistente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRolExistente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRolExistente.FormattingEnabled = true;
            this.cmbRolExistente.Location = new System.Drawing.Point(16, 48);
            this.cmbRolExistente.Name = "cmbRolExistente";
            this.cmbRolExistente.Size = new System.Drawing.Size(1026, 29);
            this.cmbRolExistente.TabIndex = 1;
            this.cmbRolExistente.SelectedIndexChanged += new System.EventHandler(this.cmbRolExistente_SelectedIndexChanged);
            // 
            // lblRoles
            // 
            this.lblRoles.AutoSize = true;
            this.lblRoles.Location = new System.Drawing.Point(16, 20);
            this.lblRoles.Name = "lblRoles";
            this.lblRoles.Size = new System.Drawing.Size(48, 21);
            this.lblRoles.TabIndex = 0;
            this.lblRoles.Text = "Roles";
            // 
            // FrmGestionRoles_83KI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(1090, 544);
            this.Controls.Add(this.tabPrincipal);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FrmGestionRoles_83KI";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Text = "Gestion de roles";
            this.Load += new System.EventHandler(this.FrmGestionRoles_83KI_Load);
            this.tabPrincipal.ResumeLayout(false);
            this.tabCrear.ResumeLayout(false);
            this.tabCrear.PerformLayout();
            this.tabGestionar.ResumeLayout(false);
            this.tabGestionar.PerformLayout();
            this.pnlAcciones.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabPrincipal;
        private System.Windows.Forms.TabPage tabCrear;
        private System.Windows.Forms.TabPage tabGestionar;

        // Crear tab
        private System.Windows.Forms.Label lblNombreRol;
        private System.Windows.Forms.TextBox txtNombreRol;
        private System.Windows.Forms.Label lblFamiliasCrear;
        private System.Windows.Forms.CheckedListBox clbFamiliasCrear;
        private System.Windows.Forms.Label lblPatentesCrear;
        private System.Windows.Forms.CheckedListBox clbPatentesCrear;
        private System.Windows.Forms.Button btnCrearRol;
        private System.Windows.Forms.Button btnLimpiarCreacion;

        // Gestionar tab
        private System.Windows.Forms.Label lblRoles;
        private System.Windows.Forms.ComboBox cmbRolExistente;
        private System.Windows.Forms.Label lblFamiliasRol;
        private System.Windows.Forms.ListBox lstFamiliasRol;
        private System.Windows.Forms.Label lblPatentesFamilia;
        private System.Windows.Forms.TreeView treeContenidoFamilia;
        private System.Windows.Forms.Label lblPatentesRol;
        private System.Windows.Forms.ListBox lstPatentesRol;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Label lblFamiliasGestion;
        private System.Windows.Forms.ComboBox cmbFamiliaAgregar;
        private System.Windows.Forms.Button btnAgregarFamilia;
        private System.Windows.Forms.Button btnQuitarFamilia;
        private System.Windows.Forms.Label lblPatentesGestion;
        private System.Windows.Forms.ComboBox cmbPatenteAgregar;
        private System.Windows.Forms.Button btnAsignarPatente;
        private System.Windows.Forms.Button btnQuitarPatente;
        private System.Windows.Forms.Button btnEliminarRol;
    }
}

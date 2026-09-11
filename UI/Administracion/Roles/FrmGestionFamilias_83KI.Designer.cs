namespace UI
{
    partial class FrmGestionFamilias_83KI
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
            this.btnLimpiarCreacion = new System.Windows.Forms.Button();
            this.btnCrearFamilia = new System.Windows.Forms.Button();
            this.clbFamiliasCreacion = new System.Windows.Forms.CheckedListBox();
            this.lblSubfamilias = new System.Windows.Forms.Label();
            this.lstPatentesDisponibles = new System.Windows.Forms.CheckedListBox();
            this.lblPatentes = new System.Windows.Forms.Label();
            this.txtNombreFamilia = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.tabGestionar = new System.Windows.Forms.TabPage();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.lblPatentesGestion = new System.Windows.Forms.Label();
            this.cmbPatenteAgregar = new System.Windows.Forms.ComboBox();
            this.btnAgregarPatente = new System.Windows.Forms.Button();
            this.btnQuitarPatente = new System.Windows.Forms.Button();
            this.lblSubfamiliasGestion = new System.Windows.Forms.Label();
            this.cmbSubfamiliaAgregar = new System.Windows.Forms.ComboBox();
            this.btnAgregarSubfamilia = new System.Windows.Forms.Button();
            this.btnQuitarSubfamilia = new System.Windows.Forms.Button();
            this.btnEliminarFamilia = new System.Windows.Forms.Button();
            this.treeFamilia = new System.Windows.Forms.TreeView();
            this.lblDetalleFamilia = new System.Windows.Forms.Label();
            this.cmbFamiliaExistente = new System.Windows.Forms.ComboBox();
            this.lblFamiliasExistentes = new System.Windows.Forms.Label();
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
            this.tabPrincipal.Size = new System.Drawing.Size(790, 777);
            this.tabPrincipal.TabIndex = 0;
            // 
            // tabCrear
            // 
            this.tabCrear.Controls.Add(this.btnLimpiarCreacion);
            this.tabCrear.Controls.Add(this.btnCrearFamilia);
            this.tabCrear.Controls.Add(this.clbFamiliasCreacion);
            this.tabCrear.Controls.Add(this.lblSubfamilias);
            this.tabCrear.Controls.Add(this.lstPatentesDisponibles);
            this.tabCrear.Controls.Add(this.lblPatentes);
            this.tabCrear.Controls.Add(this.txtNombreFamilia);
            this.tabCrear.Controls.Add(this.lblNombre);
            this.tabCrear.Location = new System.Drawing.Point(4, 30);
            this.tabCrear.Name = "tabCrear";
            this.tabCrear.Padding = new System.Windows.Forms.Padding(12);
            this.tabCrear.Size = new System.Drawing.Size(768, 622);
            this.tabCrear.TabIndex = 0;
            this.tabCrear.Text = "Crear familia";
            this.tabCrear.UseVisualStyleBackColor = true;
            // 
            // btnLimpiarCreacion
            // 
            this.btnLimpiarCreacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimpiarCreacion.Location = new System.Drawing.Point(622, 440);
            this.btnLimpiarCreacion.Name = "btnLimpiarCreacion";
            this.btnLimpiarCreacion.Size = new System.Drawing.Size(130, 34);
            this.btnLimpiarCreacion.TabIndex = 7;
            this.btnLimpiarCreacion.Text = "Limpiar";
            this.btnLimpiarCreacion.UseVisualStyleBackColor = true;
            this.btnLimpiarCreacion.Click += new System.EventHandler(this.btnLimpiarCreacion_Click);
            // 
            // btnCrearFamilia
            // 
            this.btnCrearFamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrearFamilia.Location = new System.Drawing.Point(482, 440);
            this.btnCrearFamilia.Name = "btnCrearFamilia";
            this.btnCrearFamilia.Size = new System.Drawing.Size(130, 34);
            this.btnCrearFamilia.TabIndex = 6;
            this.btnCrearFamilia.Text = "Guardar";
            this.btnCrearFamilia.UseVisualStyleBackColor = true;
            this.btnCrearFamilia.Click += new System.EventHandler(this.btnCrearFamilia_Click);
            // 
            // clbFamiliasCreacion
            // 
            this.clbFamiliasCreacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbFamiliasCreacion.CheckOnClick = true;
            this.clbFamiliasCreacion.FormattingEnabled = true;
            this.clbFamiliasCreacion.Location = new System.Drawing.Point(16, 278);
            this.clbFamiliasCreacion.Name = "clbFamiliasCreacion";
            this.clbFamiliasCreacion.Size = new System.Drawing.Size(736, 148);
            this.clbFamiliasCreacion.TabIndex = 5;
            // 
            // lblSubfamilias
            // 
            this.lblSubfamilias.AutoSize = true;
            this.lblSubfamilias.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSubfamilias.Location = new System.Drawing.Point(16, 250);
            this.lblSubfamilias.Name = "lblSubfamilias";
            this.lblSubfamilias.Size = new System.Drawing.Size(100, 21);
            this.lblSubfamilias.TabIndex = 4;
            this.lblSubfamilias.Text = "Subfamilias";
            // 
            // lstPatentesDisponibles
            // 
            this.lstPatentesDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPatentesDisponibles.CheckOnClick = true;
            this.lstPatentesDisponibles.FormattingEnabled = true;
            this.lstPatentesDisponibles.Location = new System.Drawing.Point(16, 88);
            this.lstPatentesDisponibles.Name = "lstPatentesDisponibles";
            this.lstPatentesDisponibles.Size = new System.Drawing.Size(736, 148);
            this.lstPatentesDisponibles.TabIndex = 3;
            // 
            // lblPatentes
            // 
            this.lblPatentes.AutoSize = true;
            this.lblPatentes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPatentes.Location = new System.Drawing.Point(16, 60);
            this.lblPatentes.Name = "lblPatentes";
            this.lblPatentes.Size = new System.Drawing.Size(76, 21);
            this.lblPatentes.TabIndex = 2;
            this.lblPatentes.Text = "Patentes";
            // 
            // txtNombreFamilia
            // 
            this.txtNombreFamilia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNombreFamilia.Location = new System.Drawing.Point(90, 16);
            this.txtNombreFamilia.Name = "txtNombreFamilia";
            this.txtNombreFamilia.Size = new System.Drawing.Size(662, 29);
            this.txtNombreFamilia.TabIndex = 1;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(16, 20);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(68, 21);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre";
            // 
            // tabGestionar
            // 
            this.tabGestionar.Controls.Add(this.pnlAcciones);
            this.tabGestionar.Controls.Add(this.treeFamilia);
            this.tabGestionar.Controls.Add(this.lblDetalleFamilia);
            this.tabGestionar.Controls.Add(this.cmbFamiliaExistente);
            this.tabGestionar.Controls.Add(this.lblFamiliasExistentes);
            this.tabGestionar.Location = new System.Drawing.Point(4, 30);
            this.tabGestionar.Name = "tabGestionar";
            this.tabGestionar.Padding = new System.Windows.Forms.Padding(12);
            this.tabGestionar.Size = new System.Drawing.Size(782, 743);
            this.tabGestionar.TabIndex = 1;
            this.tabGestionar.Text = "Gestionar familias";
            this.tabGestionar.UseVisualStyleBackColor = true;
            // 
            // pnlAcciones
            // 
            this.pnlAcciones.Controls.Add(this.lblPatentesGestion);
            this.pnlAcciones.Controls.Add(this.cmbPatenteAgregar);
            this.pnlAcciones.Controls.Add(this.btnAgregarPatente);
            this.pnlAcciones.Controls.Add(this.btnQuitarPatente);
            this.pnlAcciones.Controls.Add(this.lblSubfamiliasGestion);
            this.pnlAcciones.Controls.Add(this.cmbSubfamiliaAgregar);
            this.pnlAcciones.Controls.Add(this.btnAgregarSubfamilia);
            this.pnlAcciones.Controls.Add(this.btnQuitarSubfamilia);
            this.pnlAcciones.Controls.Add(this.btnEliminarFamilia);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAcciones.Location = new System.Drawing.Point(12, 581);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(758, 150);
            this.pnlAcciones.TabIndex = 4;
            // 
            // lblPatentesGestion
            // 
            this.lblPatentesGestion.AutoEllipsis = true;
            this.lblPatentesGestion.Location = new System.Drawing.Point(4, 0);
            this.lblPatentesGestion.Name = "lblPatentesGestion";
            this.lblPatentesGestion.Size = new System.Drawing.Size(220, 21);
            this.lblPatentesGestion.TabIndex = 0;
            this.lblPatentesGestion.Text = "Patentes";
            // 
            // cmbPatenteAgregar
            // 
            this.cmbPatenteAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPatenteAgregar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatenteAgregar.FormattingEnabled = true;
            this.cmbPatenteAgregar.Location = new System.Drawing.Point(8, 24);
            this.cmbPatenteAgregar.Name = "cmbPatenteAgregar";
            this.cmbPatenteAgregar.Size = new System.Drawing.Size(422, 29);
            this.cmbPatenteAgregar.TabIndex = 1;
            // 
            // btnAgregarPatente
            // 
            this.btnAgregarPatente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarPatente.Location = new System.Drawing.Point(436, 20);
            this.btnAgregarPatente.Name = "btnAgregarPatente";
            this.btnAgregarPatente.Size = new System.Drawing.Size(150, 34);
            this.btnAgregarPatente.TabIndex = 2;
            this.btnAgregarPatente.Text = "Agregar patente";
            this.btnAgregarPatente.UseVisualStyleBackColor = true;
            this.btnAgregarPatente.Click += new System.EventHandler(this.btnAgregarPatente_Click);
            // 
            // btnQuitarPatente
            // 
            this.btnQuitarPatente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuitarPatente.Location = new System.Drawing.Point(594, 20);
            this.btnQuitarPatente.Name = "btnQuitarPatente";
            this.btnQuitarPatente.Size = new System.Drawing.Size(150, 34);
            this.btnQuitarPatente.TabIndex = 3;
            this.btnQuitarPatente.Text = "Quitar patente";
            this.btnQuitarPatente.UseVisualStyleBackColor = true;
            this.btnQuitarPatente.Click += new System.EventHandler(this.btnQuitarPatente_Click);
            // 
            // lblSubfamiliasGestion
            // 
            this.lblSubfamiliasGestion.AutoEllipsis = true;
            this.lblSubfamiliasGestion.Location = new System.Drawing.Point(4, 52);
            this.lblSubfamiliasGestion.Name = "lblSubfamiliasGestion";
            this.lblSubfamiliasGestion.Size = new System.Drawing.Size(220, 21);
            this.lblSubfamiliasGestion.TabIndex = 4;
            this.lblSubfamiliasGestion.Text = "Subfamilias";
            // 
            // cmbSubfamiliaAgregar
            // 
            this.cmbSubfamiliaAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSubfamiliaAgregar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubfamiliaAgregar.FormattingEnabled = true;
            this.cmbSubfamiliaAgregar.Location = new System.Drawing.Point(8, 76);
            this.cmbSubfamiliaAgregar.Name = "cmbSubfamiliaAgregar";
            this.cmbSubfamiliaAgregar.Size = new System.Drawing.Size(422, 29);
            this.cmbSubfamiliaAgregar.TabIndex = 5;
            // 
            // btnAgregarSubfamilia
            // 
            this.btnAgregarSubfamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarSubfamilia.Location = new System.Drawing.Point(438, 72);
            this.btnAgregarSubfamilia.Name = "btnAgregarSubfamilia";
            this.btnAgregarSubfamilia.Size = new System.Drawing.Size(150, 34);
            this.btnAgregarSubfamilia.TabIndex = 6;
            this.btnAgregarSubfamilia.Text = "Agregar subfamilia";
            this.btnAgregarSubfamilia.UseVisualStyleBackColor = true;
            this.btnAgregarSubfamilia.Click += new System.EventHandler(this.btnAgregarSubfamilia_Click);
            // 
            // btnQuitarSubfamilia
            // 
            this.btnQuitarSubfamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuitarSubfamilia.Location = new System.Drawing.Point(594, 72);
            this.btnQuitarSubfamilia.Name = "btnQuitarSubfamilia";
            this.btnQuitarSubfamilia.Size = new System.Drawing.Size(150, 34);
            this.btnQuitarSubfamilia.TabIndex = 7;
            this.btnQuitarSubfamilia.Text = "Quitar subfamilia";
            this.btnQuitarSubfamilia.UseVisualStyleBackColor = true;
            this.btnQuitarSubfamilia.Click += new System.EventHandler(this.btnQuitarSubfamilia_Click);
            // 
            // btnEliminarFamilia
            // 
            this.btnEliminarFamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminarFamilia.Location = new System.Drawing.Point(594, 112);
            this.btnEliminarFamilia.Name = "btnEliminarFamilia";
            this.btnEliminarFamilia.Size = new System.Drawing.Size(150, 34);
            this.btnEliminarFamilia.TabIndex = 8;
            this.btnEliminarFamilia.Text = "Eliminar familia";
            this.btnEliminarFamilia.UseVisualStyleBackColor = true;
            this.btnEliminarFamilia.Click += new System.EventHandler(this.btnEliminarFamilia_Click);
            // 
            // treeFamilia
            // 
            this.treeFamilia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeFamilia.Location = new System.Drawing.Point(12, 104);
            this.treeFamilia.Name = "treeFamilia";
            this.treeFamilia.Size = new System.Drawing.Size(758, 485);
            this.treeFamilia.TabIndex = 3;
            this.treeFamilia.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFamilia_AfterSelect);
            // 
            // lblDetalleFamilia
            // 
            this.lblDetalleFamilia.AutoSize = true;
            this.lblDetalleFamilia.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblDetalleFamilia.Location = new System.Drawing.Point(16, 80);
            this.lblDetalleFamilia.Name = "lblDetalleFamilia";
            this.lblDetalleFamilia.Size = new System.Drawing.Size(164, 21);
            this.lblDetalleFamilia.TabIndex = 2;
            this.lblDetalleFamilia.Text = "Detalle de la familia";
            // 
            // cmbFamiliaExistente
            // 
            this.cmbFamiliaExistente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFamiliaExistente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFamiliaExistente.FormattingEnabled = true;
            this.cmbFamiliaExistente.Location = new System.Drawing.Point(16, 48);
            this.cmbFamiliaExistente.Name = "cmbFamiliaExistente";
            this.cmbFamiliaExistente.Size = new System.Drawing.Size(750, 29);
            this.cmbFamiliaExistente.TabIndex = 1;
            this.cmbFamiliaExistente.SelectedIndexChanged += new System.EventHandler(this.cmbFamiliaExistente_SelectedIndexChanged);
            // 
            // lblFamiliasExistentes
            // 
            this.lblFamiliasExistentes.AutoSize = true;
            this.lblFamiliasExistentes.Location = new System.Drawing.Point(16, 20);
            this.lblFamiliasExistentes.Name = "lblFamiliasExistentes";
            this.lblFamiliasExistentes.Size = new System.Drawing.Size(205, 21);
            this.lblFamiliasExistentes.TabIndex = 0;
            this.lblFamiliasExistentes.Text = "Seleccionar familia existente";
            // 
            // FrmGestionFamilias_83KI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(814, 801);
            this.Controls.Add(this.tabPrincipal);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.MinimumSize = new System.Drawing.Size(680, 520);
            this.Name = "FrmGestionFamilias_83KI";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Text = "Gestion de familias";
            this.Load += new System.EventHandler(this.FrmGestionFamilias_83KI_Load);
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
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombreFamilia;
        private System.Windows.Forms.Label lblPatentes;
        private System.Windows.Forms.CheckedListBox lstPatentesDisponibles;
        private System.Windows.Forms.Label lblSubfamilias;
        private System.Windows.Forms.CheckedListBox clbFamiliasCreacion;
        private System.Windows.Forms.Button btnCrearFamilia;
        private System.Windows.Forms.Button btnLimpiarCreacion;
        private System.Windows.Forms.Label lblFamiliasExistentes;
        private System.Windows.Forms.ComboBox cmbFamiliaExistente;
        private System.Windows.Forms.Label lblDetalleFamilia;
        private System.Windows.Forms.TreeView treeFamilia;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Label lblPatentesGestion;
        private System.Windows.Forms.ComboBox cmbPatenteAgregar;
        private System.Windows.Forms.Button btnAgregarPatente;
        private System.Windows.Forms.Button btnQuitarPatente;
        private System.Windows.Forms.Label lblSubfamiliasGestion;
        private System.Windows.Forms.ComboBox cmbSubfamiliaAgregar;
        private System.Windows.Forms.Button btnAgregarSubfamilia;
        private System.Windows.Forms.Button btnQuitarSubfamilia;
        private System.Windows.Forms.Button btnEliminarFamilia;
    }
}

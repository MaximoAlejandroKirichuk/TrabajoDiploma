namespace UI
{
    partial class FrmPrincipal
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.iniciarSesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCerrarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.cambiarContraseñaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionDeUsuariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionDeFamiliasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionDeRolesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bitacoraEventosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recuperacionIntegridadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maestrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionCursosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionProfesoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionCursoProfesorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.planificacionAcademicaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cobrosMorosidadActasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIdioma = new System.Windows.Forms.ToolStripMenuItem();
            this.espanolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inglesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSesion,
            this.adminToolStripMenuItem,
            this.maestrosToolStripMenuItem,
            this.planificacionAcademicaToolStripMenuItem,
            this.cobrosMorosidadActasToolStripMenuItem,
            this.reportesToolStripMenuItem,
            this.menuIdioma,
            this.reToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1135, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuSesion
            // 
            this.menuSesion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iniciarSesionToolStripMenuItem,
            this.menuCerrarSesion,
            this.cambiarContraseñaToolStripMenuItem});
            this.menuSesion.Name = "menuSesion";
            this.menuSesion.Size = new System.Drawing.Size(75, 24);
            this.menuSesion.Text = "Usuario";
            this.menuSesion.Click += new System.EventHandler(this.menuSesion_Click);
            // 
            // iniciarSesionToolStripMenuItem
            // 
            this.iniciarSesionToolStripMenuItem.Name = "iniciarSesionToolStripMenuItem";
            this.iniciarSesionToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
            this.iniciarSesionToolStripMenuItem.Text = "Iniciar sesion";
            this.iniciarSesionToolStripMenuItem.Click += new System.EventHandler(this.iniciarSesionToolStripMenuItem_Click);
            // 
            // menuCerrarSesion
            // 
            this.menuCerrarSesion.Name = "menuCerrarSesion";
            this.menuCerrarSesion.Size = new System.Drawing.Size(217, 24);
            this.menuCerrarSesion.Text = "Cerrar sesión";
            this.menuCerrarSesion.Click += new System.EventHandler(this.menuCerrarSesion_Click);
            // 
            // cambiarContraseñaToolStripMenuItem
            // 
            this.cambiarContraseñaToolStripMenuItem.Name = "cambiarContraseñaToolStripMenuItem";
            this.cambiarContraseñaToolStripMenuItem.Size = new System.Drawing.Size(217, 24);
            this.cambiarContraseñaToolStripMenuItem.Text = "Cambiar contraseña";
            this.cambiarContraseñaToolStripMenuItem.Click += new System.EventHandler(this.cambiarContraseñaToolStripMenuItem_Click);
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gestionDeUsuariosToolStripMenuItem,
            this.gestionDeFamiliasToolStripMenuItem,
            this.gestionDeRolesToolStripMenuItem,
            this.bitacoraEventosToolStripMenuItem,
            this.recuperacionIntegridadToolStripMenuItem});
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.adminToolStripMenuItem.Text = "Admin";
            // 
            // gestionDeUsuariosToolStripMenuItem
            // 
            this.gestionDeUsuariosToolStripMenuItem.Name = "gestionDeUsuariosToolStripMenuItem";
            this.gestionDeUsuariosToolStripMenuItem.Size = new System.Drawing.Size(269, 24);
            this.gestionDeUsuariosToolStripMenuItem.Text = "Gestion de usuarios";
            this.gestionDeUsuariosToolStripMenuItem.Click += new System.EventHandler(this.gestionDeUsuariosToolStripMenuItem_Click);
            // 
            // gestionDeFamiliasToolStripMenuItem
            // 
            this.gestionDeFamiliasToolStripMenuItem.Name = "gestionDeFamiliasToolStripMenuItem";
            this.gestionDeFamiliasToolStripMenuItem.Size = new System.Drawing.Size(269, 24);
            this.gestionDeFamiliasToolStripMenuItem.Text = "Gestion de familias";
            this.gestionDeFamiliasToolStripMenuItem.Click += new System.EventHandler(this.gestionDeFamiliasToolStripMenuItem_Click);
            // 
            // gestionDeRolesToolStripMenuItem
            // 
            this.gestionDeRolesToolStripMenuItem.Name = "gestionDeRolesToolStripMenuItem";
            this.gestionDeRolesToolStripMenuItem.Size = new System.Drawing.Size(269, 24);
            this.gestionDeRolesToolStripMenuItem.Text = "Gestion de roles";
            this.gestionDeRolesToolStripMenuItem.Click += new System.EventHandler(this.gestionDeRolesToolStripMenuItem_Click);
            // 
            // bitacoraEventosToolStripMenuItem
            // 
            this.bitacoraEventosToolStripMenuItem.Name = "bitacoraEventosToolStripMenuItem";
            this.bitacoraEventosToolStripMenuItem.Size = new System.Drawing.Size(269, 24);
            this.bitacoraEventosToolStripMenuItem.Text = "Bitacora eventos";
            this.bitacoraEventosToolStripMenuItem.Click += new System.EventHandler(this.bitacoraEventosToolStripMenuItem_Click);
            // 
            // recuperacionIntegridadToolStripMenuItem
            // 
            this.recuperacionIntegridadToolStripMenuItem.Name = "recuperacionIntegridadToolStripMenuItem";
            this.recuperacionIntegridadToolStripMenuItem.Size = new System.Drawing.Size(269, 24);
            this.recuperacionIntegridadToolStripMenuItem.Text = "Recuperacion de integridad";
            this.recuperacionIntegridadToolStripMenuItem.Click += new System.EventHandler(this.recuperacionIntegridadToolStripMenuItem_Click);
            // 
            // maestrosToolStripMenuItem
            // 
            this.maestrosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gestionCursosToolStripMenuItem,
            this.gestionProfesoresToolStripMenuItem,
            this.gestionCursoProfesorToolStripMenuItem});
            this.maestrosToolStripMenuItem.Name = "maestrosToolStripMenuItem";
            this.maestrosToolStripMenuItem.Size = new System.Drawing.Size(86, 24);
            this.maestrosToolStripMenuItem.Text = "Maestros";
            // 
            // gestionCursosToolStripMenuItem
            // 
            this.gestionCursosToolStripMenuItem.Name = "gestionCursosToolStripMenuItem";
            this.gestionCursosToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.gestionCursosToolStripMenuItem.Text = "Gestion de cursos";
            this.gestionCursosToolStripMenuItem.Click += new System.EventHandler(this.gestionCursosToolStripMenuItem_Click);
            // 
            // gestionProfesoresToolStripMenuItem
            // 
            this.gestionProfesoresToolStripMenuItem.Name = "gestionProfesoresToolStripMenuItem";
            this.gestionProfesoresToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.gestionProfesoresToolStripMenuItem.Text = "Gestion de profesores";
            this.gestionProfesoresToolStripMenuItem.Click += new System.EventHandler(this.gestionProfesoresToolStripMenuItem_Click);
            // 
            // gestionCursoProfesorToolStripMenuItem
            // 
            this.gestionCursoProfesorToolStripMenuItem.Name = "gestionCursoProfesorToolStripMenuItem";
            this.gestionCursoProfesorToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.gestionCursoProfesorToolStripMenuItem.Text = "Cursos por profesor";
            this.gestionCursoProfesorToolStripMenuItem.Click += new System.EventHandler(this.gestionCursoProfesorToolStripMenuItem_Click);
            // 
            // planificacionAcademicaToolStripMenuItem
            // 
            this.planificacionAcademicaToolStripMenuItem.Name = "planificacionAcademicaToolStripMenuItem";
            this.planificacionAcademicaToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.planificacionAcademicaToolStripMenuItem.Text = "Planificación Académica";
            this.planificacionAcademicaToolStripMenuItem.Click += new System.EventHandler(this.planificacionAcademicaToolStripMenuItem_Click);
            // 
            // cobrosMorosidadActasToolStripMenuItem
            // 
            this.cobrosMorosidadActasToolStripMenuItem.Name = "cobrosMorosidadActasToolStripMenuItem";
            this.cobrosMorosidadActasToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
            this.cobrosMorosidadActasToolStripMenuItem.Text = "Cobros, Morosidad y Actas";
            // 
            // reportesToolStripMenuItem
            // 
            this.reportesToolStripMenuItem.Name = "reportesToolStripMenuItem";
            this.reportesToolStripMenuItem.Size = new System.Drawing.Size(84, 24);
            this.reportesToolStripMenuItem.Text = "Reportes";
            // 
            // menuIdioma
            // 
            this.menuIdioma.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.espanolToolStripMenuItem,
            this.inglesToolStripMenuItem});
            this.menuIdioma.Name = "menuIdioma";
            this.menuIdioma.Size = new System.Drawing.Size(70, 24);
            this.menuIdioma.Text = "Idioma";
            // 
            // espanolToolStripMenuItem
            // 
            this.espanolToolStripMenuItem.Name = "espanolToolStripMenuItem";
            this.espanolToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.espanolToolStripMenuItem.Text = "Español";
            this.espanolToolStripMenuItem.Click += new System.EventHandler(this.espanolToolStripMenuItem_Click);
            // 
            // inglesToolStripMenuItem
            // 
            this.inglesToolStripMenuItem.Name = "inglesToolStripMenuItem";
            this.inglesToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.inglesToolStripMenuItem.Text = "Inglés";
            this.inglesToolStripMenuItem.Click += new System.EventHandler(this.inglesToolStripMenuItem_Click);
            // 
            // reToolStripMenuItem
            // 
            this.reToolStripMenuItem.Name = "reToolStripMenuItem";
            this.reToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.reToolStripMenuItem.Text = "Ayuda";
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(1135, 631);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 20.25F);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.Name = "FrmPrincipal";
            this.Text = "FrmPrincipal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrincipal_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuSesion;
        private System.Windows.Forms.ToolStripMenuItem menuCerrarSesion;
        private System.Windows.Forms.ToolStripMenuItem iniciarSesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adminToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionDeUsuariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionDeFamiliasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionDeRolesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bitacoraEventosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuIdioma;
        private System.Windows.Forms.ToolStripMenuItem espanolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inglesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cambiarContraseñaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recuperacionIntegridadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maestrosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem planificacionAcademicaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cobrosMorosidadActasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionProfesoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionCursosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionCursoProfesorToolStripMenuItem;
    }
}

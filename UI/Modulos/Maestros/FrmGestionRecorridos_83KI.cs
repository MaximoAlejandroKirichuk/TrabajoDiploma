using BE.DTOs.Recorridos;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UI.Modulos.Maestros
{
    public sealed class FrmGestionRecorridos_83KI : FormConAyuda_83KI, IObservadorIdioma
    {
        private readonly IRecorridoCursoBLL_83KI _bll;
        private readonly IGestorIdioma_83KI _idioma = ServiceFactory_83KI.GetGestorIdioma();
        private readonly DataGridView _lista = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private readonly ComboBox _previo = Combo(), _siguiente = Combo(), _referencia = Combo();
        private readonly NumericUpDown _orden = new NumericUpDown { Minimum = 1, Maximum = 999, Width = 65 };
        private readonly NumericUpDown _minimo = new NumericUpDown { Minimum = 0, Maximum = 10000, Width = 75 };
        private readonly Dictionary<string, Control> _textos = new Dictionary<string, Control>();
        private int _id;
        public FrmGestionRecorridos_83KI(IRecorridoCursoBLL_83KI bll)
        {
            _bll = bll; ClientSize = new Size(1100,620); MinimumSize = new Size(900,500);
            Font = new Font("Segoe UI",9F); StartPosition = FormStartPosition.CenterParent;
            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 110, Padding = new Padding(8), AutoScroll = true };
            Etiqueta(editor,"Recorridos.Previo"); editor.Controls.Add(_previo);
            Etiqueta(editor,"Recorridos.Siguiente"); editor.Controls.Add(_siguiente);
            Etiqueta(editor,"Recorridos.Orden"); editor.Controls.Add(_orden);
            Boton(editor,"Recorridos.Nuevo", IniciarAlta);
            Boton(editor,"Recorridos.Guardar", () => { _bll.Guardar(_id,Id(_previo),Id(_siguiente),(int)_orden.Value); _id=0; Cargar(); });
            Boton(editor,"Recorridos.Eliminar", () =>
            {
                if (_id <= 0) throw new InvalidOperationException("Inteligente.NoEncontrado");
                if (MessageBox.Show(this,T("Recorridos.ConfirmarEliminar"),Text,MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                { _bll.Eliminar(_id); _id=0; Cargar(); }
            });
            var referencia = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(8) };
            Etiqueta(referencia,"Recorridos.Referencia"); referencia.Controls.Add(_referencia); referencia.Controls.Add(_minimo);
            Boton(referencia,"Recorridos.GuardarReferencia", () => { _bll.GuardarReferencia(Id(_referencia),_minimo.Value==0 ? (int?)null : (int)_minimo.Value); Cargar(); });
            Etiqueta(referencia,"Recorridos.CeroSinReferencia");
            Controls.Add(_lista); Controls.Add(editor); Controls.Add(referencia);
            _lista.SelectionChanged += (s,e) =>
            {
                if (_lista.SelectedRows.Count == 0) return;
                var row = _lista.SelectedRows[0].DataBoundItem as RecorridoListado_83KI;
                if (row==null) return;
                _id=row.IdRecorridoCurso; _previo.SelectedValue=row.IdCursoPrevio; _siguiente.SelectedValue=row.IdCursoRecomendado; _orden.Value=row.Orden;
            };
            _referencia.SelectedIndexChanged += (s,e) =>
            {
                var curso = _referencia.SelectedItem as BE.Entidades.Curso_83KI;
                _minimo.Value=curso?.CupoMinimoReferencia ?? 0;
            };
            _idioma.Suscribir(this); Shown += (s,e) => Ejecutar(Cargar);
        }
        private static ComboBox Combo() { return new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList,Width=240,DisplayMember="Nombre",ValueMember="IdCurso" }; }
        private void IniciarAlta()
        {
            _lista.ClearSelection();
            _lista.CurrentCell = null;
            _id = 0;
        }
        private static int Id(ComboBox c) { return c.SelectedValue is int ? (int)c.SelectedValue : 0; }
        private static string T(string key) { return IdiomaUiHelper_83KI.Texto(key); }
        private void Ejecutar(Action accion)
        { try { accion(); } catch(Exception ex) { IdiomaUiHelper_83KI.MostrarError(this,ex,"Comun.Validacion",MessageBoxIcon.Warning); } }
        private void Etiqueta(FlowLayoutPanel panel,string clave)
        { var label=new Label { AutoSize=true,Padding=new Padding(0,6,0,0) }; _textos.Add(clave,label); panel.Controls.Add(label); }
        private void Boton(FlowLayoutPanel panel,string clave,Action accion)
        { var boton=new Button { AutoSize=true }; _textos.Add(clave,boton); panel.Controls.Add(boton); boton.Click+=(s,e)=>Ejecutar(accion); }
        private void Cargar()
        {
            _previo.DataSource=_bll.Cursos(); _siguiente.DataSource=_bll.Cursos(); _referencia.DataSource=_bll.Cursos();
            _lista.DataSource=_bll.Listar(); Traducir(); IniciarAlta();
        }
        private void Traducir()
        {
            Text=T("Recorridos.Titulo"); foreach(var item in _textos) item.Value.Text=T(item.Key);
            foreach(DataGridViewColumn col in _lista.Columns)
            { col.Visible=!col.DataPropertyName.StartsWith("Id",StringComparison.Ordinal); col.HeaderText=T("Recorridos.Col."+col.DataPropertyName); }
        }
        public void ActualizarIdioma(IIdioma idioma) { Traducir(); }
        protected override void OnFormClosed(FormClosedEventArgs e) { _idioma.Desuscribir(this); base.OnFormClosed(e); }
    }
}

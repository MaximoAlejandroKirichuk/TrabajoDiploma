using System;
using System.Collections.Generic;
using BE.DTOs.Recorridos;
using BE.Entidades;
using Service.Entidades;
using Service.Interfaces;

namespace BLL
{
    public sealed class KI68RecorridoCursoBLL : IRecorridoCursoBLL_83KI
    {
        private readonly IRecorridoCursoDAL_83KI _dal;
        private readonly ISessionManager_83KI _sesion;
        private readonly IBitacoraManager_83KI _bitacora;
        public KI68RecorridoCursoBLL(IRecorridoCursoDAL_83KI dal, ISessionManager_83KI sesion, IBitacoraManager_83KI bitacora)
        { _dal = dal; _sesion = sesion; _bitacora = bitacora; }
        private void Autorizar()
        {
            if (!_sesion.TienePermiso(PermisoSistema_83KI.GestionRecorridos)) throw new InvalidOperationException("Errores.SinPermisos");
        }
        private void Auditar(string texto)
        {
            try { _bitacora.RegistrarEvento(BitacoraEvento_83KI.CrearNuevo(texto, Criticidad.Alto, Modulo.Admin, _sesion.UsuarioActivo.UserName)); }
            catch { throw new InvalidOperationException("Errores.AuditoriaFallidaCambioConfirmado"); }
        }
        public List<RecorridoListado_83KI> Listar() { Autorizar(); return _dal.Listar(); }
        public List<Curso_83KI> Cursos() { Autorizar(); return _dal.Cursos(); }
        public void Guardar(int id, int previo, int recomendado, int orden)
        {
            Autorizar();
            _dal.Guardar(new RecorridoCurso_83KI(id, previo, recomendado, orden));
            Auditar(string.Format("Recorrido guardado: {0}. Curso previo: {1}. Curso recomendado: {2}. Orden: {3}.", id, previo, recomendado, orden));
        }
        public void Eliminar(int id)
        {
            Autorizar();
            if (id <= 0) throw new ArgumentException("Inteligente.RecorridoInvalido");
            _dal.Eliminar(id);
            Auditar("Recorrido eliminado: " + id);
        }
        public void GuardarReferencia(int idCurso, int? minimo)
        {
            Autorizar();
            if (idCurso <= 0 || (minimo.HasValue && minimo <= 0)) throw new ArgumentException("Inteligente.MinimoInvalido");
            _dal.GuardarReferencia(idCurso, minimo);
            Auditar("Cupo mínimo de referencia actualizado. Curso: " + idCurso + ". Mínimo: " + minimo);
        }
    }
}

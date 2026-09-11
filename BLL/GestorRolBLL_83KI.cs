using DAL.interfaces;
using Service;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    internal class GestorRolBLL_83KI : IGestorRol_83KI
    {
        private readonly IRolDAL_83KI _rolDal;
        private readonly ISessionManager_83KI _sessionManager;
        private readonly IBitacoraManager_83KI _bitacoraManager;

        public GestorRolBLL_83KI(IRolDAL_83KI rolDal)
            : this(rolDal, SessionManager_83KI.Instancia, null)
        {
        }

        public GestorRolBLL_83KI(IRolDAL_83KI rolDal, ISessionManager_83KI sessionManager)
            : this(rolDal, sessionManager, null)
        {
        }

        public GestorRolBLL_83KI(IRolDAL_83KI rolDal, ISessionManager_83KI sessionManager, IBitacoraManager_83KI bitacoraManager)
        {
            _rolDal = rolDal;
            _sessionManager = sessionManager;
            _bitacoraManager = bitacoraManager;
        }

        public IEnumerable<Rol_83KI> ObtenerRoles()
        {
            return _rolDal.ObtenerRoles();
        }

        public IEnumerable<Rol_83KI> ObtenerRolesConPermisos()
        {
            return _rolDal.ObtenerRolesConPermisos();
        }

        public IEnumerable<Familia_83KI> ObtenerFamilias()
        {
            return _rolDal.ObtenerFamilias();
        }

        public IEnumerable<Patente_83KI> ObtenerPatentes()
        {
            return _rolDal.ObtenerPatentes();
        }

        public Rol_83KI CrearRol(string nombre, List<int> codigosPatentes, List<int> codigosFamilias)
        {
            ValidarPermiso(PermisoSistema_83KI.GestionRoles);

            string nombreNormalizado = ValidarNombre(nombre);

            if (_rolDal.ObtenerRoles().Any(r => string.Equals(r.Nombre, nombreNormalizado, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Ya existe un rol con ese nombre.");
            }

            List<int> patentes = codigosPatentes ?? new List<int>();
            List<int> familias = codigosFamilias ?? new List<int>();

            if (patentes.Count == 0 && familias.Count == 0)
            {
                throw new InvalidOperationException("El rol debe contener al menos una patente o familia.");
            }

            // Validar que todas las entidades referenciadas existan
            foreach (int codigoPatente in patentes)
            {
                ObtenerPatente(codigoPatente);
            }

            List<Familia_83KI> familiasCargadas = new List<Familia_83KI>();
            foreach (int codigoFamilia in familias)
            {
                familiasCargadas.Add(ObtenerFamilia(codigoFamilia));
            }

            // Evitar asignaciones lógicas duplicadas: asegurar que las patentes directas no
            // estén ya cubiertas por la jerarquía de patentes de alguna familia seleccionada
            HashSet<int> patentesDesdeFamilias = new HashSet<int>();
            foreach (Familia_83KI familia in familiasCargadas)
            {
                foreach (Patente_83KI p in familia.ObtenerPatentes())
                {
                    patentesDesdeFamilias.Add(p.CodigoPatente);
                }
            }

            foreach (int codigoPatente in patentes)
            {
                if (patentesDesdeFamilias.Contains(codigoPatente))
                {
                    throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
                }
            }

            // Evitar superposición entre familias seleccionadas: si dos familias comparten patentes,
            // la asignación genera permisos duplicados
            for (int i = 0; i < familiasCargadas.Count; i++)
            {
                HashSet<int> patentesI = new HashSet<int>(
                    familiasCargadas[i].ObtenerPatentes().Select(p => p.CodigoPatente));
                for (int j = i + 1; j < familiasCargadas.Count; j++)
                {
                    foreach (Patente_83KI p in familiasCargadas[j].ObtenerPatentes())
                    {
                        if (patentesI.Contains(p.CodigoPatente))
                        {
                            throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
                        }
                    }
                }
            }

            // Evitar familias duplicadas
            if (familias.Distinct().Count() != familias.Count)
            {
                throw new InvalidOperationException("Hay familias duplicadas en la seleccion.");
            }

            // Evitar patentes duplicadas
            if (patentes.Distinct().Count() != patentes.Count)
            {
                throw new InvalidOperationException("Hay patentes duplicadas en la seleccion.");
            }

            Rol_83KI rol = _rolDal.CrearRolConComponentes(nombreNormalizado, patentes, familias);

            RegistrarAuditoria(string.Format("Nuevo rol creado: {0}", nombreNormalizado));

            return rol;
        }

        public Familia_83KI CrearFamilia(string nombre, List<int> codigosPatentes, List<int> codigosFamilias)
        {
            ValidarPermiso(PermisoSistema_83KI.CrearFamilia);

            string nombreNormalizado = ValidarNombre(nombre);

            if (_rolDal.ObtenerFamilias().Any(f => string.Equals(f.Nombre, nombreNormalizado, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Ya existe una familia con ese nombre.");
            }

            List<int> patentes = codigosPatentes ?? new List<int>();
            List<int> familias = codigosFamilias ?? new List<int>();

            // Regla de negocio: la familia debe tener al menos una patente en su jerarquía (directa o indirecta)
            bool tienePatenteDirecta = patentes.Count > 0;

            bool tienePatenteIndirecta = false;
            List<Familia_83KI> familiasCargadas = new List<Familia_83KI>();
            foreach (int codigoFamilia in familias)
            {
                Familia_83KI familiaCargada = ObtenerFamilia(codigoFamilia);
                familiasCargadas.Add(familiaCargada);
                if (familiaCargada.ObtenerPatentes().Any())
                {
                    tienePatenteIndirecta = true;
                }
            }

            if (!tienePatenteDirecta && !tienePatenteIndirecta)
            {
                throw new InvalidOperationException("La familia debe contener al menos una patente.");
            }

            // Validar que todas las entidades referenciadas existan
            foreach (int codigoPatente in patentes)
            {
                ObtenerPatente(codigoPatente);
            }

            // Evitar referencias circulares: asegurar que ninguna familia seleccionada contenga
            // (transitivamente) a otra familia seleccionada, y que ninguna familia seleccionada
            // contenga una patente ya seleccionada
            HashSet<int> familiasSeleccionadas = new HashSet<int>(familias);
            HashSet<int> patentesSeleccionadas = new HashSet<int>(patentes);

            foreach (Familia_83KI famCargada in familiasCargadas)
            {
                // Una familia no puede contenerse a sí misma
                if (familiasSeleccionadas.Contains(famCargada.CodigoFamilia))
                {
                    // Verificar contención transitiva: si alguna otra familia seleccionada
                    // está dentro de la jerarquía de esta familia, hay riesgo de ciclo
                    foreach (Familia_83KI otra in familiasCargadas)
                    {
                        if (otra.CodigoFamilia != famCargada.CodigoFamilia &&
                            famCargada.Contiene(otra))
                        {
                            throw new InvalidOperationException("No se puede asignar una familia a si misma ni generar ciclos.");
                        }
                    }
                }

                // Verificar asignaciones de patentes duplicadas entre la patente seleccionada y las patentes de la familia
                foreach (Patente_83KI p in famCargada.ObtenerPatentes())
                {
                    if (patentesSeleccionadas.Contains(p.CodigoPatente))
                    {
                        throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
                    }
                }
            }

            // Evitar superposición entre subfamilias seleccionadas: si dos familias comparten patentes,
            // la asignación genera permisos duplicados
            for (int i = 0; i < familiasCargadas.Count; i++)
            {
                HashSet<int> patentesI = new HashSet<int>(
                    familiasCargadas[i].ObtenerPatentes().Select(p => p.CodigoPatente));
                for (int j = i + 1; j < familiasCargadas.Count; j++)
                {
                    foreach (Patente_83KI p in familiasCargadas[j].ObtenerPatentes())
                    {
                        if (patentesI.Contains(p.CodigoPatente))
                        {
                            throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
                        }
                    }
                }
            }

            // Evitar familias duplicadas
            if (familias.Distinct().Count() != familias.Count)
            {
                throw new InvalidOperationException("Hay familias duplicadas en la seleccion.");
            }

            // Evitar patentes duplicadas
            if (patentes.Distinct().Count() != patentes.Count)
            {
                throw new InvalidOperationException("Hay patentes duplicadas en la seleccion.");
            }

            Familia_83KI familia = _rolDal.CrearFamiliaConComponentes(nombreNormalizado, patentes, familias);

            RegistrarAuditoria(string.Format("Nueva familia creada: {0}", nombreNormalizado));

            return familia;
        }

        public void EliminarFamilia(int codigoFamilia)
        {
            ValidarPermiso(PermisoSistema_83KI.EliminarFamilia);

            if (_rolDal.FamiliaAsignadaARol(codigoFamilia))
            {
                throw new InvalidOperationException("No se puede eliminar la familia porque está asignada a uno o más roles.");
            }

            if (_rolDal.FamiliaEsSubfamiliaDeOtra(codigoFamilia))
            {
                throw new InvalidOperationException("No se puede eliminar la familia porque es subfamilia de otra familia.");
            }

            // Obtener nombre antes de eliminar para no perderlo en la auditoría
            Familia_83KI familiaEliminada = _rolDal.ObtenerFamilias()
                .FirstOrDefault(f => f.CodigoFamilia == codigoFamilia);
            string nombreFamilia = familiaEliminada != null ? familiaEliminada.Nombre : codigoFamilia.ToString();

            _rolDal.EliminarFamilia(codigoFamilia);
            RegistrarAuditoria(string.Format("Familia eliminada: {0} (Codigo: {1})", nombreFamilia, codigoFamilia));
        }

        public void EliminarRol(int codigoRol)
        {
            ValidarPermiso(PermisoSistema_83KI.EliminarRol);

            if (_rolDal.RolTieneUsuarios(codigoRol))
            {
                throw new InvalidOperationException("El rol tiene usuarios asignados.");
            }

            // Obtener nombre antes de eliminar para no perderlo en la auditoría
            Rol_83KI rolEliminado = _rolDal.ObtenerRoles()
                .FirstOrDefault(r => r.CodigoRol == codigoRol);
            string nombreRol = rolEliminado != null ? rolEliminado.Nombre : codigoRol.ToString();

            _rolDal.EliminarRol(codigoRol);
            RegistrarAuditoria(string.Format("Rol eliminado: {0} (Codigo: {1})", nombreRol, codigoRol));
        }

        public void AsignarPatenteAFamilia(int codigoFamilia, int codigoPatente)
        {
            ValidarPermiso(PermisoSistema_83KI.AgregarPatenteFamilia);

            Familia_83KI familia = ObtenerFamilia(codigoFamilia);
            Patente_83KI patente = ObtenerPatente(codigoPatente);

            ValidarPatenteEnCadenaAncestral(familia, codigoPatente);
            ValidarPatenteEnRolesQueContienenFamilia(familia, patente);

            familia.Agregar(patente);
            _rolDal.AsignarPatenteAFamilia(codigoFamilia, codigoPatente);
            RegistrarAuditoria(string.Format("Familia modificada: {0}", familia.Nombre));
        }

        public void QuitarPatenteDeFamilia(int codigoFamilia, int codigoPatente)
        {
            ValidarPermiso(PermisoSistema_83KI.QuitarPatenteFamilia);

            Familia_83KI familia = ObtenerFamilia(codigoFamilia);

            bool hayOtrasPatentesDirectas = familia.Hijos.OfType<Patente_83KI>()
                .Any(p => p.CodigoPatente != codigoPatente);
            bool haySubfamiliasConPatentes = familia.Hijos.OfType<Familia_83KI>()
                .Any(f => f.ObtenerPatentes().Any());

            if (!hayOtrasPatentesDirectas && !haySubfamiliasConPatentes)
            {
                throw new InvalidOperationException("La familia debe contener al menos una patente.");
            }

            _rolDal.QuitarPatenteDeFamilia(codigoFamilia, codigoPatente);
            RegistrarAuditoria(string.Format("Familia modificada: {0}", familia.Nombre));
        }

        public void AsignarFamiliaAFamilia(int codigoFamiliaPadre, int codigoFamiliaHija)
        {
            ValidarPermiso(PermisoSistema_83KI.AgregarSubfamilia);

            Familia_83KI familiaPadre = ObtenerFamilia(codigoFamiliaPadre);
            Familia_83KI familiaHija = ObtenerFamilia(codigoFamiliaHija);

            ValidarFamiliaEnCadenaAncestral(familiaPadre, familiaHija);

            familiaPadre.Agregar(familiaHija);
            _rolDal.AsignarFamiliaAFamilia(codigoFamiliaPadre, codigoFamiliaHija);
            RegistrarAuditoria(string.Format("Familia modificada: {0}", familiaPadre.Nombre));
        }

        public void QuitarFamiliaDeFamilia(int codigoFamiliaPadre, int codigoFamiliaHija)
        {
            ValidarPermiso(PermisoSistema_83KI.QuitarSubfamilia);

            Familia_83KI familiaPadre = ObtenerFamilia(codigoFamiliaPadre);

            bool hayPatentesDirectas = familiaPadre.Hijos.OfType<Patente_83KI>().Any();
            bool hayOtrasSubfamiliasConPatentes = familiaPadre.Hijos.OfType<Familia_83KI>()
                .Any(f => f.CodigoFamilia != codigoFamiliaHija && f.ObtenerPatentes().Any());

            if (!hayPatentesDirectas && !hayOtrasSubfamiliasConPatentes)
            {
                throw new InvalidOperationException("La familia debe contener al menos una patente.");
            }

            _rolDal.QuitarFamiliaDeFamilia(codigoFamiliaPadre, codigoFamiliaHija);
            RegistrarAuditoria(string.Format("Familia modificada: {0}", familiaPadre.Nombre));
        }

        public void AsignarPatenteARol(int codigoRol, int codigoPatente)
        {
            ValidarPermiso(PermisoSistema_83KI.AsignarPatenteRol);

            Rol_83KI rol = ObtenerRol(codigoRol);
            Patente_83KI patente = ObtenerPatente(codigoPatente);
            rol.AgregarPatente(patente);
            _rolDal.AsignarPatenteARol(codigoRol, codigoPatente);
            RegistrarAuditoria(string.Format("Rol modificado: {0}", rol.Nombre));
        }

        public void QuitarPatenteDeRol(int codigoRol, int codigoPatente)
        {
            ValidarPermiso(PermisoSistema_83KI.QuitarPatenteRol);

            Rol_83KI rol = ObtenerRol(codigoRol);

            bool hayOtrasPatentesDirectas = rol.PatentesDirectas
                .Any(p => p.CodigoPatente != codigoPatente);
            bool hayFamilias = rol.Familias.Any();

            if (!hayOtrasPatentesDirectas && !hayFamilias)
            {
                throw new InvalidOperationException("El rol debe contener al menos una patente o familia.");
            }

            _rolDal.QuitarPatenteDeRol(codigoRol, codigoPatente);
            RegistrarAuditoria(string.Format("Rol modificado: {0}", rol.Nombre));
        }

        public void AsignarFamiliaARol(int codigoRol, int codigoFamilia)
        {
            ValidarPermiso(PermisoSistema_83KI.AgregarFamiliaRol);

            Rol_83KI rol = ObtenerRol(codigoRol);
            Familia_83KI familia = ObtenerFamilia(codigoFamilia);
            rol.AgregarFamilia(familia);
            _rolDal.AsignarFamiliaARol(codigoRol, codigoFamilia);
            RegistrarAuditoria(string.Format("Rol modificado: {0}", rol.Nombre));
        }

        public void QuitarFamiliaDeRol(int codigoRol, int codigoFamilia)
        {
            ValidarPermiso(PermisoSistema_83KI.QuitarFamiliaRol);

            Rol_83KI rol = ObtenerRol(codigoRol);

            bool hayOtrasFamilias = rol.Familias
                .Any(f => f.CodigoFamilia != codigoFamilia);
            bool hayPatentesDirectas = rol.PatentesDirectas.Any();

            if (!hayOtrasFamilias && !hayPatentesDirectas)
            {
                throw new InvalidOperationException("El rol debe contener al menos una patente o familia.");
            }

            _rolDal.QuitarFamiliaDeRol(codigoRol, codigoFamilia);
            RegistrarAuditoria(string.Format("Rol modificado: {0}", rol.Nombre));
        }

        private string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
            }

            return nombre.Trim();
        }

        private Rol_83KI ObtenerRol(int codigoRol)
        {
            Rol_83KI rol = _rolDal.ObtenerRolesConPermisos().FirstOrDefault(r => r.CodigoRol == codigoRol);

            if (rol == null)
            {
                throw new InvalidOperationException("El rol seleccionado no existe.");
            }

            return rol;
        }

        private Familia_83KI ObtenerFamilia(int codigoFamilia)
        {
            Familia_83KI familia = _rolDal.ObtenerFamilias().FirstOrDefault(f => f.CodigoFamilia == codigoFamilia);

            if (familia == null)
            {
                throw new InvalidOperationException("La familia seleccionada no existe.");
            }

            return familia;
        }

        private Patente_83KI ObtenerPatente(int codigoPatente)
        {
            Patente_83KI patente = _rolDal.ObtenerPatentes().FirstOrDefault(p => p.CodigoPatente == codigoPatente);

            if (patente == null)
            {
                throw new InvalidOperationException("La patente seleccionada no existe.");
            }

            return patente;
        }

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (!_sessionManager.TienePermiso(permiso))
            {
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
            }
        }

        private void ValidarPatenteEnCadenaAncestral(Familia_83KI familiaDestino, int codigoPatente)
        {
            foreach (Familia_83KI familia in _rolDal.ObtenerFamilias())
            {
                if (familia.CodigoFamilia == familiaDestino.CodigoFamilia)
                    continue;

                if (familia.Contiene(familiaDestino))
                {
                    if (familia.ObtenerPatentes().Any(p => p.CodigoPatente == codigoPatente))
                    {
                        throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
                    }
                }
            }
        }

        private void ValidarPatenteEnRolesQueContienenFamilia(Familia_83KI familiaDestino, Patente_83KI patente)
        {
            foreach (Rol_83KI rol in _rolDal.ObtenerRolesConPermisos())
            {
                if (!RolContieneFamilia(rol, familiaDestino))
                {
                    continue;
                }

                if (rol.PatentesDirectas.Any(p => p.CodigoPatente == patente.CodigoPatente))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Errores.PatenteDuplicadaEnRolDirecto|{0}|{1}|{2}",
                            patente.Nombre,
                            familiaDestino.Nombre,
                            rol.Nombre));
                }

                Familia_83KI familiaDuplicada = rol.Familias.FirstOrDefault(f =>
                    !PerteneceARamaDeFamiliaDestino(f, familiaDestino)
                    && f.ObtenerPatentes().Any(p => p.CodigoPatente == patente.CodigoPatente));

                if (familiaDuplicada != null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Errores.PatenteDuplicadaEnRolPorFamilia|{0}|{1}|{2}|{3}",
                            patente.Nombre,
                            familiaDestino.Nombre,
                            rol.Nombre,
                            familiaDuplicada.Nombre));
                }
            }
        }

        private bool RolContieneFamilia(Rol_83KI rol, Familia_83KI familiaDestino)
        {
            return rol.Familias.Any(f => PerteneceARamaDeFamiliaDestino(f, familiaDestino));
        }

        private bool PerteneceARamaDeFamiliaDestino(Familia_83KI familia, Familia_83KI familiaDestino)
        {
            return familia.CodigoFamilia == familiaDestino.CodigoFamilia || familia.Contiene(familiaDestino);
        }

        private void ValidarFamiliaEnCadenaAncestral(Familia_83KI familiaPadre, Familia_83KI familiaHija)
        {
            HashSet<int> patentesHija = new HashSet<int>(
                familiaHija.ObtenerPatentes().Select(p => p.CodigoPatente));

            if (patentesHija.Count == 0)
                return;

            foreach (Familia_83KI familia in _rolDal.ObtenerFamilias())
            {
                if (familia.CodigoFamilia == familiaPadre.CodigoFamilia)
                    continue;

                if (familia.Contiene(familiaPadre))
                {
                    if (familia.ObtenerPatentes().Any(p => patentesHija.Contains(p.CodigoPatente)))
                    {
                        throw new InvalidOperationException("La asignacion duplicaria permisos indirectos.");
                    }
                }
            }
        }

        private void RegistrarAuditoria(string descripcion)
        {
            if (_bitacoraManager == null || _sessionManager.UsuarioActivo == null)
            {
                return;
            }

            try
            {
                BitacoraEvento_83KI evento = BitacoraEvento_83KI.CrearNuevo(
                    $"{descripcion} (Actor: {_sessionManager.UsuarioActivo.UserName})",
                    Criticidad.Bajo,
                    Modulo.Admin,
                    _sessionManager.UsuarioActivo.UserName);
                _bitacoraManager.RegistrarEvento(evento);
            }
            catch
            {
                // Un error de auditoría no debe interrumpir el flujo de creación
            }
        }
    }
}

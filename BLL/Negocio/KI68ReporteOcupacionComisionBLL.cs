using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BLL
{
    public class KI68ReporteOcupacionComisionBLL : IReporteOcupacionComisionBLL_83KI
    {
        private readonly IReporteOcupacionComisionDAL_83KI _dal;
        private readonly ISessionManager_83KI _sessionManager;

        public KI68ReporteOcupacionComisionBLL(IReporteOcupacionComisionDAL_83KI dal, ISessionManager_83KI sessionManager)
        {
            _dal = dal;
            _sessionManager = sessionManager;
        }

        public IEnumerable<ReporteOcupacionComision_83KI> Generar(string estado)
        {
            if (!_sessionManager.TienePermiso(PermisoSistema_83KI.VerReporteOcupacionComisiones))
                throw new InvalidOperationException("Errores.SinPermisos");

            try
            {
                return _dal.Generar(estado);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}

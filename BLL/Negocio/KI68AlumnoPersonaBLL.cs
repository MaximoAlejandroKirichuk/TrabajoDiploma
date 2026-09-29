using BE.Entidades;
using Service.DTOs;
using Service.Interfaces;
using System;

namespace BLL
{
    public class KI68AlumnoPersonaBLL : IAlumnoPersonaBLL_83KI
    {
        private readonly ILeadDAL_83KI _leadDAL;

        public KI68AlumnoPersonaBLL(ILeadDAL_83KI leadDAL)
        {
            _leadDAL = leadDAL;
        }

        public BusquedaLeadResultado_83KI BuscarLead(string dni, string email, string telefono)
        {
            return _leadDAL.BuscarPorIdentidad(dni, email, telefono);
        }

        public Lead_83KI ResolverLeadUnico(ConsultaLeadRequest_83KI request)
        {
            var resultado = BuscarLead(request.DNI, request.Email, request.Telefono);
            if (resultado.EsAmbigua) throw new InvalidOperationException("Errores.LeadCoincidenciaAmbigua");
            return resultado.Lead;
        }
    }
}

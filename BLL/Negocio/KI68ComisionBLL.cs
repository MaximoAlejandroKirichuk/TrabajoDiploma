using BE.Entidades;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BLL
{
    public class KI68ComisionBLL : IComisionConsultaBLL_83KI, IComisionSerializacionBLL_83KI
    {
        private readonly IComisionDAL_83KI _comisionDAL;
        private readonly ISessionManager_83KI _sessionManager;

        public KI68ComisionBLL(IComisionDAL_83KI comisionDAL)
            : this(comisionDAL, SessionManager_83KI.Instancia)
        {
        }

        public KI68ComisionBLL(IComisionDAL_83KI comisionDAL, ISessionManager_83KI sessionManager)
        {
            _comisionDAL = comisionDAL;
            _sessionManager = sessionManager;
        }

        public IEnumerable<ComisionListado_83KI> ListarComisionesPreapertura()
        {
            return _comisionDAL.ListarComisionesPreapertura();
        }

        public void ValidarComisionPreapertura(int idComision)
        {
            if (idComision <= 0 || !_comisionDAL.ExisteComisionPreapertura(idComision))
                throw new InvalidOperationException("Errores.ComisionPreaperturaNoDisponible");
        }

        // A03 - Serializacion: genera el archivo XML con las comisiones seleccionadas en la grilla.
        public void Serializar(string ruta, List<ComisionListado_83KI> comisiones)
        {
            ValidarPermiso(PermisoSistema_83KI.SerializarComisiones);
            if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Errores.RutaArchivoObligatoria");
            if (comisiones == null || comisiones.Count == 0) throw new ArgumentException("Errores.ComisionesSerializarObligatorias");

            var comisionesXml = new List<ComisionXml_83KI>();
            foreach (var listado in comisiones)
            {
                // Se busca la comision completa para incluir plan de pago, matricula y recargo.
                Comision_83KI comision = _comisionDAL.ObtenerComision(listado.IdComision);
                if (comision == null) throw new InvalidOperationException("Errores.ComisionNoEncontrada");
                comisionesXml.Add(ConvertirAXml(comision, listado));
            }

            var serializador = CrearSerializador();
            using (var escritor = new StreamWriter(ruta, false, System.Text.Encoding.UTF8))
            {
                serializador.Serialize(escritor, comisionesXml);
            }
        }

        // A03 - Deserializacion: lee el archivo XML y devuelve las comisiones para mostrarlas en la grilla.
        public List<ComisionXml_83KI> Deserializar(string ruta)
        {
            ValidarPermiso(PermisoSistema_83KI.DeserializarComisiones);
            if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Errores.RutaArchivoObligatoria");
            if (!File.Exists(ruta)) throw new FileNotFoundException("Errores.ArchivoXmlNoEncontrado");

            List<ComisionXml_83KI> comisiones;
            try
            {
                var serializador = CrearSerializador();
                using (var lector = new StreamReader(ruta))
                {
                    comisiones = (List<ComisionXml_83KI>)serializador.Deserialize(lector);
                }
            }
            catch (InvalidOperationException ex)
            {
                // XmlSerializer envuelve cualquier error de lectura (XML mal formado, horas invalidas, etc.)
                throw new InvalidOperationException("Errores.ArchivoXmlInvalido", ex);
            }
            catch (XmlException ex)
            {
                throw new InvalidOperationException("Errores.ArchivoXmlInvalido", ex);
            }

            if (comisiones == null || comisiones.Count == 0)
                throw new InvalidOperationException("Errores.ArchivoXmlSinComisiones");
            return comisiones;
        }

        private static XmlSerializer CrearSerializador()
        {
            return new XmlSerializer(typeof(List<ComisionXml_83KI>), new XmlRootAttribute("Comisiones"));
        }

        private static ComisionXml_83KI ConvertirAXml(Comision_83KI comision, ComisionListado_83KI listado)
        {
            return new ComisionXml_83KI
            {
                Codigo = comision.Codigo,
                IdCurso = comision.IdCurso,
                Curso = listado.Curso,
                IdProfesor = comision.IdProfesor,
                Profesor = listado.Profesor,
                DiaSemana = comision.DiaSemana,
                HoraInicio = comision.HoraInicio,
                HoraFin = comision.HoraFin,
                CupoMinimo = comision.CupoMinimo,
                CupoMaximo = comision.CupoMaximo,
                FechaLimitePago = comision.FechaLimitePago,
                FechaInicio = comision.FechaInicio,
                FechaFin = comision.FechaFin,
                ArancelBase = comision.ArancelBase,
                MontoMatricula = comision.MontoMatricula,
                IdPlanDePago = comision.IdPlanDePago,
                RecargoPlanSnapshot = comision.RecargoPlanSnapshot,
                Estado = comision.Estado
            };
        }

        private void ValidarPermiso(PermisoSistema_83KI permiso)
        {
            if (_sessionManager == null || !_sessionManager.TienePermiso(permiso))
                throw new InvalidOperationException("No tiene permisos para realizar esta accion.");
        }
    }
}

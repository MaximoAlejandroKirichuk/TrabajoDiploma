using System;
using System.Collections.Generic;
using DAL.interfaces;
using Service;
using Service.DTOs;
using Service.Entidades;
using Service.Interfaces;

namespace BLL
{
    /// <summary>
    /// logica de negocio para verificacion de integridad y recalculo de hashes.
    /// implementa IIntegridadDatosService_83KI — la puerta de login y el contrato de recuperacion del admin.
    /// </summary>
    internal class IntegridadBLL_83KI : IIntegridadDatosService_83KI
    {
        private readonly IIntegridadDAL_83KI _integridadDal;
        private readonly IBitacoraManager_83KI _bitacora;

        public IntegridadBLL_83KI(IIntegridadDAL_83KI integridadDal, IBitacoraManager_83KI bitacora)
        {
            _integridadDal = integridadDal ?? throw new ArgumentNullException(nameof(integridadDal));
            _bitacora = bitacora ?? throw new ArgumentNullException(nameof(bitacora));
        }

        /// <summary>
        /// verifica la integridad de todas las tablas protegidas comparando el dvv
        /// almacenado contra el dvv calculado desde los datos vivos de las filas.
        /// para las tablas que fallan, recolecta el detalle de discrepancia dvh por fila
        /// para que la ui de recuperacion del admin muestre que filas y columnas estan afectadas.
        /// </summary>
        public IntegridadSistemaEstado_83KI Verificar()
        {
            var estado = new IntegridadSistemaEstado_83KI { EstaSano = true, RequiereRecuperacion = false };
            Dictionary<string, string> dvvActuales;

            try
            {
                dvvActuales = _integridadDal.ObtenerDVVActuales();
            }
            catch (Exception)
            {
                // no se pudo leer la tabla de control dvv — el sistema no esta sano
                estado.EstaSano = false;
                estado.RequiereRecuperacion = true;
                return estado;
            }

            foreach (var kvp in dvvActuales)
            {
                string nombreTabla = kvp.Key;
                string dvvAlmacenado = kvp.Value ?? string.Empty;

                string dvvCalculado;
                try
                {
                    dvvCalculado = _integridadDal.CalcularDVVPorTabla(nombreTabla);
                }
                catch
                {
                    // la tabla puede estar inaccesible — marcar como invalida
                    estado.Tablas.Add(new IntegridadTablaEstado_83KI
                    {
                        NombreTabla = nombreTabla,
                        DVVActual = dvvAlmacenado,
                        DVVEsperado = "ERROR",
                        EsValido = false
                    });
                    estado.EstaSano = false;
                    estado.RequiereRecuperacion = true;
                    continue;
                }

                bool esValido = string.Equals(dvvAlmacenado, dvvCalculado, StringComparison.OrdinalIgnoreCase);

                var tablaEstado = new IntegridadTablaEstado_83KI
                {
                    NombreTabla = nombreTabla,
                    DVVActual = dvvAlmacenado,
                    DVVEsperado = dvvCalculado,
                    EsValido = esValido
                };

                if (!esValido)
                {
                    estado.EstaSano = false;
                    estado.RequiereRecuperacion = true;

                    // recolecta detalle por fila para que la ui del admin muestre
                    // exactamente que filas y columnas estan afectadas
                    try
                    {
                        tablaEstado.FilasInconsistentes = _integridadDal.ObtenerFilasInconsistentes(nombreTabla);
                    }
                    catch
                    {
                        // el detalle por fila es mejor-esfuerzo; una falla aca no debe
                        // impedir que se reporte la falla a nivel tabla
                        tablaEstado.FilasInconsistentes = new List<IntegridadFilaInconsistencia_83KI>();
                    }
                }

                estado.Tablas.Add(tablaEstado);
            }

            return estado;
        }

        /// <summary>
        /// recalcula todos los valores dvh desde los datos vivos, regenera las entradas dvv,
        /// audita la accion y devuelve el nuevo estado sano.
        /// </summary>
        public IntegridadSistemaEstado_83KI RecalcularTodo(string actor)
        {
            _integridadDal.RecalcularTodo();

            RegistrarAuditoriaSegura(
                string.Format("Recalculo de hashes ejecutado: {0}", actor),
                Criticidad.Alto,
                Modulo.Admin,
                actor
            );

            return Verificar();
        }

        private void RegistrarAuditoriaSegura(string descripcion, Criticidad criticidad, Modulo modulo, string username)
        {
            try
            {
                _bitacora.RegistrarEvento(
                    BitacoraEvento_83KI.CrearNuevo(descripcion, criticidad, modulo, username)
                );
            }
            catch
            {
                // la auditoria no debe interrumpir el flujo de negocio
            }
        }
    }
}

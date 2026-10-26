using BE.Entidades;
using Service.DTOs;
using System;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface ICertificadoDAL_83KI
    {
        List<Comision_83KI> ListarFinalizadas();
        List<AlumnoCertificacion_83KI> Buscar(int idComision);
        // Volver a leer la elegibilidad actual dentro de una transacción y mantener los bloqueos durante la decisión de la BLL.
        // Una instantánea devuelta solicita la emisión; null indica consulta o retención sin escrituras de negocio.
        // Retorna solo después de confirmar la transacción; cualquier fallo de negocio o de integridad provoca su reversión.
        AlumnoCertificacion_83KI RevalidarEnTransaccion(int idInscripcion,
            Func<AlumnoCertificacion_83KI, CertificadoFinalizacion_83KI> resolver);
    }
}

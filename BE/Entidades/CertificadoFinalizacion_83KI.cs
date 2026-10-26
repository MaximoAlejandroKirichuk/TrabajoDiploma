using System;

namespace BE.Entidades
{
    // Instantánea al momento de emisión. Deliberadamente, no existen casos de uso para actualizarla o eliminarla.
    public sealed class CertificadoFinalizacion_83KI
    {
        public int IdCertificado { get; set; }
        public int IdInscripcion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Curso { get; set; }
        public decimal Promedio { get; set; }
        public string Firmante { get; set; }
        public string Cargo { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string Modalidad { get; set; }
        public string Email { get; set; }
        public string Mencion { get; set; }
        public string Observaciones { get; set; }
    }
}

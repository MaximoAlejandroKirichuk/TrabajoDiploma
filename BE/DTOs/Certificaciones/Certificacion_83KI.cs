using BE.Entidades;
using System;
using System.Collections.Generic;

namespace BE.DTOs.Certificaciones
{
    public sealed class CuotaCertificacion_83KI
    {
        public int Numero { get; set; }
        public decimal Balance { get; set; }
        public DateTime Vencimiento { get; set; }
    }
    public sealed class AlumnoCertificacion_83KI
    {
        public int IdInscripcion { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Curso { get; set; }
        public string CodigoComision { get; set; }
        public DateTime FechaFinComision { get; set; }
        public int CantidadNotas { get; set; }
        public decimal Evaluacion1 { get; set; }
        public decimal Evaluacion2 { get; set; }
        public decimal Promedio { get { return CantidadNotas == 2 ? (Evaluacion1 + Evaluacion2) / 2m : 0; } }
        public bool Aprobado { get { return CantidadNotas == 2 && Evaluacion1 >= 7m && Evaluacion2 >= 7m; } }
        public string Condicion { get { return Aprobado ? "Aprobado" : "Desaprobado"; } }
        public string ObservacionesProfesor { get; set; }
        public decimal Balance { get; set; }
        public DateTime Hoy { get; set; }
        public List<CuotaCertificacion_83KI> Cuotas { get; set; } = new List<CuotaCertificacion_83KI>();
        public CertificadoFinalizacion_83KI Certificado { get; set; }
        public string Motivos { get { return Certificado != null || CantidadNotas != 2 ? "" :
            (!Aprobado ? (Balance > 0 ? "Academico+Arancelario" : "Academico") : (Balance > 0 ? "Arancelario" : "")); } }
    }
    public sealed class GenerarCertificadoRequest_83KI
    {
        public int IdInscripcion { get; set; }
        public string Firmante { get; set; }
        public string Cargo { get; set; }
        public string NumeroFolio { get; set; }
        public string Modalidad { get; set; }
        public string Email { get; set; }
        public string Mencion { get; set; }
        public string Observaciones { get; set; }
    }
    public sealed class ResultadoCertificacion_83KI
    {
        public AlumnoCertificacion_83KI Alumno { get; set; }
        public bool Existente { get; set; }
        public bool Retenido { get; set; }
        public bool Reconfirmar { get; set; }
        public bool AuditoriaFallida { get; set; }
    }
}

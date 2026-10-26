using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Service.DTOs
{
    // Clase exclusiva para serializar/deserializar comisiones en XML.
    // XmlSerializer necesita constructor publico sin parametros y propiedades con get/set publicos.
    [Serializable]
    [XmlType("Comision")]
    public class ComisionXml_83KI
    {
        public string Codigo { get; set; }
        public int IdCurso { get; set; }
        public string Curso { get; set; }
        public int IdProfesor { get; set; }
        public string Profesor { get; set; }
        public DayOfWeek DiaSemana { get; set; }

        // XmlSerializer no serializa TimeSpan: se guarda como texto "HH:mm" y se expone como TimeSpan.
        [XmlIgnore]
        public TimeSpan HoraInicio { get; set; }

        [XmlIgnore]
        public TimeSpan HoraFin { get; set; }

        public int CupoMinimo { get; set; }
        public int CupoMaximo { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime FechaLimitePago { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime FechaInicio { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime FechaFin { get; set; }

        public decimal ArancelBase { get; set; }
        public decimal MontoMatricula { get; set; }
        public int IdPlanDePago { get; set; }
        public decimal RecargoPlanSnapshot { get; set; }
        public string Estado { get; set; }

        [XmlElement("HoraInicio")]
        public string HoraInicioXml
        {
            get { return HoraInicio.ToString(@"hh\:mm", CultureInfo.InvariantCulture); }
            set { HoraInicio = ConvertirHora(value); }
        }

        [XmlElement("HoraFin")]
        public string HoraFinXml
        {
            get { return HoraFin.ToString(@"hh\:mm", CultureInfo.InvariantCulture); }
            set { HoraFin = ConvertirHora(value); }
        }

        private static TimeSpan ConvertirHora(string valor)
        {
            TimeSpan hora;
            if (!TimeSpan.TryParse(valor, CultureInfo.InvariantCulture, out hora))
                throw new FormatException("Errores.ArchivoXmlInvalido");
            return hora;
        }
    }
}

using BE.Entidades;

namespace BE.DTOs.Leads
{
    public class ConsultaLeadResultado_83KI
    {
        public Lead_83KI Lead { get; set; }
        public ConsultaLead_83KI Consulta { get; set; }
        public bool LeadCreado { get; set; }
        public bool AuditoriaFallida { get; set; }
    }
}

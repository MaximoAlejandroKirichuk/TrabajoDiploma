namespace Service.DTOs
{
    /// <summary>
    /// estado que devuelve el coordinador de bootstrap despues de evaluar la configuracion.
    /// </summary>
    public enum EstadoBootstrap_83KI
    {
        Listo,
        RequiereConfiguracion,
        BaseDeDatosFaltante,
        ConexionInvalida
    }

    /// <summary>
    /// resultado del evaluador de bootstrap con estado y mensaje descriptivo.
    /// </summary>
    public class ResultadoBootstrap_83KI
    {
        public EstadoBootstrap_83KI Estado { get; set; }
        public string Mensaje { get; set; }
    }
}

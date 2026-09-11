namespace Service.Interfaces
{
    public interface IEncriptador_83KI
    {
        string HashContrasena(string texto);
        string CalcularHashIntegridad(string texto);
    }
}

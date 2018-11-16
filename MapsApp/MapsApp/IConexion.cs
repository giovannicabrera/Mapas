namespace MapsApp
{
    public interface IConexion
    {
        void Iniciar();
        void GuardarPos(string lat, string lon);
        Modelo.CustomMap TraeMapa();
    }
}

using Xamarin.Forms;
using MapsApp.Droid;

[assembly: Dependency(typeof(Conexion))]
namespace MapsApp.Droid
{
    class Conexion : IConexion
    {

        public Conexion(){
        }

        public void Iniciar() {
            Controles.Inicio();
        }

        public void GuardarPos(string lat, string lon) {
            Controles.PosicionXML(lat: lat, lon: lon);
        }

        public Modelo.CustomMap TraeMapa() {
            return Controles.MapaXML();
        }

    }
}
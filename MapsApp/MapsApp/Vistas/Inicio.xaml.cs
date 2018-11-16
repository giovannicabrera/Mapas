using System;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Inicio : ContentPage
    {
        private string mensaje="";
        public Inicio()
        {
            InitializeComponent();
        }

        async void Clic(object sender, EventArgs e)
        {
            double lati = 0, longi = 0;
            try
            {
                Localizar(sender, e);
                await this.DisplayAlert("Información", mensaje, "Ok");

                lati = double.Parse(App.latitud);
                longi = double.Parse(App.longitud);

                if ((lati != 0) & (longi != 0))
                {
                    DependencyService.Get<IConexion>().GuardarPos(App.latitud, App.longitud);                    
                }
            }
            catch (Exception ex) {
                mensaje = ex.Message;
            }
            await this.DisplayAlert("Información", mensaje, "Ok");
            if ((lati != 0) & (longi != 0))
            {
                await Navigation.PushAsync(new CustomMapPage());
                Navigation.RemovePage(this);
            }
        }

        async void Localizar(object sender, EventArgs e)
        {
            //Geolocalizacion
            try
            {
                var localizador = CrossGeolocator.Current;
                localizador.DesiredAccuracy = 50;
                var posicion = await localizador.GetPositionAsync(10000);

                App.latitud = posicion.Latitude.ToString();
                App.longitud = posicion.Longitude.ToString();
                mensaje = "Datos recolectados correctamente";
            }
            catch (Exception eg)
            {
                App.latitud = "0";
                App.longitud = "0";
                mensaje = eg.Message;
            }
        }

    }
}
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System;
using System.Xml.Linq;

namespace MapsApp
{
    public partial class CustomMapPage : ContentPage
    {
        private static List<Modelo.Posiciones> regiruta = new List<Modelo.Posiciones>();
        public CustomMapPage()
        {

            InitializeComponent();

            double lati = double.Parse(App.latitud);
            double longi = double.Parse(App.longitud);


            if ((lati != 0) & (longi != 0)) {

                var customMap = DependencyService.Get<IConexion>().TraeMapa();
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(lati, longi), Distance.FromMiles(0.5)));

                Content = customMap;
            }
        }
    }
}

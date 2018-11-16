using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MapsApp
{
    public partial class App : Application
    {

        public static string longitud = "0", latitud = "0";
        public App()
        {
            InitializeComponent();
            DependencyService.Get<IConexion>().Iniciar();
            MainPage = new NavigationPage(new Inicio());
            /*MainPage = new TabbedPage
            {
                Children =
                {
                    //new SampleMapPage(),
                    new CustomMapPage()
                }
            };*/
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

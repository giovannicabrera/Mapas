using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Xamarin.Forms.Maps;

namespace MapsApp.Droid
{
    class Controles
    {
        public static string rutaSD = "/storage/emulated/0/MicroApp";
        public static string nomArc = "info.xml";
        public static String rutaArchivo = Path.Combine(rutaSD, nomArc);
        public static string respuesta = "";

        public static void Inicio() {
            try
            {
                Java.IO.File ruta = new Java.IO.File(rutaSD);
                Java.IO.File archivo = new Java.IO.File(rutaArchivo);
                XDocument xdocini;
                if (!ruta.Exists()) ruta.Mkdirs();

                if (!archivo.Exists())
                {
                    XElement xDatos = new XElement("datos");
                    XElement xDato = new XElement("dato");
                    XAttribute xIme = new XAttribute("imei", "");
                    XAttribute xDet = new XAttribute("detalles", "");
                    XAttribute xOS = new XAttribute("os", "");
                    XAttribute xIP = new XAttribute("ip", "0.0.0.0");
                    XAttribute xErr = new XAttribute("errores", "");

                    xDato.Add(xIme); xDato.Add(xDet);
                    xDato.Add(xOS); xDato.Add(xIP);
                    xDato.Add(xErr); 
                    xDatos.Add(xDato);

                    xdocini = new XDocument();
                    xdocini.Add(xDatos);
                    GuardarInfo(xdocini, "datos");

                    XElement xPosiciones = new XElement("posicion");
                    xdocini = new XDocument();
                    xdocini.Add(xPosiciones);
                    GuardarInfo(xdocini, "posiciones");

                    XElement xBancos = new XElement("mensaje");
                    xdocini = new XDocument();
                    xdocini.Add(xBancos);
                    GuardarInfo(xdocini, "mensajes");

                    XElement xRutas = new XElement("ruta");
                    xdocini = new XDocument();
                    xdocini.Add(xRutas);
                    GuardarInfo(xdocini, "rutas");

                }
            }
            catch (Exception ex)
            {
                respuesta = ex.Message;
            }
        }

        public static void PosicionXML(string lat, string lon) {

            try
            {
                XDocument xdocp = LeerInfo("posiciones");
                XElement xPos = new XElement("posicion");
                XAttribute xLati = new XAttribute("latitud", lat);
                XAttribute xLong = new XAttribute("longitud", lon);

                xPos.Add(xLati);
                xPos.Add(xLong);                

                //Agregamos el pago al elemento root
                xdocp.Root.Add(new XElement(xPos));
                ActualizarInfo(xdocp, "posiciones");
            }
            catch (Exception ex) {
                respuesta = ex.Message;
            }
        }

        public static Modelo.CustomMap MapaXML()
        {           
            Modelo.CustomMap mapa = null;
            Modelo.CustomPin Pos;
            Position Posi;
            List<Modelo.CustomPin> Pines = new List<Modelo.CustomPin>();

            int contador = 0;
            string mlongitud = "", mlatitud= "";
            try
            {
                XDocument xdocp = LeerInfo("posiciones");                
                foreach (XElement element in xdocp.Root.Elements("posicion"))
                {
                    mlongitud = element.Attribute("longitud").Value.ToString();
                    mlatitud = element.Attribute("latitud").Value.ToString();
                    contador++;

                    Posi = new Position(Double.Parse(mlatitud), Double.Parse(mlongitud));
                    Pos = new Modelo.CustomPin
                    {
                        Pin = new Pin
                        {
                            Type = PinType.Place,
                            Position = Posi,
                            Label = "Pos " + contador,
                            Address = "Dir"
                        },
                        Url = "",
                    };
                    if (contador == 1) mapa = new Modelo.CustomMap() { MapType = MapType.Hybrid };
                    Pines.Add(Pos);
                }
                mapa.CustomPins = Pines;
            }
            catch (Exception ex)
            {
                respuesta = ex.Message;
            }
            return mapa;
        }

        //Procedimiento que guarda los datos en los archivos por primera vez
        private static void GuardarInfo(XDocument xdoc_guardar, string etiqueta)
        {
            XDocument xdoc_interno = null;
            try
            {
                string cadena_xdoc = xdoc_guardar.ToString();
                cadena_xdoc = Cifrado.Cripto(cadena_xdoc,"E");
                //cadena_xdoc = Cifrado.Cifrar(cadena_xdoc);

                if (etiqueta != "datos") xdoc_interno = XDocument.Load(rutaArchivo); else xdoc_interno = new XDocument();
                XElement xRoot = new XElement("informacion");
                XElement xInfo = new XElement("info");
                XAttribute xEle = new XAttribute("etiqueta", etiqueta);
                XAttribute xElb = new XAttribute("bytes", cadena_xdoc);
                xInfo.Add(xEle);
                xInfo.Add(xElb);
                if (etiqueta != "datos")
                {
                    xdoc_interno.Root.Add(xInfo);
                }
                else
                {
                    xRoot.Add(xInfo);
                    xdoc_interno.Add(xRoot);
                }
                xdoc_interno.Save(rutaArchivo);
                xdoc_interno = null;
            }
            catch (Exception ex)
            {
                xdoc_interno = null;
            }
        }

        //Procedimiento que actualiza los datos en los archivos
        private static void ActualizarInfo(XDocument xdoc_guardar, string etiqueta)
        {
            XDocument xdoc_interno = null;
            try
            {
                string cadena_xdoc = xdoc_guardar.ToString();
                cadena_xdoc = Cifrado.Cripto(cadena_xdoc, "E");
                //cadena_xdoc = Cifrado.Cifrar(cadena_xdoc);

                xdoc_interno = XDocument.Load(rutaArchivo);
                foreach (XElement xEle in xdoc_interno.Root.Elements("info"))
                {
                    if (xEle.Attribute("etiqueta").Value.ToString() == etiqueta)
                        xEle.SetAttributeValue("bytes", cadena_xdoc);
                }
                xdoc_interno.Save(rutaArchivo);                
            }
            catch (Exception ex)
            {
                xdoc_interno = null;
            }
        }

        //Función que devuelve la información de los archivos según la etiqueta indicada
        private static XDocument LeerInfo(string etiqueta)
        {
            XDocument xdoc_interno = null, xdoc_leido = null; string cadena_xdoc = "";
            try
            {
                xdoc_interno = XDocument.Load(rutaArchivo);
                foreach (XElement xEle in xdoc_interno.Root.Elements("info"))
                {
                    if (xEle.Attribute("etiqueta").Value.ToString() == etiqueta)
                        cadena_xdoc = xEle.Attribute("bytes").Value.ToString();
                }
                xdoc_interno = null;
                cadena_xdoc = Cifrado.Cripto(cadena_xdoc);
                //cadena_xdoc = Cifrado.Descifrar(cadena_xdoc);

                if (cadena_xdoc != null)
                xdoc_leido = XDocument.Parse(cadena_xdoc);
            }
            catch (Exception ex)
            {
                xdoc_leido = null;
            }
            return xdoc_leido;
        }

        private static void BorrarInfo(string etiqueta)
        {
            XDocument xdoc_interno = null;
            try
            {
                xdoc_interno = XDocument.Load(rutaArchivo);
                foreach (XElement xEle in xdoc_interno.Root.Elements("info"))
                {
                    if (xEle.Attribute("etiqueta").Value.ToString() == etiqueta)
                        xEle.Remove();
                }
                xdoc_interno.Save(rutaArchivo);
                xdoc_interno = null;
            }
            catch (Exception ex)
            {
                xdoc_interno = null;
            }
        }

    }
}

using System;
using System.Xml.Linq;

namespace MapsApp.Control
{
    class Controles
    {
        public static string rutaSD = "/storage/emulated/0/MicroApp";
        public static string nomArc = "info.xml";
        public static List<Modelo.Datos> regidatos = new List<Modelo.Datos>();
        public static string rutaArchivo = rutaSD + nomArc;
        public static string respuesta = "";

        public static void Inicio() {
            Inicial();
        }

        public string LeerPosicionesXML() {
            respuesta = "";
            try {
            } catch (Exception ex) {
                List<Modelo.Datos> posiciones = new List<Modelo.Datos>();
                XDocument xdoc_posiciones = XDocument.Load(rutaArchivo);
                posiciones = LeerXML(xdoc => xdoc_posiciones, elemento=>"posicion");
                foreach (var posicion in posiciones) {
                    posicion.latitud;
                }
            }
            return respuesta;
        }

        private static List<Modelo.Datos> LeerXML(XDocument xdoc, string elemento="", string atributo="")
        {
            respuesta = "";
            regidatos.Clear();
            string nombre_elemento = "", nombre_atributo = "", valor_elemento = "", valor_atributo = "";
            try {
                foreach (XElement raiz in xdoc.Root.Elements())
                {
                    if (raiz.HasElements) //Si el elemento raiz tiene elementos
                    {
                        foreach (XElement nivel01 in raiz.Elements()) //Recorremos a los elementos dependientes del nivel 1 
                        {
                            if (!nivel01.IsEmpty) nombre_elemento = nivel01.Name; //nivel01.SetValue(DLFDecodificar(nivel01.Value));
                            if (elemento == nombre_elemento) {
                                valor_elemento = nivel01.Value.ToString();
                                regidatos.Add(
                                    new Modelo.Datos()
                                    {
                                        nombre = nombre_elemento,
                                        valor = valor_elemento,
                                        tipo = "elemento"
                                    }
                                );
                            }
                            foreach (XAttribute atributos01 in nivel01.Attributes()) {
                                nombre_atributo = atributos01.Name;
                                if ((atributo == nombre_atributo) || (elemento == nombre_elemento))
                                {
                                    valor_atributo = nivel01.Value.ToString();
                                    regidatos.Add(
                                        new Modelo.Datos()
                                        {
                                            nombre = nombre_atributo,
                                            valor = valor_atributo,
                                            tipo = "atributo"
                                        }
                                    );
                                }
                                valor_atributo = ""; nombre_atributo = "";
                                valor_elemento = ""; nombre_elemento = "";
                                //atributos01.SetValue(DLFDecodificar(atributos01.Value));
                            } //Recorremos a los atributos de cada elemento del nivel 1 

                            if (nivel01.HasElements) //Si el elemento de nivel 1 tiene elementos
                            {
                                foreach (XElement nivel02 in nivel01.Elements()) //Recorremos a los elementos dependientes del nivel 2 
                                {
                                    if (!nivel02.IsEmpty) nombre_elemento = nivel02.Name; //nivel02.SetValue(DLFDecodificar(nivel02.Value));
                                    if (elemento == nombre_elemento)
                                    {
                                        valor_elemento = nivel01.Value.ToString();
                                        regidatos.Add(
                                            new Modelo.Datos()
                                            {
                                                nombre = nombre_elemento,
                                                valor = valor_elemento,
                                                tipo = "elemento"
                                            }
                                        );                                        
                                    }
                                    foreach (XAttribute atributos02 in nivel02.Attributes()) {
                                        nombre_atributo = atributos02.Name;
                                        if ((atributo == nombre_atributo) || (elemento == nombre_elemento))
                                        {
                                            valor_atributo = nivel01.Value.ToString();
                                            regidatos.Add(
                                                new Modelo.Datos()
                                                {
                                                    nombre = nombre_atributo,
                                                    valor = valor_atributo,
                                                    tipo = "atributo"
                                                }
                                            );                                            
                                        }
                                        valor_atributo = ""; nombre_atributo = "";
                                        valor_elemento = ""; nombre_elemento = "";
                                        //atributos02.SetValue(DLFDecodificar(atributos02.Value));
                                    } //Recorremos a los atributos de cada elemento del nivel 2
                                }
                            }
                        }
                    }
                    if (raiz.HasAttributes)
                    {
                        foreach (XAttribute atributos in raiz.Attributes()) {
                            nombre_atributo = atributos.Name;
                            //atributos.SetValue(DLFDecodificar(atributos.Value));
                        }//Recorremos a los atributos de cada elemento del elemento raiz
                    }
                }
            }
            catch (Exception ex) {
                regidatos.Add(new Modelo.Datos() { nombre = "error", valor = "No existen registros a procesar ["+ex.Message+"]", tipo = "Error" });
            }
            return regidatos;
        }


        //Función que crea los archivos cuando no existen
        private static void Inicial()
        {
            try
            {
                Java.IO.File ruta = new Java.IO.File(rutaSD);
                Java.IO.File archivo = new Java.IO.File(rutaArchivo);
                XDocument xdoc_dat;
                XElement xRoot, xInfo ;
                XAttribute xEle, xElb ;
                if (!ruta.Exists()) ruta.Mkdirs();

                if (!archivo.Exists())
                {
                    xdoc_dat = new XDocument();
                    xRoot = new XElement("informacion");
                    xInfo = new XElement("info");
                    xEle = new XAttribute("etiqueta", "posiciones");
                    xElb = new XAttribute("bytes", "");
                    xInfo.Add(xEle);
                    xInfo.Add(xElb);
                    xRoot.Add(xInfo);
                    xdoc_dat.Add(xRoot);
                    xdoc_dat.Save(rutaArchivo);                    
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }            
        }
    }
}

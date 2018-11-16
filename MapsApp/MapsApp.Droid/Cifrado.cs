using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.IO;

namespace MapsApp
{

    /*
       Constantes
       contrasena: Nuestra contraseña.
       semilla: Llave de encriptacion.
       algoritmo: El algoritmo para generar el hash, puede ser MD5 o SHA1.
       iteraciones: Cantidad de iteraciones de encriptacion.
       hexa: Llave de encriptacion. Debe ser una cadena de 16 caracteres. 
       tipoLlave: Puede ser cualquiera de estos tres valores: 128, 192 o 256.
     */
    static class Cons
    {
        public const int factor = 10;
        public const int tipoLlave = 256;
        public const int iteraciones = 10;
        public const string hexa = "494E464F4D4F56494C";
        public const string semilla = "4D4F56494C415050";
        public const string algoritmo = "MD5";
        public const string contrasena = "434C41564553454352455441";
    }

    public class Cifrado
    {
        public static string Cripto(string clave, string tipo = "D") {
            string llave = "";
            byte[] cadenaCifradaBytes;

            if (tipo == "D")
            {
                //llave = AlgoritmoFactor(clave, tipo);
                cadenaCifradaBytes = Convert.FromBase64String(clave);
                llave = UTF8Encoding.UTF8.GetString(cadenaCifradaBytes);

            }
            else {
                //llave = AlgoritmoFactor(clave, tipo);
				llave = clave;

                cadenaCifradaBytes = UTF8Encoding.UTF8.GetBytes(clave);
                llave = Convert.ToBase64String(cadenaCifradaBytes, 0, cadenaCifradaBytes.Length);

            }
            return llave;
        }

        private static string AlgoritmoFactor(string cadena, string tipo="D") {
            string cadenafinal = "", unidad = "", tres = "";
            int numero = 0, contador = 0;

            try
            {
                cadenafinal = ""; unidad = ""; tres = "";
                numero = 0; contador = 0;
                if (tipo == "D") {
                    while (int.Parse(cadena.Substring(0,2))>0) {
                        numero = ((((int.Parse(cadena.Substring(0, 2)) + Cons.factor) * Cons.factor) - Cons.factor) / Cons.factor);
                        unidad = (Convert.ToChar(numero)).ToString();
                        cadenafinal = cadenafinal + unidad;
                        cadenafinal = cadenafinal.Substring(3, cadenafinal.Length - 3);
                    }
                }
                else {
                    cadena = cadena.ToUpper();
                    byte[] ASCIIValues = Encoding.ASCII.GetBytes(cadena);
                    foreach (byte b in ASCIIValues)
                    {
                        contador++;
                        numero = ((((int.Parse(b.ToString()) * Cons.factor) + Cons.factor) / Cons.factor) - Cons.factor);
                        cadenafinal = cadenafinal + numero;
                        if (numero < 48) numero = 48;
                        if ((numero < 65) & (numero > 57)) numero = 65;
                        unidad = (Convert.ToChar(numero)).ToString();
                        if (contador <= 3) tres = tres + unidad;
                    }
                    cadenafinal = tres + cadenafinal;
                }
            }
            catch (Exception ex) {
                cadenafinal = ex.Message;
            }
            return cadenafinal;
        }

        /*
        **********************************************************************************************
        DESCIFRAR: 
        Función que devuelve cadena en claro luego de descifrar cadena cifrada con encriptacion AES,
        Parametros
        cadena: Texto a descifrar.
        [Giovanni Cabrera - 29 de Julio de 2017]
        **********************************************************************************************
        */
        public static string Descifrar(string cadena)
        {
            string cadenaCifrada;
            try
            {
                byte[] keyBytes;
                byte[] cadenaBytes;

                byte[] hexaBytes = Encoding.ASCII.GetBytes(Cons.hexa);
                byte[] semillaBytes = Encoding.ASCII.GetBytes(Cons.semilla);

                byte[] cadenaCifradaBytes = Convert.FromBase64String(cadena);

                PasswordDeriveBytes clave = new PasswordDeriveBytes(Cons.contrasena, semillaBytes, Cons.algoritmo, Cons.iteraciones);

                keyBytes = clave.GetBytes(Cons.tipoLlave / 8);

                RijndaelManaged llave = new RijndaelManaged();

                llave.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = llave.CreateDecryptor(keyBytes, hexaBytes);

                MemoryStream cadenaEnMemoria = new MemoryStream(cadenaCifradaBytes);

                CryptoStream cadenaEncriptadora = new CryptoStream(cadenaEnMemoria, decryptor, CryptoStreamMode.Read);

                cadenaBytes = new byte[cadenaCifradaBytes.Length];

                int decryptedByteCount = cadenaEncriptadora.Read(cadenaBytes, 0, cadenaBytes.Length);

                cadenaEnMemoria.Close();
                cadenaEncriptadora.Close();

                string cadenaDescifrada = Encoding.UTF8.GetString(cadenaBytes, 0, decryptedByteCount);

                return cadenaDescifrada;
            }
            catch(Exception ex)
            {
                cadenaCifrada = "No fue posible realizar la desencriptación del texto ["+ ex.Message +"]";
                return cadenaCifrada;
            }
        }

        /*
        ************************************************************************************
        CIFRAR: 
        Función que devuelve cadena cifrada encriptacion AES
        Parametros
        cadena: Texto a cifrar.
        [Giovanni Cabrera - 29 de Julio de 2017]
        ************************************************************************************
        */
        public static string Cifrar(string cadena)
        {
            string cadenaCifrada;
            try
            {
                byte[] keyBytes;
                byte[] cadenaCifradaBytes;

                byte[] hexaBytes = Encoding.ASCII.GetBytes(Cons.hexa);
                byte[] semillaBytes = Encoding.ASCII.GetBytes(Cons.semilla);
                byte[] cadenaBytes = Encoding.UTF8.GetBytes(cadena);

                PasswordDeriveBytes clave = new PasswordDeriveBytes(Cons.contrasena, semillaBytes, Cons.algoritmo, Cons.iteraciones);

                keyBytes = clave.GetBytes(Cons.tipoLlave / 8);

                RijndaelManaged llave = new RijndaelManaged();

                llave.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = llave.CreateEncryptor(keyBytes, hexaBytes);

                MemoryStream cadenaEnMemoria = new MemoryStream();

                CryptoStream cadenaEncriptadora = new CryptoStream(cadenaEnMemoria, encryptor, CryptoStreamMode.Write);

                cadenaEncriptadora.Write(cadenaBytes, 0, cadenaBytes.Length);

                cadenaEncriptadora.FlushFinalBlock();

                cadenaCifradaBytes = cadenaEnMemoria.ToArray();

                cadenaEnMemoria.Close();
                cadenaEncriptadora.Close();

                cadenaCifrada = Convert.ToBase64String(cadenaCifradaBytes);

                return cadenaCifrada;
            }
            catch(Exception ex)
            {
                cadenaCifrada = "No fue posible realizar la encriptación del texto ["+ ex.Message +"]";
                return cadenaCifrada;
            }
        }

    }
}
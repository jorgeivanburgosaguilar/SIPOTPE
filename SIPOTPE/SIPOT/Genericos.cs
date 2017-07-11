using System;
using System.Text;
using System.Xml;

namespace SIPOTPE.SIPOT
{
    public static class Genericos
    {
        public static int ConvertirCadenaAEntero(string cadena, int valorPorDefecto = 0)
        {
            try
            {
                return string.IsNullOrWhiteSpace(cadena.Trim()) ? valorPorDefecto : Convert.ToInt32(cadena.Trim());
            }
            catch
            {
                return valorPorDefecto;
            }
        }

        public static void EliminarUltimoCaracter(StringBuilder cadena)
        {
            var largoCadena = cadena.Length;
            if (largoCadena <= 0)
                return;

            cadena.Remove(largoCadena - 1, 1);
        }

        public static string EscaparCadenaParaXML(string cadena)
        {
            var documento = new XmlDocument();
            var nodo = documento.CreateElement("root");
            nodo.InnerText = cadena;
            return nodo.InnerXml;
        }
    }
}

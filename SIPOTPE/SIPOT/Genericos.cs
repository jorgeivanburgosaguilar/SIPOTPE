using System;
using System.Text;

namespace SIPOTPE.SIPOT
{
    public static class Genericos
    {
        public static int ConvertirCadenaAEntero(string cadena)
        {
            try
            {
                return string.IsNullOrWhiteSpace(cadena) ? 0 : Convert.ToInt32(cadena.Trim());
            }
            catch
            {
                return 0;
            }
        }

        public static void EliminarUltimoCaracter(StringBuilder cadena)
        {
            var largoCadena = cadena.Length;
            if (largoCadena <= 0)
                return;

            cadena.Remove(largoCadena - 1, 1);
        }
    }
}

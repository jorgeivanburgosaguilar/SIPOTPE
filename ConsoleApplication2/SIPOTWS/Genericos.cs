using System;

namespace ConsoleApplication2.SIPOTWS
{
    public static class Genericos
    {
        public static int ConvertirCadenaAEntero(string cadena)
        {
            try
            {
                return string.IsNullOrWhiteSpace(cadena) ? 0 : Convert.ToInt32(cadena);
            }
            catch
            {
                return 0;
            }
        }
    }
}

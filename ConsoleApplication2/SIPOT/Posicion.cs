using System;
using System.Linq;

namespace SIPOTPE.SIPOT
{
    [Serializable]
    public class Posicion
    {
        public string Hoja { get; set; }
        public int Columna { get; set; }
        public int Fila { get; set; }

        public Posicion()
        {
            Hoja = string.Empty;
            Columna = 0;
            Fila = 0;
        }

        public Posicion(string hoja, int columna, int fila)
        {
            Hoja = hoja;
            Columna = columna;
            Fila = fila;
        }

        /// <remarks>Columna y Fila deben estar en base a 0</remarks>
        public static string ObtenerParaExcel(string hoja, int columna, int fila)
        {
            var dividendo = columna + 1;
            var columnaParaExcel = string.Empty;

            while (dividendo > 0)
            {
                var modulo = (dividendo - 1) % 26;
                columnaParaExcel = Convert.ToChar(65 + modulo) + columnaParaExcel;
                dividendo = (dividendo - modulo) / 26;
            }

            var hojaParaExcel = hoja.Any(char.IsWhiteSpace) ? string.Format("'{0}'", hoja) : hoja;
            var filaParaExcel = fila + 1;

            return string.Format("{0}!{1}{2}", hojaParaExcel, columnaParaExcel, filaParaExcel);
        }

        public string ParaExcel()
        {
            return ObtenerParaExcel(Hoja, Columna, Fila);
        }

        public override string ToString()
        {
            return ParaExcel();
        }
    }
}

using System;
using System.Linq;

namespace ConsoleApplication2.SIPOTWS
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

        public override string ToString()
        {
            var hoja = Hoja.Any(char.IsWhiteSpace) ? string.Format("'{0}'", Hoja) : Hoja;
            return string.Format("{0}!{1}{2}", hoja, Columna, Fila);
        }
    }
}

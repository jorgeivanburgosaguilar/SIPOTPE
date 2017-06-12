using System;
using System.Linq;

namespace ConsoleApplication2.SIPOTWS
{
    [Serializable]
    public class Posicion
    {
        public string Hoja { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Posicion()
        {
            Hoja = string.Empty;
            X = 0;
            Y = 0;
        }

        public Posicion(string hoja, int x, int y)
        {
            Hoja = hoja;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            var hoja = Hoja.Any(char.IsWhiteSpace) ? string.Format("'{0}'", Hoja) : Hoja;
            return string.Format("{0}!{1}{2}", hoja, X, Y);
        }
    }
}

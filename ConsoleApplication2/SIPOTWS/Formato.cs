using System.Collections.Generic;

namespace ConsoleApplication2.SIPOTWS
{
    public class Formato
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public List<Campo> Campos { get; set; }

        public Formato()
        {
            ID = 0;
            Nombre = string.Empty;
            Campos = new List<Campo>();
        }

        public Formato(int id, string nombre)
        {
            ID = id;
            Nombre = nombre;
            Campos = new List<Campo>();
        }
    }
}

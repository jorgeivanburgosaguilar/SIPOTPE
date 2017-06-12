using System;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS
{
    [Serializable]
    public class Error
    {
        public TipoError Tipo { get; set; }
        public Posicion Posicion { get; set; }
        public string Mensaje { get; set; }

        public Error()
        {
            Tipo = TipoError.Informativo;
            Posicion = new Posicion();
            Mensaje = string.Empty;
        }

        public Error(TipoError tipo, Posicion posicion, string mensaje)
        {
            Tipo = tipo;
            Posicion = posicion;
            Mensaje = mensaje;
        }

        public override string ToString()
        {
            return string.Format("({2}) {0}: {1}", Tipo.Descripcion(), Mensaje, Posicion);
        }
    }
}

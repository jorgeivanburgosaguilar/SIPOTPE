using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS
{
    public class Error
    {
        public TipoError Tipo { get; set; }
        public string Posicion { get; set; }
        public string Mensaje { get; set; }

        public Error()
        {
            Tipo = TipoError.Informativo;
            Posicion = string.Empty;
            Mensaje = string.Empty;
        }

        public Error(TipoError tipo, string posicion, string mensaje)
        {
            Tipo = tipo;
            Posicion = posicion;
            Mensaje = mensaje;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} ({2})", Tipo.Descripcion(), Mensaje, Posicion);
        }
    }
}

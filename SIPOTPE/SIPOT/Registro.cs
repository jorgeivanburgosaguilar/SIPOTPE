using System;

namespace SIPOTPE.SIPOT
{
    [Serializable]
    public class Registro
    {
        private string _valor;

        public int Numero { get; set; }
        public Posicion Posicion { get; set; }

        public string Valor
        {
            get { return _valor; }
            set { _valor = string.IsNullOrWhiteSpace(value) ? string.Empty : value; }
        }

        public Registro()
        {
            Numero = 0;
            Posicion = new Posicion();
            Valor = string.Empty;
        }

        public override string ToString()
        {
            return Valor;
        }
    }
}

namespace ConsoleApplication2.SIPOTWS
{
    public class Registro
    {
        private string _valor;

        public int Numero { get; set; }
        public string Posicion { get; set; }

        public string Valor
        {
            get { return _valor; }
            set { _valor = string.IsNullOrWhiteSpace(value) ? string.Empty : value; }
        }

        public Registro()
        {
            Numero = 0;
            Posicion = string.Empty;
            Valor = string.Empty;
        }

        public Registro(int numero, string posicion, string valor)
        {
            Numero = numero;
            Posicion = posicion;
            Valor = valor;
        }
    }
}

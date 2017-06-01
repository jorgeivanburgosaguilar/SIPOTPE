using System;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS
{
    public class Registro
    {
        public int Numero { get; set; }
        public string Posicion { get; set; }
        public string Valor { get; private set; }

        public Registro()
        {
            Numero = 0;
            Valor = string.Empty;
            Posicion = string.Empty;
        }

        public void EstablecerValor(string valor, TipoCampo tipoCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                Valor = string.Empty;
                return;
            }

            var tmpValor = valor.Trim();

            switch (tipoCampo)
            {
                case TipoCampo.Numero:
                    tmpValor = valor.Replace(",", "");
                    break;

                case TipoCampo.FechaActualizacion:
                case TipoCampo.Fecha:
                    DateTime fecha;
                    if (DateTime.TryParse(tmpValor, out fecha))
                        tmpValor = fecha.ToString("yyyy-MM-dd");
                    break;

                case TipoCampo.Moneda:
                    tmpValor = valor.Replace(",", "");
                    if (Regex.IsMatch(tmpValor.Substring(Math.Max(0, tmpValor.Length - 2)), @"\A\.\d\Z"))
                        tmpValor += "0";
                    break;
            }

            Valor = tmpValor;
        }
    }
}

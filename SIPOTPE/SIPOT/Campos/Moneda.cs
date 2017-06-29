using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("monedas", "moneda")]
    public class Moneda : Campo
    {
        public Moneda()
        {
            Tipo = TipoCampo.Moneda;
            ValorPorDefecto = "0.00";
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Advertencia, posicion, "El valor monetario no debe estar vacio"));
                return errores;
            }

            // Esto valida que sea un numero valido mas no que tenga 2 decimales, tomar en cuenta antes de pasarlo al XML
            if (!Regex.IsMatch(valor, @"[\-+]?[0-9]{1,12}([.][0-9]{1,2})?"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "El valor monetario tiene un formato incorrecto. El valor monetario debe tener un numero de maximo de 12 digitos con maximo 2 digitos decimales, Ejemplos: 123456789012, 123456789012.1 y 123456789012.10"));

            return errores;
        }

        public override string ObtenerValorRegistroParaXML(Registro registro)
        {
            if (ValidarRegistro(registro).Count > 0)
                return ValorPorDefecto;

            try
            {
                return string.Format("{0:0.00}", double.Parse(registro.Valor));
            }
            catch
            {
                Debug.WriteLine("Error Valor Tipo Moneda: {0}", registro.Valor);
                return ValorPorDefecto;
            }
        }
    }
}

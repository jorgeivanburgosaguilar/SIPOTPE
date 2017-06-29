using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("numeros", "numero")]
    public class Numero : Campo
    {
        public Numero()
        {
            Tipo = TipoCampo.Numero;
            ValorPorDefecto = "0";
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Advertencia, posicion, "El valor numerico no debe estar vacio"));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"[\-+]?[0-9]{1,12}([.][0-9]{1,2})?"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "El valor numerico es invalido. El valor numerico debe tener un numero de maximo de 12 digitos con 2 digitos decimales opcionales, Ejemplo(s): 123456789012, 123456789012.1 y 123456789012.10"));

            return errores;
        }
    }
}

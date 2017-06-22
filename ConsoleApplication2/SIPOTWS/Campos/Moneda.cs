using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Moneda : Campo
    {
        public Moneda()
        {
            ValorPorDefecto = "0.00";
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Grave, posicion, "El valor monetario no puede estar vacio"));
                return errores;
            }

            // ToDo esto valida que tenga o no 2 digitos decimales, lo que contradice a la validacion
            // ToDo hay que poner una adecuacion para que añada esos decimales al convertir a XML
            if (!Regex.IsMatch(valor, @"[\-+]?[0-9]{1,12}([.][0-9]{1,2})?"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "El valor monetario tiene un formato incorrecto. El valor monetario debe tener un numero de maximo de 12 digitos con 2 digitos decimales opcionales, Ejemplo(s): 123456789012, 123456789012.1 y 123456789012.10"));

            return errores;
        }
    }
}

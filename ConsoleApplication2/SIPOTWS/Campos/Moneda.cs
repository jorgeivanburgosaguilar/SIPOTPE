using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Moneda : Campo
    {
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

            if (!Regex.IsMatch(valor, @"\A[\-+]?[0-9]{1,12}[.][0-9]{2}\Z"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "El valor monetario tiene un formato incorrecto. El valor monetario debe tener un numero de maximo de 12 digitos con 2 digitos decimales, Ejemplo: 123456789012.01"));

            return errores;
        }
    }
}

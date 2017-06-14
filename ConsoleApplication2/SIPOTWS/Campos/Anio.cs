using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Anio : Campo
    {
        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Grave, posicion, "El año no puede estar vacio"));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\A\d{4}\Z"))
                errores.Add(new Error(TipoError.Grave, posicion, "El año tiene un formato invalido. El año debe llenarse con 4 digitos decimales, Ejemplo: 2017"));

            return errores;
        }
    }
}

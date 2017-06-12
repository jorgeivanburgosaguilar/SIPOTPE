using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Hora : Campo
    {
        public override List<Error> Validar(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Grave, posicion, "La hora no puede estar vacia"));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\A(?:([0-1][0-9]|2[1-3]):[0-5][0-9])\Z"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "La hora tiene un formato incorrecto. Formato: Horas:Minutos, Ejemplo: 23:59"));

            return errores;
        }
    }
}

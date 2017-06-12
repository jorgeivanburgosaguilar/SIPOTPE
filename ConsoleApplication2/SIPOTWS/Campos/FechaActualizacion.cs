using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class FechaActualizacion : Campo
    {
        public override List<Error> Validar(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Informativo, posicion, "El registro esta vacio"));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\A(?:(0[1-9]|[12][0-9]|3[01])[/](0[1-9]|1[012])[/](\d{4}))\Z"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "La fecha tiene un formato incorrecto. Las fechas deben tener el formato Dia/Mes/Año, Ejemplo: 01/09/2017"));

            return errores;
        }
    }
}

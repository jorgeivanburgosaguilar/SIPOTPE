using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("horas", "hora")]
    public class Hora : Campo
    {
        public Hora()
        {
            Tipo = TipoCampo.Hora;
            ValorPorDefecto = "00:00";
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Informativo, posicion, "El registro esta vacio."));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\A(?:([0-1][0-9]|2[1-3]):[0-5][0-9])\Z"))
                errores.Add(new Error(TipoError.Grave, posicion,
                    "La hora tiene un formato incorrecto. Formato: Horas:Minutos, Ejemplo: 23:59"));

            return errores;
        }
    }
}

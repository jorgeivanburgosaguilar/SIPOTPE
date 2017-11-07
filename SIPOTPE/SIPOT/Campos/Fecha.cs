using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("fechas", "fecha")]
    public class Fecha : Campo
    {
        public Fecha()
        {
            Tipo = TipoCampo.Fecha;
        }

        public override List<Error> ValidarRegistro(Registro registro)
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
            {
                errores.Add(new Error(TipoError.Grave, posicion,
                    "La fecha tiene un formato incorrecto. Las fechas deben tener el formato Dia/Mes/Año, Ejemplo: 01/09/2017"));
                return errores;
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var tmp = DateTime.ParseExact(valor, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                errores.Add(new Error(TipoError.Grave, posicion, "La fecha es invalida. Las fechas deben ser validas segun el calendario gregoriano."));
            }

            return errores;
        }

        public override string ObtenerValorRegistroParaXML(Registro registro)
        {
            if (ValidarRegistro(registro).Count > 0)
                return ValorPorDefecto;

            var dtFecha = DateTime.ParseExact(registro.Valor, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dtFecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Decoradores;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [NombresXML("anios", "anio")]
    public class Anio : Campo
    {
        public Anio()
        {
            ValorPorDefecto = DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture);
        }

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
            {
                errores.Add(new Error(TipoError.Grave, posicion,
                    "El año tiene un formato invalido. El año debe llenarse con 4 digitos decimales, Ejemplo: 2017"));
                return errores;
            }

            // Validar que el año este entre el año 2000 y el año actual
            var valorAnio = Genericos.ConvertirCadenaAEntero(valor);
            if (valorAnio < 2000 || valorAnio > DateTime.Now.Year)
                errores.Add(new Error(TipoError.Grave, posicion, string.Format("El año solo puede establecerse entre el año 2000 y el año {0}", DateTime.Now.Year)));

            return errores;
        }

        public override string ObtenerValorRegistroParaXML(Registro registro)
        {
            if (ValidarRegistro(registro).Count > 0)
                throw new Exception("El Año contiene un valor invalido");

            return registro.Valor;
        }
    }
}

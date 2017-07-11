using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("anios", "anio")]
    public class Anio : Campo
    {
        public Anio()
        {
            Tipo = TipoCampo.Anio;
            ValorPorDefecto = DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture);
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Critico, posicion, "El año no puede estar vacio"));
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

        public override List<Error> ValidarRegistros()
        {
            var errores = new List<Error>();
            var registrosSinError = new List<string>();

            foreach (var registro in Registros)
            {
                var lstErroresRegistro = ValidarRegistro(registro);
                if (lstErroresRegistro.Count > 0)
                    errores.AddRange(lstErroresRegistro);
                else
                    registrosSinError.Add(registro.ToString());
            }

            // Detectar multiples años en el mismo formato
            if (registrosSinError.Count > 0 && registrosSinError.Distinct().Count() != 1)
                errores.Add(new Error(TipoError.Advertencia, Posicion,
                    "El formato solo debe contener registros de un año en especifico, establecer multiples años puede ocasionar perdida de información al reemplazar la información existente en el SIPOT."));

            return errores;
        }
    }
}

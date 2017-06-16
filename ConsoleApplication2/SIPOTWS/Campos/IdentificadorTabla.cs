using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class IdentificadorTabla : Campo
    {
        public override List<Error> Validar()
        {
            // El campo IdentificadorTabla no implementa ninguna validacion
            // en el ID o nombre del campo.
            return new List<Error>();
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Grave, posicion, "El Identificador de la Tabla no puede estar vacio."));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\d+"))
                errores.Add(new Error(TipoError.Grave, posicion, "El Identificador de la Tabla debe ser un valor numerico."));

            return errores;
        }

        public override List<Error> ValidarRegistros()
        {
            var errores = new List<Error>();

            // Validamos los ID's duplicados de las tablas solo si el registro es valido.
            var lista = new List<string>();
            foreach (var registro in Registros)
            {
                var erroresRegistro = ValidarRegistro(registro);
                if (erroresRegistro.Count > 0)
                {
                    errores.AddRange(erroresRegistro);
                }
                else
                {
                    if (lista.Contains(registro.Valor))
                        errores.Add(new Error(TipoError.Grave, registro.Posicion, "El Valor del Identificador de la Tabla esta duplicado."));
                    else
                        lista.Add(registro.Valor);
                }
            }

            return errores;
        }
    }
}

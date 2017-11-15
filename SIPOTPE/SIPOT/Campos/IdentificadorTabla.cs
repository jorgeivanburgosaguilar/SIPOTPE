using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("identificadoresTablas", "identificadorTabla", false)]
    public class IdentificadorTabla : Campo
    {
        public IdentificadorTabla()
        {
            Tipo = TipoCampo.IdentificadorTabla;
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Grave, posicion, "El Identificador de la Tabla no debe estar vacio."));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\d+"))
                errores.Add(new Error(TipoError.Grave, posicion, "El Identificador de la Tabla debe ser un valor numerico."));

            return errores;
        }

        public override List<Error> Validar()
        {
            // El campo IdentificadorTabla no implementa ninguna validacion
            // en el ID o Nombre del campo, por lo que solo se validan
            // sus registros
            return ValidarRegistros();
        }
    }
}

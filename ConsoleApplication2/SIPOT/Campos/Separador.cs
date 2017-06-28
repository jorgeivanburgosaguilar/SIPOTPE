using System;
using System.Collections.Generic;
using SIPOTPE.SIPOT.Campos.Decoradores;

namespace SIPOTPE.SIPOT.Campos
{
    /// <remarks>
    /// El campo separador se procesa mas no se valida.
    /// </remarks>
    [Serializable]
    [NombresXML("separadores", "separador")]
    public class Separador : Campo
    {
        public override List<Error> Validar()
        {
            return new List<Error>();
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            return new List<Error>();
        }

        public override List<Error> ValidarRegistros()
        {
            return new List<Error>();
        }

        public override string HaciaXML()
        {
            return string.Empty;
        }
    }
}

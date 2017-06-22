using System;
using System.Collections.Generic;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    /// <remarks>
    /// El campo separador se procesa mas no se valida.
    /// </remarks>
    [Serializable]
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
    }
}

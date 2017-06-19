using System;
using System.Collections.Generic;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Separador : Campo
    {
        /// <remarks>El campo separador no se valida.</remarks>
        public override List<Error> Validar()
        {
            return new List<Error>();
        }

        /// <remarks>Los registros del campo separador no se validan.</remarks>
        public override List<Error> ValidarRegistro(Registro registro)
        {
            return new List<Error>();
        }

        /// <remarks>Los registros del campo separador no se validan.</remarks>
        public override List<Error> ValidarRegistros()
        {
            return new List<Error>();
        }
    }
}

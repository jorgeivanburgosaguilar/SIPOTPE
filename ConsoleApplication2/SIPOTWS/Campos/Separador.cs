using System;
using System.Collections.Generic;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Separador : Campo
    {
        public override List<Error> ValidarRegistro(Registro registro)
        {
            // El campo separador no se debe validar
            return new List<Error>();
        }
    }
}

using System;
using System.Collections.Generic;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Catalogo : Campo
    {
        public override List<Error> Validar(Registro registro)
        {
            // ToDo Implementar validacion
            return new List<Error>();
        }
    }
}

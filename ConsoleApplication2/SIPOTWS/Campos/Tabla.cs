using System;
using System.Collections.Generic;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Tabla : Campo
    {
        public List<Campo> Campos { get; set; }

        public Tabla()
        {
            Campos = new List<Campo>();
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            // ToDo Implementar validacion
            return new List<Error>();
        }
    }
}

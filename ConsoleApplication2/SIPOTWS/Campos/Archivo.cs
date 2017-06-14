using System;
using System.Collections.Generic;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Archivo : Campo
    {
        public override List<Error> ValidarRegistro(Registro registro)
        {
            var error = new List<Error>
            {
                new Error(TipoError.Critico, registro.Posicion,
                    "El tipo de campo archivo no puede ser procesado por esta aplicación, comuníquese con el área de soporte.")
            };
            return error;
        }
    }
}

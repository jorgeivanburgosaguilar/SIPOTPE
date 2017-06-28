using System;
using System.Collections.Generic;
using SIPOTPE.SIPOT.Campos.Decoradores;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [NombresXML("archivos", "archivo")]
    public class Archivo : Campo
    {
        private List<Error> ErrorPorDefecto()
        {
            return new List<Error>
            {
                new Error(TipoError.Critico, Posicion,
                    "El tipo de campo archivo no puede ser procesado por esta aplicación, comuníquese con el área de soporte.")
            };
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            return ErrorPorDefecto();
        }

        public override List<Error> ValidarRegistros()
        {
            return ErrorPorDefecto();
        }

        public override List<Error> Validar()
        {
            return ErrorPorDefecto();
        }
    }
}

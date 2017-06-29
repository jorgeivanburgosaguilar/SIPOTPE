using System;
using System.Collections.Generic;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("archivos", "archivo", false)]
    public class Archivo : Campo
    {
        public Archivo()
        {
            Tipo = TipoCampo.Archivo;
        }

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

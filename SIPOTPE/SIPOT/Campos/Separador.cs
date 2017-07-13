using System;
using System.Collections.Generic;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    /// <remarks>
    /// El campo separador se procesa mas no se valida.
    /// </remarks>
    [Serializable]
    [ConfiguracionesXML("separadores", "separador", false)]
    public class Separador : Campo
    {
        public Separador()
        {
            Tipo = TipoCampo.Separador;
        }

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

using System.ComponentModel;

namespace SIPOTPE.SIPOT.Enumeradores
{
    public enum TipoError
    {
        [Description("Informativo")]
        Informativo = 1,

        [Description("Advertencia")]
        Advertencia = 2,

        [Description("Grave")]
        Grave = 3,

        [Description("Crítico")]
        Critico = 4
    }
}

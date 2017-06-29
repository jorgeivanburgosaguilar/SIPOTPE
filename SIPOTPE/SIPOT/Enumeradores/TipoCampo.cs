using System.ComponentModel;

namespace SIPOTPE.SIPOT.Enumeradores
{
    public enum TipoCampo
    {
        [Description("Desconocido")]
        Desconocido = 0,

        [Description("Texto Corto")]
        TextoCorto = 1,

        [Description("Texto Largo")]
        TextoLargo = 2,

        [Description("Número")]
        Numero = 3,

        [Description("Fecha")]
        Fecha = 4,

        [Description("Hora")]
        Hora = 5,

        [Description("Moneda")]
        Moneda = 6,

        [Description("Pagina Web")]
        PaginaWeb = 7,

        [Description("Archivo")]
        Archivo = 8,

        [Description("Catalogo")]
        Catalogo = 9,

        [Description("Tabla")]
        Tabla = 10,

        [Description("Separador")]
        Separador = 11,

        [Description("Año")]
        Anio = 12,

        [Description("Fecha de Actualización")]
        FechaActualizacion = 13,

        [Description("Nota")]
        Nota = 14,

        // Tipo campo invalido especial para el ID de la tabla
        [Description("Identificador de la Tabla")]
        IdentificadorTabla = -99
    }
}

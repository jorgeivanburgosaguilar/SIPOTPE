using System.ComponentModel;
using ConsoleApplication2.SIPOTWS.Decoradores;

namespace ConsoleApplication2.SIPOTWS.Enumeradores
{
    public enum TipoCampo
    {
        [NoValidar]
        [Description("Inexistente o Nulo")]
        Nulo = 0,

        [Description("Texto Corto")]
        TextoCorto = 1,

        [Description("Texto Largo")]
        TextoLargo = 2,

        [Description("Numero")]
        Numero = 3,

        [Description("Fecha")]
        Fecha = 4,

        [Description("Hora")]
        Hora = 5,

        [Description("Moneda")]
        Moneda = 6,

        [Description("Pagina Web")]
        PaginaWeb = 7,

        [NoValidar]
        [Description("Archivo")]
        Archivo = 8,

        [Description("Catalogo")]
        Catalogo = 9,

        [Description("Tabla")]
        Tabla = 10,

        [NoValidar]
        [Description("Separador")]
        Separador = 11,

        [Description("Año")]
        Anio = 12,

        [Description("Fecha de Actualizacion")]
        FechaActualizacion = 13,

        [PuedeEstarVacio]
        [Description("Nota")]
        Nota = 14
    }
}

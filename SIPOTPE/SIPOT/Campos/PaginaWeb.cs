using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("paginasWeb", "pagina")]
    public class PaginaWeb : Campo
    {
        public PaginaWeb()
        {
            Tipo = TipoCampo.PaginaWeb;
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Informativo, posicion, "El registro esta vacio"));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\A(?:((https?|ftp)://(-\.)?([^\s/?.#]+\.?)+(/[^\s]*)?)?)\Z"))
                errores.Add(new Error(TipoError.Grave, posicion, "URL invalida. Tipos de URL permitidos: http://, https:// y ftp://"));

            return errores;
        }
    }
}

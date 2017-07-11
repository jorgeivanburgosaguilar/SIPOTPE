using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("notas", "nota")]
    public class Nota : Campo
    {
        public int LargoMaximo
        {
            get { return 4000; }
        }

        public Nota()
        {
            Tipo = TipoCampo.Nota;
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (valor.Length > LargoMaximo)
                errores.Add(new Error(TipoError.Advertencia, posicion,
                    string.Format("El texto excede el tamaño maximo de caracteres permitidos. Estas usando {0} de {1} caracteres permitidos.",
                        valor.Length, LargoMaximo)));

            if (!valor.Trim().Equals(valor))
                errores.Add(new Error(TipoError.Advertencia, posicion, "El texto tiene espacios en blanco al principio o al final."));

            if (!Regex.IsMatch(valor, @"\A[\w\d!""#$%&'()*+,\-.?¿¡@_:;Üü°Öö/\s]*\Z"))
                errores.Add(new Error(TipoError.Advertencia, posicion, "El texto tiene caracteres no permitidos."));

            return errores;
        }

        public override string ObtenerValorRegistroParaXML(Registro registro)
        {
            var valor = registro.Valor.Trim();

            if (valor.Length > LargoMaximo)
                valor = valor.Substring(0, LargoMaximo);

            return Genericos.EscaparCadenaParaXML(Regex.Replace(valor, @"[^\w\d!""#$%&'()*+,\-.?¿¡@_:;Üü°Öö/\s]", ""));
        }
    }
}

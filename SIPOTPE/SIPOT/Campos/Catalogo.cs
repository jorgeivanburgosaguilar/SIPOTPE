using System;
using System.Collections.Generic;
using System.Globalization;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("catalogos", "catalogo")]
    public class Catalogo : Campo
    {
        public SortedList<int, string> Elementos;

        public Catalogo()
        {
            Tipo = TipoCampo.Catalogo;
            Elementos = new SortedList<int, string>();
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

            // Se validan en minusculas los valores debido a que el catalogo es procesado en minusculas,
            // al ser los elementos del catalogo insensibles a mayusculas y minusculas
            if (!Elementos.ContainsValue(valor.Trim().ToLowerInvariant()))
                errores.Add(new Error(TipoError.Grave, posicion, "El valor seleccionado no forma parte de los elementos autorizados por el catalogo."));

            return errores;
        }

        public override List<Error> Validar()
        {
            var errores = new List<Error>();

            // Validacion para ver si tenemos elementos en el catalogo
            if (Elementos.Count <= 0)
            {
                errores.Add(new Error(TipoError.Critico, Posicion,
                    string.Format(
                        "No pudimos encontrar el catalogo de la columna \"{0}\", verifique que la estructura del formato no haya sido alterada.",
                        Nombre)));
                return errores;
            }

            // Validacion para detectar los valores duplicados en el Catalogo
            var lista = new SortedList<int, string>();
            foreach (var elemento in Elementos)
            {
                if (lista.ContainsValue(elemento.Value))
                    errores.Add(new Error(TipoError.Critico, new Posicion(Posicion.Hoja, Posicion.Columna, elemento.Key),
                        "Elemento del catalogo duplicado"));
                else
                    lista.Add(elemento.Key, elemento.Value);
            }

            // Ejecutar las validaciones de la base (ID, Nombre y Registros)
            errores.AddRange(base.Validar());

            return errores;
        }

        public override string ObtenerValorRegistroParaXML(Registro registro)
        {
            if (ValidarRegistro(registro).Count > 0)
                return ValorPorDefecto;

            var valor = registro.Valor.Trim().ToLowerInvariant();
            var indexValor = Elementos.IndexOfValue(valor);
            return indexValor < 0 ? ValorPorDefecto : Elementos.Keys[indexValor].ToString(CultureInfo.InvariantCulture);
        }

        public override Campo Clonar()
        {
            var tmpCampo = base.Clonar();
            var tmpCatalogo = (Catalogo) tmpCampo;
            tmpCatalogo.Elementos = Elementos;
            return tmpCatalogo;
        }
    }
}
using System;
using System.Collections.Generic;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Catalogo : Campo
    {
        public SortedList<int, string> Elementos;

        public Catalogo()
        {
            Elementos = new SortedList<int, string>();
        }

        // ToDo Como sacar la hoja que le corresponde al catalogo?
        public override List<Error> Validar()
        {
            var errores = base.Validar();

            // Validacion para ver si tenemos elementos en el catalogo
            if (Elementos.Count <= 0)
            {
                errores.Add(new Error(TipoError.Critico, new Posicion(), string.Format("No pudimos procesar el catalogo de la Columna {0}", Nombre)));
                return errores;
            }

            // Validacion para detectar los valores duplicados en el Catalogo
            var lista = new SortedList<int, string>();
            foreach (var elemento in Elementos)
            {
                if (lista.ContainsValue(elemento.Value))
                    errores.Add(new Error(TipoError.Critico, new Posicion(string.Empty, 0, elemento.Key), "Elemento del catalogo duplicado"));
                else
                    lista.Add(elemento.Key, elemento.Value);
            }

            return errores;
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

            if (!Elementos.ContainsValue(valor))
                errores.Add(new Error(TipoError.Grave, posicion, "El valor seleccionado no forma parte de los elementos autorizados por el catalogo."));

            return errores;
        }
    }
}
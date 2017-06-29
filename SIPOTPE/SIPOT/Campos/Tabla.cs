using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("tablas", "tabla")]
    public class Tabla : Campo
    {
        public List<Campo> Campos { get; set; }

        public Tabla()
        {
            Tipo = TipoCampo.Tabla;
            Campos = new List<Campo>();
        }

        public override List<Error> ValidarRegistro(Registro registro)
        {
            var valor = registro.Valor ?? string.Empty;
            var posicion = registro.Posicion;
            var errores = new List<Error>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                errores.Add(new Error(TipoError.Advertencia, posicion, "El Identificador de la Tabla no debe estar vacio."));
                return errores;
            }

            if (!Regex.IsMatch(valor, @"\d+"))
                errores.Add(new Error(TipoError.Grave, posicion, "El Identificador de la Tabla debe ser un valor numerico."));

            return errores;
        }

        public override List<Error> ValidarRegistros()
        {
            var errores = new List<Error>();

            // Asumo que se ha previamente validado la cantidad de Identificadores de Tabla a 1 en Validar()
            // en el peor de los casos se devuelve un campo vacio de tipo IdentificadorTabla
            var campoIdentificadorTabla = Campos.Find(p => p is IdentificadorTabla) ?? new IdentificadorTabla();
            var identificadoresTabla = campoIdentificadorTabla.Registros.Select(registro => registro.Valor).ToList();

            // Validamos si el identificador del registro existe en la tabla,
            // siempre y cuando el registro sea valido.
            foreach (var registro in Registros)
            {
                var erroresRegistro = ValidarRegistro(registro);
                if (erroresRegistro.Count > 0)
                {
                    errores.AddRange(erroresRegistro);
                }
                else
                {
                    if (!identificadoresTabla.Contains(registro.Valor))
                        errores.Add(new Error(TipoError.Grave, registro.Posicion, "El Identificador de la Tabla no existe"));
                }
            }

            return errores;
        }

        public override List<Error> Validar()
        {
            var errores = new List<Error>();

            // Validacion para ver si tenemos campos en la tabla
            if (Campos.Count <= 0)
            {
                errores.Add(new Error(TipoError.Critico, Posicion,
                    string.Format(
                        "No pudimos encontrar la tabla de la columna \"{0}\", verifique que la estructura del formato no haya sido alterada.",
                        Nombre)));
                return errores;
            }

            // Validacion para comprobar que la tabla solo tenga 1 campo tipo IdentificadorTabla (Columna ID en Excel)
            if (Campos.Count(p => p is IdentificadorTabla) != 1)
            {
                errores.Add(new Error(TipoError.Critico, Posicion,
                    string.Format(
                        "No pudimos procesar la tabla de la columna \"{0}\", verifique que la estructura del formato no haya sido alterada.",
                        Nombre)));
                return errores;
            }

            // Validar campos de la tabla
            foreach (var campo in Campos)
                errores.AddRange(campo.Validar());

            // Ejecutar las validaciones de la base (ID, Nombre y Registros)
            errores.AddRange(base.Validar());

            return errores;
        }

        public override string HaciaXML()
        {
            var configuracionesXML = (ConfiguracionesXML)GetType().GetCustomAttribute(typeof(ConfiguracionesXML), false);
            if (!configuracionesXML.Procesar)
                return string.Empty;

            // Aislamos los campos de la tabla por cada fila
            var tablaPorFila = new Dictionary<int, List<Campo>>();
            foreach (var campo in Campos)
            {
                foreach (var registro in campo.Registros)
                {
                    if (!tablaPorFila.ContainsKey(registro.Numero))
                        tablaPorFila.Add(registro.Numero, new List<Campo>());

                    var tmpCampo = FabricarPorTipo(campo.Tipo);
                    tmpCampo.ID = campo.ID;
                    tmpCampo.Nombre = campo.Nombre;
                    tmpCampo.Posicion = campo.Posicion;
                    tmpCampo.ValorPorDefecto = campo.ValorPorDefecto;
                    tmpCampo.Registros = new List<Registro> { registro };

                    if (campo.Tipo == TipoCampo.Catalogo)
                    {
                        var campoCatalogo = (Catalogo) campo;
                        var tmpCampoCatalogo = (Catalogo) tmpCampo;
                        tmpCampoCatalogo.Elementos = campoCatalogo.Elementos;
                    }

                    tablaPorFila[registro.Numero].Add(tmpCampo);
                }
            }

            // Convertimos el IdentificadorTabla a la llave en un diccionario para mejorar
            // el rendimiento de las busquedas en la tabla
            // Nota: Este codigo asume que estamos procesando una tabla 100% valida
            var tabla = new Dictionary<int, List<Campo>>();
            foreach (var fila in tablaPorFila)
            {
                var campoIdentificadorTabla = fila.Value.Find(p => p is IdentificadorTabla) ?? new IdentificadorTabla();
                var idTabla = Genericos.ConvertirCadenaAEntero(campoIdentificadorTabla.Registros[0].Valor);

                if (!tabla.ContainsKey(idTabla))
                    tabla.Add(idTabla, new List<Campo>());

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var campo in fila.Value)
                {
                    if (campo is IdentificadorTabla)
                        continue;

                    tabla[idTabla].Add(campo);
                }
            }

            var strCamposTablaXML = new StringBuilder();
            strCamposTablaXML.Append("--- TABLA ---\n");
            foreach (var registro in Registros)
            {
                var idTabla = Genericos.ConvertirCadenaAEntero(registro.Valor);
                if (!tabla.ContainsKey(idTabla))
                    continue;

                var camposRegistro = tabla[idTabla];
                foreach (var campo in camposRegistro)
                {
                    strCamposTablaXML.Append(campo.HaciaXML());
                    strCamposTablaXML.Append("\n");
                }
            }
            strCamposTablaXML.Append("--- TABLA ---\n");

            return strCamposTablaXML.ToString();
        }
    }
}

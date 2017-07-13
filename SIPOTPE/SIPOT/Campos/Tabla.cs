using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DotLiquid;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("tablas", "tabla")]
    public class Tabla : Campo
    {
        public List<Campo> Campos { get; set; }

        public int ValorPorDefectoEntero
        {
            get { return Genericos.ConvertirCadenaAEntero(ValorPorDefecto); }
        }

        public Tabla()
        {
            Tipo = TipoCampo.Tabla;
            Campos = new List<Campo>();
            ValorPorDefecto = "-9999";
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
                        "No pudimos encontrar el identificador de la tabla para la tabla de la columna \"{0}\", verifique que la estructura del formato no haya sido alterada.",
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

        /// <summary>
        /// Convierte la tabla de un conjunto de campos a un diccionario de elementos
        /// campo clasificados por ID en una relacion 1 a muchos registros.
        /// </summary>
        /// <remarks>Este codigo asume que estamos procesando una tabla 100% valida</remarks>
        private Dictionary<int, List<Campo>> ProcesarTabla()
        {
            // Aislamos cada campo de la tabla (con su registro) por fila
            var tablaPorFila = new Dictionary<int, List<Campo>>();
            foreach (var campo in Campos)
            {
                foreach (var registro in campo.Registros)
                {
                    if (!tablaPorFila.ContainsKey(registro.Numero))
                        tablaPorFila.Add(registro.Numero, new List<Campo>());

                    var tmpCampo = campo.Clonar();
                    tmpCampo.Registros = new List<Registro> { registro };
                    tablaPorFila[registro.Numero].Add(tmpCampo);
                }
            }

            // Convertimos la tabla por filas en un diccionario de RegistrosTabla
            // donde el ID de la tabla es la llave
            var tablaPorID = new Dictionary<int, List<RegistroTabla>>();
            for (var i = 0; i < tablaPorFila.Count; i++)
            {
                var fila = tablaPorFila[i];
                var campoIdentificadorTabla = fila.Find(p => p.Tipo.Equals(TipoCampo.IdentificadorTabla)) ?? new IdentificadorTabla();
                var idTabla = Genericos.ConvertirCadenaAEntero(campoIdentificadorTabla.Registros[0].Valor);

                if (!tablaPorID.ContainsKey(idTabla))
                    tablaPorID.Add(idTabla, new List<RegistroTabla>());

                var registroTabla = new RegistroTabla(i);
                foreach (var campo in fila.Where(campo => campo.Tipo != TipoCampo.IdentificadorTabla))
                    registroTabla.Campos.Add(campo);

                tablaPorID[idTabla].Add(registroTabla);
            }

            // Unificamos los registros de los campos con el mismo ID de campo
            // por cada ID de la tabla
            var tabla = new Dictionary<int, List<Campo>>();
            foreach (var registroTablaPorID in tablaPorID)
            {
                var camposTabla = new List<Campo>();
                foreach (var registroTabla in registroTablaPorID.Value)
                {
                    foreach (var campoRegistroTabla in registroTabla.Campos)
                    {
                        var tmpCampo = camposTabla.Find(c => c.ID.Equals(campoRegistroTabla.ID));
                        if (tmpCampo == null)
                        {
                            camposTabla.Add(campoRegistroTabla);
                            continue;
                        }

                        tmpCampo.Registros.AddRange(campoRegistroTabla.Registros);
                    }
                }

                tabla.Add(registroTablaPorID.Key, camposTabla);
            }

            // Generamos una "fila" de campos tabla vacio
            var camposTablaVacio = new List<Campo>();
            foreach (var campo in Campos)
            {
                if (campo.Tipo == TipoCampo.IdentificadorTabla)
                    continue;

                var tmpCampo = campo.Clonar();
                var tmpCampoRegistro = new Registro
                {
                    Posicion = tmpCampo.Posicion
                };

                tmpCampo.Registros.Add(tmpCampoRegistro);
                camposTablaVacio.Add(tmpCampo);
            }

            // Insertamos la fila de campos vacios de la tabla en el registro con el ID "-9999"
            // para hacer referencia a los registros vacios, los cuales retornan este ValorPorDefecto
            // al ser considerados invalidos.
            tabla.Add(ValorPorDefectoEntero, camposTablaVacio);

            return tabla;
        }

        public override StringBuilder HaciaXML()
        {
            var ordenarCamposPorTipo = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("OrdenarCamposPorTipo"));
            var configuracionesXML = (ConfiguracionesXML) GetType().GetCustomAttribute(typeof (ConfiguracionesXML), false);
            var plantillaCampoTabla = Template.Parse(File.ReadAllText("SIPOT/Plantillas/CampoTabla.xml"));
            var plantillaRegistroTabla = Template.Parse(File.ReadAllText("SIPOT/Plantillas/RegistroCampoTabla.xml"));
            var tabla = ProcesarTabla();
            var strRegistrosTabla = new StringBuilder();

            var tiposCampo = new List<TipoCampo>();
            if (ordenarCamposPorTipo)
            {
                tiposCampo.AddRange(Enum.GetValues(typeof(TipoCampo)).Cast<TipoCampo>().ToList());
            }
            else
            {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var campo in Campos)
                {
                    var tipoCampoActual = campo.Tipo;
                    if (tiposCampo.Contains(tipoCampoActual))
                        continue;

                    tiposCampo.Add(tipoCampoActual);
                }
            }
            
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var registro in Registros)
            {
                var valor = ObtenerValorRegistroParaXML(registro);
                var idTabla = Genericos.ConvertirCadenaAEntero(valor, ValorPorDefectoEntero);
                if (!tabla.ContainsKey(idTabla))
                {
                    Debug.WriteLine("ID de la tabla \"{0}\" no existe al convertir a XML: {1}", Nombre, idTabla);
                    continue;
                }

                var camposTabla = tabla[idTabla];
                var strCamposTabla = new StringBuilder();
                foreach (var tipoCampo in tiposCampo)
                {
                    var tipoCampoActual = tipoCampo;
                    var configXMLTipoCampo =
                        FabricarPorTipo(tipoCampoActual).GetType().GetCustomAttribute(typeof (ConfiguracionesXML), false) as ConfiguracionesXML;

                    if (configXMLTipoCampo == null)
                    {
                        Debug.WriteLine("Error al obtener configuraciones xml del tipo de campo \"{0}\"", tipoCampoActual.Descripcion());
                        continue;
                    }

                    if (!configXMLTipoCampo.Procesar)
                        continue;

                    if (camposTabla.Count(c => c.Tipo.Equals(tipoCampoActual)) <= 0)
                        continue;

                    var strCamposRegistroPorTipo = new StringBuilder();
                    foreach (var campo in camposTabla.Where(c => c.Tipo.Equals(tipoCampoActual)))
                    {
                        strCamposRegistroPorTipo.Append(campo.HaciaXML());
                        strCamposRegistroPorTipo.Append("\n");
                    }

                    // Eliminar ultimo "\n"
                    Genericos.EliminarUltimoCaracter(strCamposRegistroPorTipo);

                    // Aplicar template de tipo campo tabla
                    strCamposTabla.Append(plantillaCampoTabla.Render(Hash.FromAnonymousObject(
                        new
                        {
                            nombre = configXMLTipoCampo.NombreCampoTabla,
                            registros = strCamposRegistroPorTipo.ToString()
                        }
                        )));

                    strCamposTabla.Append("\n");
                }

                // Eliminar ultimo "\n"
                Genericos.EliminarUltimoCaracter(strCamposTabla);

                strRegistrosTabla.Append(plantillaRegistroTabla.Render(Hash.FromAnonymousObject(
                    new
                    {
                        nombre = configuracionesXML.NombreRegistro,
                        id = ID,
                        numero = registro.Numero,
                        registros = strCamposTabla.ToString()
                    }
                    )));

                strRegistrosTabla.Append("\n");
            }

            // Eliminar ultimo "\n"
            Genericos.EliminarUltimoCaracter(strRegistrosTabla);

            return strRegistrosTabla;
        }

        public override Campo Clonar()
        {
            var tmpCampo = base.Clonar();
            var tmpTabla = (Tabla) tmpCampo;
            tmpTabla.Campos = Campos;
            return tmpTabla;
        }
    }
}

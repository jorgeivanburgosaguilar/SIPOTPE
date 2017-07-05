using System;
using System.Collections.Generic;
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

        private Dictionary<int, List<RegistroTabla>> ProcesarTabla()
        {
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
                    tmpCampo.EstaDentroDeUnaTabla = campo.EstaDentroDeUnaTabla;

                    if (campo.Tipo == TipoCampo.Catalogo)
                    {
                        var campoCatalogo = (Catalogo)campo;
                        var tmpCampoCatalogo = (Catalogo)tmpCampo;
                        tmpCampoCatalogo.Elementos = campoCatalogo.Elementos;
                    }

                    tablaPorFila[registro.Numero].Add(tmpCampo);
                }
            }

            // Convertimos el IdentificadorTabla en la llave de un diccionario para mejorar
            // el rendimiento de las busquedas en la tabla
            // Nota: Este codigo asume que estamos procesando una tabla 100% valida
            var tabla = new Dictionary<int, List<RegistroTabla>>();
            for (var i = 0; i < tablaPorFila.Count; i++)
            {
                var fila = tablaPorFila[i];
                var campoIdentificadorTabla = fila.Find(p => p is IdentificadorTabla) ?? new IdentificadorTabla();
                var idTabla = Genericos.ConvertirCadenaAEntero(campoIdentificadorTabla.Registros[0].Valor);

                if (!tabla.ContainsKey(idTabla))
                    tabla.Add(idTabla, new List<RegistroTabla>());

                var registroTabla = new RegistroTabla(i);
                foreach (var campo in fila.Where(campo => campo.Tipo != TipoCampo.IdentificadorTabla))
                    registroTabla.Campos.Add(campo);

                tabla[idTabla].Add(registroTabla);
            }

            return tabla;
        }

        public override string HaciaXML()
        {
            var configuracionesXML = (ConfiguracionesXML) GetType().GetCustomAttribute(typeof (ConfiguracionesXML), false);
            var plantillaCampoTabla = Template.Parse(File.ReadAllText("SIPOT/Plantillas/CampoTabla.xml"));
            var plantillaRegistroTabla = Template.Parse(File.ReadAllText("SIPOT/Plantillas/RegistroCampoTabla.xml"));
            var tiposCampo = Enum.GetValues(typeof(TipoCampo)).Cast<TipoCampo>().ToList();
            var tabla = ProcesarTabla();
            var strRegistrosTabla = new StringBuilder();
            
            foreach (var registro in Registros)
            {
                var idTabla = Genericos.ConvertirCadenaAEntero(registro.Valor);
                if (!tabla.ContainsKey(idTabla))
                    continue;

                var registrosTabla = tabla[idTabla];
                foreach (var registroTabla in registrosTabla)
                {
                    var strCamposRegistroTabla = new StringBuilder();
                    foreach (var tipoCampoRegistro in tiposCampo)
                    {
                        var tipoCampoRegistroActual = tipoCampoRegistro;

                        var configXMLTipoActualRegistro =
                            FabricarPorTipo(tipoCampoRegistroActual).GetType().GetCustomAttribute(typeof (ConfiguracionesXML), false) as ConfiguracionesXML;

                        if (configXMLTipoActualRegistro == null)
                        {
                            Debug.WriteLine("Error al obtener configuraciones xml de {0}", tipoCampoRegistroActual.Descripcion());
                            continue;
                        }

                        if (!configXMLTipoActualRegistro.Procesar)
                            continue;

                        if (registroTabla.Campos.Count(campo => campo.Tipo.Equals(tipoCampoRegistroActual)) <= 0)
                            continue;

                        var strCamposRegistroPorTipo = new StringBuilder();
                        foreach (var campo in registroTabla.Campos.Where(campo => campo.Tipo == tipoCampoRegistroActual))
                        {
                            strCamposRegistroPorTipo.Append(campo.HaciaXML());
                            strCamposRegistroPorTipo.Append("\n");
                        }

                        // Eliminar ultimo "\n"
                        Genericos.EliminarUltimoCaracter(strCamposRegistroPorTipo);

                        // Aplicar template de tipo campo tabla
                        strCamposRegistroTabla.Append(plantillaCampoTabla.Render(Hash.FromAnonymousObject(
                        new
                        {
                            nombre = string.Format("{0}Tabla", configXMLTipoActualRegistro.NombreCampo),
                            registros = strCamposRegistroPorTipo.ToString()
                        }
                        )));

                        strCamposRegistroTabla.Append("\n");
                    }

                    // Eliminar ultimo "\n"
                    Genericos.EliminarUltimoCaracter(strCamposRegistroTabla);

                    strRegistrosTabla.Append(plantillaRegistroTabla.Render(Hash.FromAnonymousObject(
                        new
                        {
                            nombre = configuracionesXML.NombreRegistro,
                            id = ID,
                            numero = registroTabla.Numero,
                            registros = strCamposRegistroTabla.ToString()
                        }
                        )));

                    strRegistrosTabla.Append("\n");
                }
            }

            // Eliminar ultimo "\n"
            Genericos.EliminarUltimoCaracter(strRegistrosTabla);

            return strRegistrosTabla.ToString();
        }
    }
}

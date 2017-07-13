using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DotLiquid;
using SIPOTPE.SIPOT.Campos;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT
{
    [Serializable]
    public class Formato
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public List<Campo> Campos { get; set; }

        public Formato()
        {
            ID = 0;
            Nombre = string.Empty;
            Campos = new List<Campo>();
        }

        public Formato(int id, string nombre)
        {
            ID = id;
            Nombre = nombre;
            Campos = new List<Campo>();
        }

        public int CantidadCampos
        {
            get { return Campos == null ? 0 : Campos.Count; }
        }

        public List<Error> Validar()
        {
            var errores = new List<Error>();

            if (ID < 0 || ID > 99999999)
                errores.Add(new Error(TipoError.Critico, new Posicion("Reporte de Formatos", 0, 2),
                    "El identificador del formato es invalido, verifique que la estructura del formato no haya sido alterada."));

            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 4000)
                errores.Add(new Error(TipoError.Critico, new Posicion("Reporte de Formatos", 0, 2),
                    "El nombre del formato es invalido, verifique que la estructura del formato no haya sido alterada."));

            // Validar Campos del Formato
            errores.AddRange(ValidarCampos());

            return errores;
        }

        public List<Error> ValidarCampos()
        {
            var errores = new List<Error>();

            foreach (var campo in Campos)
                errores.AddRange(campo.Validar());

            return errores;
        }

        public string HaciaXML()
        {
            var ordenarCamposPorTipo = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("OrdenarCamposPorTipo"));
            var plantillaFormato = Template.Parse(File.ReadAllText("SIPOT/Plantillas/Formato.xml"));
            var plantillaCampo = Template.Parse(File.ReadAllText("SIPOT/Plantillas/Campo.xml"));

            var tiposCampo = new List<TipoCampo>();
            if (ordenarCamposPorTipo)
            {
                tiposCampo.AddRange(Enum.GetValues(typeof (TipoCampo)).Cast<TipoCampo>().ToList());
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

            var strCamposFormato = new StringBuilder();
            foreach (var tipoCampo in tiposCampo)
            {
                var tipoCampoActual = tipoCampo;

                var configuracionesXML =
                    Campo.FabricarPorTipo(tipoCampoActual).GetType().GetCustomAttribute(typeof (ConfiguracionesXML), false) as ConfiguracionesXML;

                if (configuracionesXML == null)
                {
                    Debug.WriteLine("Error al obtener las configuraciones xml de {0}", tipoCampoActual.Descripcion());
                    continue;
                }

                if (!configuracionesXML.Procesar)
                    continue;

                if (Campos.Count(campo => campo.Tipo.Equals(tipoCampoActual)) <= 0)
                    continue;

                var strCamposPorTipo = new StringBuilder();

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var campo in Campos.Where(campo => campo.Tipo.Equals(tipoCampoActual)))
                {
                    var salidaXML = campo.HaciaXML();
                    if (salidaXML.Length <= 0)
                        continue;

                    strCamposPorTipo.Append(salidaXML);
                    strCamposPorTipo.Append("\n");
                }

                // Eliminar ultimo "\n"
                Genericos.EliminarUltimoCaracter(strCamposPorTipo);

                strCamposFormato.Append(plantillaCampo.Render(Hash.FromAnonymousObject(
                    new
                    {
                        nombre = configuracionesXML.NombreCampo,
                        registros = strCamposPorTipo.ToString()
                    }
                    )));

                strCamposFormato.Append("\n");
            }

            // Eliminar ultimo "\n"
            Genericos.EliminarUltimoCaracter(strCamposFormato);

            var formato = new StringBuilder();
            formato.Append(plantillaFormato.Render(Hash.FromAnonymousObject(
                new
                {
                    id = ID,
                    nombre = Nombre,
                    campos = strCamposFormato.ToString()
                }
                )));

            return formato.ToString();
        }
    }
}

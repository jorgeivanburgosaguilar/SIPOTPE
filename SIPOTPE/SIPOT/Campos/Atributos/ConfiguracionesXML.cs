using System;

namespace SIPOTPE.SIPOT.Campos.Atributos
{
    /// <summary>
    /// Atributo que permite configurar las opciones a cada tipo de campo al convertir XML
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfiguracionesXML : Attribute
    {
        public string NombreCampo { get; private set; }
        public string NombreRegistro { get; private set; }
        public bool Procesar { get; set; }

        public string NombreCampoTabla
        {
            get { return string.Format("{0}Tabla", NombreCampo); }
        }

        public string NombreRegistroTabla
        {
            get { return string.Format("{0}Tabla", NombreRegistro); }
        }

        public ConfiguracionesXML()
        {
            NombreCampo =
                NombreRegistro = string.Empty;
            Procesar = true;
        }

        public ConfiguracionesXML(string nombreCampo, string nombreRegistro, bool procesar = true)
        {
            NombreCampo = nombreCampo;
            NombreRegistro = nombreRegistro;
            Procesar = procesar;
        }
    }
}
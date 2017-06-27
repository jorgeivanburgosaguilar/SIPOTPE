using System;

namespace ConsoleApplication2.SIPOTWS.Campos.Decoradores
{
    /// <summary>
    /// Atributo que configura los nombres usados al serializar un campo con sus registros a XML
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NombresXML : Attribute
    {
        public string Campo { get; private set; }
        public string Registro { get; private set; }

        public NombresXML(string campo, string registro)
        {
            Campo = campo;
            Registro = registro;
        }
    }
}
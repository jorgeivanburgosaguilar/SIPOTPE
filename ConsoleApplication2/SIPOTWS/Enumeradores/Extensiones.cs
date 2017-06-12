using System;
using System.ComponentModel;

namespace ConsoleApplication2.SIPOTWS.Enumeradores
{
    public static class Extensiones
    {
        public static string Descripcion(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof (DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

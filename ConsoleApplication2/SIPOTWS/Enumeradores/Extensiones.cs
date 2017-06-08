using System;
using System.ComponentModel;

namespace ConsoleApplication2.SIPOTWS.Enumeradores
{
    public static class Extensiones
    {
        public static bool TieneAtributo(this Enum value, Type type)
        {
            try
            {
                return value.GetType().GetField(value.ToString()).IsDefined(type, false);
            }
            catch
            {
                return false;
            }
        }

        public static string Descripcion(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof (DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

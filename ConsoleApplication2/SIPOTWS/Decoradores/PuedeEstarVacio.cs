using System;

namespace ConsoleApplication2.SIPOTWS.Decoradores
{
    /// <summary>
    /// Atributo que marca si un tipo de campo puede estar vacio.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PuedeEstarVacio : Attribute
    {
    }
}

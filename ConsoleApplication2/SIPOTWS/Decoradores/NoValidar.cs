using System;

namespace ConsoleApplication2.SIPOTWS.Decoradores
{
    /// <summary>
    /// Atributo que marca si un tipo de campo no debe ser validado.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NoValidar : Attribute
    {
    }
}
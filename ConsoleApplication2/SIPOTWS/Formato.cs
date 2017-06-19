using System;
using System.Collections.Generic;
using ConsoleApplication2.SIPOTWS.Campos;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS
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

        public List<Error> Validar()
        {
            var errores = new List<Error>();

            if (ID < 0 || ID > 99999999)
                errores.Add(new Error(TipoError.Critico, new Posicion(),
                    "El identificador del formato es invalido, verifique que la estructura del formato no haya sido alterada."));

            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 4000)
                errores.Add(new Error(TipoError.Critico, new Posicion(),
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
    }
}

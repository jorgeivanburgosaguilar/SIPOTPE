using System;
using System.Collections.Generic;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    public class Campo
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public List<Registro> Registros { get; set; }

        public Campo()
        {
            ID = 0;
            Nombre = string.Empty;
            Registros = new List<Registro>();
        }

        public virtual List<Error> Validar()
        {
            var errores = new List<Error>();

            if (ID < 0 || ID > 99999999)
                errores.Add(new Error(TipoError.Critico, new Posicion(),
                    "El identificador del campo es invalido, verifique que la estructura del formato no haya sido alterada."));

            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 4000)
                errores.Add(new Error(TipoError.Critico, new Posicion(),
                    "El nombre del campo es invalido, verifique que la estructura del formato no haya sido alterada."));

            return errores;
        }

        public virtual List<Error> ValidarRegistro(Registro registro)
        {
            return new List<Error>
            {
                new Error(TipoError.Critico, registro.Posicion,
                    "Tipo de campo desconocido, verifique que la estructura del formato no haya sido alterada.")
            };
        }

        public virtual List<Error> ValidarRegistros()
        {
            var errores = new List<Error>();

            foreach (var registro in Registros)
                errores.AddRange(ValidarRegistro(registro));

            return errores;
        }
    }
}

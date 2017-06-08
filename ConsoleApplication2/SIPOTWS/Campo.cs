using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Decoradores;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS
{
    public class Campo
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public TipoCampo Tipo { get; set; }
        public List<Registro> Registros { get; set; }

        public Campo()
        {
            ID = 0;
            Nombre = string.Empty;
            Tipo = TipoCampo.Nulo;
            Registros = new List<Registro>();
        }

        public Campo(int id, string nombre, int idTipo)
        {
            ID = id;
            Nombre = nombre;
            Tipo = Enum.IsDefined(typeof (TipoCampo), idTipo) ? (TipoCampo) idTipo : TipoCampo.Nulo;
            Registros = new List<Registro>();
        }

        public Campo(int id, string nombre, TipoCampo tipo)
        {
            ID = id;
            Nombre = nombre;
            Tipo = tipo;
            Registros = new List<Registro>();
        }

        public static List<Error> Validar(Registro registro, TipoCampo tipo)
        {
            // ToDo Separar validaciones
            return new List<Error>();
        }

        public List<Error> ValidarRegistros()
        {
            var errores = new List<Error>();

            if (Tipo.TieneAtributo(typeof (NoValidar)))
                return errores;

            foreach (var registro in Registros)
            {
                var valor = registro.Valor;
                var posicion = registro.Posicion;

                if (valor == null)
                {
                    errores.Add(new Error(TipoError.Critico, posicion, "Registro nulo"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(valor) && !Tipo.TieneAtributo(typeof (PuedeEstarVacio)))
                {
                    errores.Add(new Error(TipoError.Grave, posicion, "El registro esta vacio"));
                    continue;
                }

                switch (Tipo)
                {
                    case TipoCampo.TextoCorto:
                    case TipoCampo.TextoLargo:
                    case TipoCampo.Nota:
                        var largoMaximo = Tipo.Equals(TipoCampo.TextoCorto) ? 150 : 4000;

                        if (valor.Length > largoMaximo)
                            errores.Add(new Error(TipoError.Advertencia, posicion,
                                string.Format(
                                    "El texto excede el tamaño maximo de caracteres permitidos. Estas usando {0} de {1} caracteres permitidos.",
                                    valor.Length, largoMaximo)));

                        if (!valor.Trim().Equals(valor))
                            errores.Add(new Error(TipoError.Advertencia, posicion, "El texto tiene espacios en blanco al principio o al final."));

                        if (!Regex.IsMatch(valor, @"\A[\w\d!""#$%&'()*+,\-.?¿¡@_:;Üü°Öö/\s]*\Z"))
                            errores.Add(new Error(TipoError.Advertencia, posicion, "El texto tiene caracteres no permitidos."));
                        break;

                    case TipoCampo.Numero:
                        if (!Regex.IsMatch(valor, @"[\-+]?[0-9]{1,12}([.][0-9]{1,2})?"))
                            errores.Add(new Error(TipoError.Grave, posicion,
                                "El valor numerico tiene un formato incorrecto. El valor numerico debe tener un numero maximo de 12 digitos con 2 digitos decimales opcionales, Ejemplo(s): 123456789012, 123456789012.1 y 123456789012.10"));
                        break;

                    case TipoCampo.Fecha:
                    case TipoCampo.FechaActualizacion:
                        if (!Regex.IsMatch(valor, @"\A(?:(0[1-9]|[12][0-9]|3[01])[/](0[1-9]|1[012])[/](\d{4}))\Z"))
                            errores.Add(new Error(TipoError.Grave, posicion,
                                "La fecha tiene un formato incorrecto. Las fechas deben tener el formato Dia/Mes/Año, Ejemplo: 01/09/2017"));
                        break;

                    case TipoCampo.Hora:
                        if (!Regex.IsMatch(valor, @"\A(?:([0-1][0-9]|2[1-3]):[0-5][0-9])\Z"))
                            errores.Add(new Error(TipoError.Grave, posicion,
                                "La hora tiene un formato incorrecto. Formato: Horas:Minutos, Ejemplo: 23:59"));
                        break;

                    case TipoCampo.Moneda:
                        if (!Regex.IsMatch(valor, @"\A[\-+]?[0-9]{1,12}[.][0-9]{2}\Z"))
                            errores.Add(new Error(TipoError.Grave, posicion,
                                "El valor monetario tiene un formato incorrecto. El valor monetario debe tener un numero de maximo de 12 digitos con 2 digitos decimales, Ejemplo: 123456789012.01"));
                        break;

                    case TipoCampo.PaginaWeb:
                        if (!Regex.IsMatch(valor, @"\A(?:((https?|ftp)://(-\.)?([^\s/?.#-]+\.?)+(/[^\s]*)?)?)\Z"))
                            errores.Add(new Error(TipoError.Grave, posicion, "URL invalida. Tipos de URL permitidos: http://, https:// y ftp://"));
                        break;

                    case TipoCampo.Anio:
                        if (!Regex.IsMatch(valor, @"\A\d{4}\Z"))
                            errores.Add(new Error(TipoError.Grave, posicion, "El campo tiene un formato incorrecto. Formato: 2017"));
                        break;
                }
            }

            return errores;
        }
    }
}

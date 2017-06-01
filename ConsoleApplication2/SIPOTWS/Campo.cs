using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConsoleApplication2.SIPOTWS.Enumeradores;

namespace ConsoleApplication2.SIPOTWS
{
    public class Campo
    {
        public int ID { get; set; }
        public string Etiqueta { get; set; }
        public TipoCampo TipoCampo { get; set; }
        public List<Registro> Registros { get; set; }

        public Campo()
        {
            ID = 0;
            Etiqueta = string.Empty;
            TipoCampo = TipoCampo.Nulo;
            Registros = new List<Registro>();
        }

        public Campo(int id, string etiqueta, int idTipoCampo)
        {
            ID = id;
            Etiqueta = etiqueta;
            TipoCampo = Enum.IsDefined(typeof (TipoCampo), idTipoCampo) ? (TipoCampo) idTipoCampo : TipoCampo.Nulo;
            Registros = new List<Registro>();
        }

        public Campo(int id, string etiqueta, TipoCampo tipoCampo)
        {
            ID = id;
            Etiqueta = etiqueta;
            TipoCampo = tipoCampo;
            Registros = new List<Registro>();
        }

        public List<string> ValidarRegistros()
        {
            var errores = new List<string>();

            // Si es de tipo archivo, separador o nulo entonces no se validan los registros.
            if (TipoCampo == TipoCampo.Archivo || TipoCampo == TipoCampo.Separador || TipoCampo == TipoCampo.Nulo)
                return errores;

            foreach (var registro in Registros)
            {
                var valor = registro.Valor;
                var posicion = registro.Posicion;

                if (string.IsNullOrWhiteSpace(valor) && TipoCampo != TipoCampo.Nota)
                {
                    errores.Add(string.Format("{0}: El registro esta vacio.", posicion));
                    continue;
                }

                switch (TipoCampo)
                {
                    case TipoCampo.TextoCorto:
                        if (valor.Length > 150)
                            errores.Add(string.Format("{0}: El registro excede el numero de caracteres permitidos. Estas usando {1} de {2} caracteres permitidos.", posicion, valor.Length, 150));

                        if (!Regex.IsMatch(valor, @"\A([\w\d!""#$%&'()*+,-.?¿¡@_:;Üü°Öö/][\s]*)*\Z"))
                            errores.Add(string.Format("{0}: El campo contiene caracteres no permitidos.", posicion));
                        break;

                    case TipoCampo.TextoLargo:
                    case TipoCampo.Nota:
                        if (valor.Length > 4000)
                            errores.Add(string.Format("{0}: El registro excede el numero de caracteres permitidos. Estas usando {1} de {2} caracteres permitidos.", posicion, valor.Length, 4000));

                        if (!Regex.IsMatch(valor, @"\A([\w\d!""#$%&'()*+,-.?¿¡@_:;Üü°Öö/][\s]*)*\Z"))
                            errores.Add(string.Format("{0}: El campo contiene caracteres no permitidos.", posicion));
                        break;

                    case TipoCampo.Numero:
                        if (!Regex.IsMatch(valor, @"\A[\-+]?[0-9]{1,12}([.][0-9]{1,2})?\Z"))
                            errores.Add(string.Format("{0}: El campo tiene un formato incorrecto. Formato: Numero con maximo 12 digitos con 2 decimales opcionales.", posicion));
                        break;

                    case TipoCampo.Fecha:
                    case TipoCampo.FechaActualizacion:
                        if (!Regex.IsMatch(valor, @"\A(\d{4}-\d\d-\d\d)*\Z"))
                            errores.Add(string.Format("{0}: El campo tiene un formato incorrecto. Formato: YYYY-MM-DD.", posicion));
                        break;

                    case TipoCampo.Hora:
                        if (!Regex.IsMatch(valor, @"\A[0-2][0-9]{0,2}:[0-5][0-9]{0,2}\Z"))
                            errores.Add(string.Format("{0}: El campo tiene un formato incorrecto. Formato: HH:MM", posicion));
                        break;

                    case TipoCampo.Moneda:
                        if (!Regex.IsMatch(valor, @"\A[\-+]?[0-9]{1,12}[.][0-9]{2}\Z"))
                            errores.Add(string.Format("{0}: El campo tiene un formato incorrecto. Formato: Numero con maximo 12 digitos y 2 decimales.", posicion));
                        break;

                    case TipoCampo.PaginaWeb:
                        if (!Regex.IsMatch(valor, @"\A(?:((https?|ftp)://(-\.)?([^\s/?.#-]+\.?)+(/[^\s]*)?)?)\Z"))
                            errores.Add(string.Format("{0}: El campo tiene un formato incorrecto. Tipos de URL permitidas: http:// https:// ftp://", posicion));
                        break;

                    case TipoCampo.Catalogo:
                        // Revisar si el catalogo coincide con algun valor
                        break;

                    case TipoCampo.Tabla:
                        // Revisar si el valor de la tabla coincide con alguna tabla
                        break;

                    case TipoCampo.Anio:
                        if (!Regex.IsMatch(valor, @"\A\d{4}\Z"))
                            errores.Add(string.Format("{0}: El campo tiene un formato incorrecto. Formato: YYYY.", posicion));
                        break;
                }
            }

            return errores;
        }
    }
}

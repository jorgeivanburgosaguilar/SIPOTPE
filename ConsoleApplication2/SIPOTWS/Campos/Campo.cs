using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ConsoleApplication2.SIPOTWS.Campos.Decoradores;
using ConsoleApplication2.SIPOTWS.Enumeradores;
using DotLiquid;

namespace ConsoleApplication2.SIPOTWS.Campos
{
    [Serializable]
    [NombresXML("camposDesconocidos", "campoDesconocido")]
    public class Campo
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public Posicion Posicion { get; set; }
        public List<Registro> Registros { get; set; }
        public string ValorPorDefecto { get; set; }

        public Campo()
        {
            ID = 0;
            Nombre = string.Empty;
            Posicion = new Posicion();
            Registros = new List<Registro>();
            ValorPorDefecto = string.Empty;
        }

        public int CantidadRegistros
        {
            get { return Registros == null ? 0 : Registros.Count; }
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

        public virtual List<Error> Validar()
        {
            var errores = new List<Error>();

            if (ID < 0 || ID > 99999999)
                errores.Add(new Error(TipoError.Critico, Posicion,
                    "El identificador del campo es invalido, verifique que la estructura del formato no haya sido alterada."));

            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 4000)
                errores.Add(new Error(TipoError.Critico, Posicion,
                    "El nombre del campo es invalido, verifique que la estructura del formato no haya sido alterada."));

            // Validar Registros de los Campos
            errores.AddRange(ValidarRegistros());

            return errores;
        }

        public virtual string ObtenerValorRegistroParaXML(Registro registro)
        {
            return ValidarRegistro(registro).Count > 0 ? ValorPorDefecto : registro.Valor;
        }

        public virtual string HaciaXML()
        {
            var nombresXML = (NombresXML) GetType().GetCustomAttribute(typeof (NombresXML), false);
            var rutaPlantillaRegistro = this is Catalogo ? "SIPOTWS/Plantillas/RegistroCatalogo.xml" : "SIPOTWS/Plantillas/Registro.xml";
            var plantillaRegistro = Template.Parse(File.ReadAllText(rutaPlantillaRegistro));

            var registrosCampos = new StringBuilder();
            var maxCantidadRegistros = CantidadRegistros;
            for (var i = 0; i < maxCantidadRegistros; i++)
            {
                var registro = Registros[i];
                registrosCampos.Append(plantillaRegistro.Render(Hash.FromAnonymousObject(
                    new
                    {
                        nombre = nombresXML.Registro,
                        valor = ObtenerValorRegistroParaXML(registro),
                        id = ID,
                        numero = registro.Numero,
                        etiqueta = Nombre
                    }
                    )));

                if (i != (maxCantidadRegistros - 1))
                    registrosCampos.Append("\n");
            }

            var plantillaCampo = Template.Parse(File.ReadAllText("SIPOTWS/Plantillas/Campo.xml"));
            var campo = plantillaCampo.Render(Hash.FromAnonymousObject(
                new
                {
                    nombre = nombresXML.Campo,
                    registros = registrosCampos.ToString()
                }
                ));

            return campo;
        }

        #region Fabricas
        public static Campo FabricarPorTipo(int idTipoCampo)
        {
            Campo campo;

            switch (idTipoCampo)
            {
                case 1:
                    campo = new TextoCorto();
                    break;

                case 2:
                    campo = new TextoLargo();
                    break;

                case 3:
                    campo = new Numero();
                    break;

                case 4:
                    campo = new Fecha();
                    break;

                case 5:
                    campo = new Hora();
                    break;

                case 6:
                    campo = new Moneda();
                    break;

                case 7:
                    campo = new PaginaWeb();
                    break;

                case 8:
                    campo = new Archivo();
                    break;

                case 9:
                    campo = new Catalogo();
                    break;

                case 10:
                    campo = new Tabla();
                    break;

                case 11:
                    campo = new Separador();
                    break;

                case 12:
                    campo = new Anio();
                    break;

                case 13:
                    campo = new FechaActualizacion();
                    break;

                case 14:
                    campo = new Nota();
                    break;

                // Tipo campo invalido especial para el id de la tabla
                case -9999:
                    campo = new IdentificadorTabla();
                    break;

                //case 0:
                default:
                    campo = new Campo();
                    break;
            }

            return campo;
        }

        public static Campo FabricarPorTipo(string strIdTipoCampo, bool esTabla = false)
        {
            try
            {
                if (esTabla && string.IsNullOrWhiteSpace(strIdTipoCampo))
                    strIdTipoCampo = "-9999";

                return FabricarPorTipo(string.IsNullOrWhiteSpace(strIdTipoCampo) ? 0 : Convert.ToInt32(strIdTipoCampo));
            }
            catch
            {
                return FabricarPorTipo(0);
            }
        }
        #endregion
    }
}

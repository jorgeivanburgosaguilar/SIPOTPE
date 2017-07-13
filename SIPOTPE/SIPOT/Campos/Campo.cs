using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using DotLiquid;
using SIPOTPE.SIPOT.Campos.Atributos;
using SIPOTPE.SIPOT.Enumeradores;

namespace SIPOTPE.SIPOT.Campos
{
    [Serializable]
    [ConfiguracionesXML("camposDesconocidos", "campoDesconocido", false)]
    public class Campo : ICloneable
    {
        public int ID { get; set; }
        public TipoCampo Tipo { get; set; }
        public string Nombre { get; set; }
        public Posicion Posicion { get; set; }
        public List<Registro> Registros { get; set; }
        public string ValorPorDefecto { get; set; }
        public bool EstaDentroDeUnaTabla { get; set; }

        public Campo()
        {
            ID = 0;
            Tipo = TipoCampo.Desconocido;
            Nombre = string.Empty;
            Posicion = new Posicion();
            Registros = new List<Registro>();
            ValorPorDefecto = string.Empty;
            EstaDentroDeUnaTabla = false;
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

        public virtual StringBuilder HaciaXML()
        {
            var configuracionesXML = (ConfiguracionesXML) GetType().GetCustomAttribute(typeof (ConfiguracionesXML), false);
            if (!configuracionesXML.Procesar)
                return new StringBuilder();

            string nombreRegistro;
            string rutaPlantillaRegistro;

            if (EstaDentroDeUnaTabla)
            {
                nombreRegistro = configuracionesXML.NombreRegistroTabla;
                rutaPlantillaRegistro = Tipo == TipoCampo.Catalogo ? "SIPOT/Plantillas/RegistroCatalogoTabla.xml" : "SIPOT/Plantillas/RegistroTabla.xml";
            }
            else
            {
                nombreRegistro = configuracionesXML.NombreRegistro;
                rutaPlantillaRegistro = Tipo == TipoCampo.Catalogo ? "SIPOT/Plantillas/RegistroCatalogo.xml" : "SIPOT/Plantillas/Registro.xml";
            }

            var plantillaRegistro = Template.Parse(File.ReadAllText(rutaPlantillaRegistro));
            var strRegistrosCampos = new StringBuilder();
            foreach (var registro in Registros)
            {
                strRegistrosCampos.Append(plantillaRegistro.Render(Hash.FromAnonymousObject(
                    new
                    {
                        nombre = nombreRegistro,
                        valor = ObtenerValorRegistroParaXML(registro),
                        id = ID,
                        numero = registro.Numero,
                        etiqueta = Nombre
                    }
                    )));
                strRegistrosCampos.Append("\n");
            }

            // Eliminar ultimo "\n"
            Genericos.EliminarUltimoCaracter(strRegistrosCampos);

            return strRegistrosCampos;
        }

        /// <summary>
        /// Clona un objeto
        /// </summary>
        /// <remarks>No incluye los registros</remarks>
        public virtual Campo Clonar()
        {
            var tmpCampo = FabricarPorTipo(Tipo);
            tmpCampo.ID = ID;
            tmpCampo.Nombre = Nombre;
            tmpCampo.Posicion = new Posicion(Posicion.Hoja, Posicion.Columna, Posicion.Fila);
            tmpCampo.ValorPorDefecto = ValorPorDefecto;
            tmpCampo.EstaDentroDeUnaTabla = EstaDentroDeUnaTabla;
            return tmpCampo;
        }

        public object Clone()
        {
            return Clonar();
        }

        #region Fabricas
        public static Campo FabricarPorTipo(TipoCampo tipoCampo)
        {
            Campo campo;

            switch (tipoCampo)
            {
                case TipoCampo.TextoCorto:
                    campo = new TextoCorto();
                    break;

                case TipoCampo.TextoLargo:
                    campo = new TextoLargo();
                    break;

                case TipoCampo.Numero:
                    campo = new Numero();
                    break;

                case TipoCampo.Fecha:
                    campo = new Fecha();
                    break;

                case TipoCampo.Hora:
                    campo = new Hora();
                    break;

                case TipoCampo.Moneda:
                    campo = new Moneda();
                    break;

                case TipoCampo.PaginaWeb:
                    campo = new PaginaWeb();
                    break;

                case TipoCampo.Archivo:
                    campo = new Archivo();
                    break;

                case TipoCampo.Catalogo:
                    campo = new Catalogo();
                    break;

                case TipoCampo.Tabla:
                    campo = new Tabla();
                    break;

                case TipoCampo.Separador:
                    campo = new Separador();
                    break;

                case TipoCampo.Anio:
                    campo = new Anio();
                    break;

                case TipoCampo.FechaActualizacion:
                    campo = new FechaActualizacion();
                    break;

                case TipoCampo.Nota:
                    campo = new Nota();
                    break;

                // Tipo campo invalido especial para el ID de la tabla
                case TipoCampo.IdentificadorTabla:
                    campo = new IdentificadorTabla();
                    break;

                //case TipoCampo.Desconocido:
                default:
                    campo = new Campo();
                    break;
            }

            return campo;
        }

        public static Campo FabricarPorTipo(int idTipoCampo)
        {
            if (!Enum.IsDefined(typeof (TipoCampo), idTipoCampo))
                idTipoCampo = 0;

            return FabricarPorTipo((TipoCampo) idTipoCampo);
        }

        public static Campo FabricarPorTipo(string strIdTipoCampo, bool esTabla = false)
        {
            try
            {
                if (esTabla && string.IsNullOrWhiteSpace(strIdTipoCampo))
                    strIdTipoCampo = "-99";

                return FabricarPorTipo(Genericos.ConvertirCadenaAEntero(strIdTipoCampo));
            }
            catch
            {
                return FabricarPorTipo(TipoCampo.Desconocido);
            }
        }
        #endregion
    }
}

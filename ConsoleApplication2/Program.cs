using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using ConsoleApplication2.SIPOTWS;
using ConsoleApplication2.SIPOTWS.Campos;
using ConsoleApplication2.SIPOTWS.Enumeradores;
using DevExpress.Spreadsheet;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace ConsoleApplication2
{
    public class Program
    {
        private static int ConvertirCadenaAEntero(string cadena)
        {
            try
            {
                return string.IsNullOrWhiteSpace(cadena) ? 0 : Convert.ToInt32(cadena);
            }
            catch
            {
                return 0;
            }
        }

        private static string ObtenerValorCelda(CellValue valor, bool esHora = false)
        {
            if (valor == null)
                return string.Empty;

            try
            {
                string cadena;
                switch (valor.Type)
                {
                    case CellValueType.Text:
                        cadena = valor.TextValue;
                        break;

                    case CellValueType.Boolean:
                        cadena = valor.BooleanValue.ToString().ToLower();
                        break;

                    case CellValueType.Numeric:
                        cadena = valor.NumericValue.ToString(CultureInfo.InvariantCulture);
                        break;

                    case CellValueType.DateTime:
                        cadena = esHora ? valor.DateTimeValue.ToString("HH:mm") : valor.DateTimeValue.ToString("dd/MM/yyyy");
                        break;

                    //case CellValueType.None:
                    //case CellValueType.Error:
                    //case CellValueType.Unknown:
                    default:
                        cadena = string.Empty;
                        break;
                }

                return cadena;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }
        }

        private static void ProcesarCatalogo(Campo campo, Workbook libro, DataValidation validaciones)
        {
            if (!(campo is Catalogo) || libro == null || validaciones == null)
                return;

            var criterio = validaciones.Criteria;
            if (criterio == null)
                return;

            if (!criterio.IsFormula)
                return;

            if (string.IsNullOrWhiteSpace(criterio.Formula))
                return;

            var nombreHojaCatalogo = criterio.Formula.Remove(0, 1);
            if (string.IsNullOrWhiteSpace(nombreHojaCatalogo))
                return;

            if (!libro.Worksheets.Contains(nombreHojaCatalogo))
            {
                Debug.WriteLine("No existe la hoja \"{0}\" del catalogo \"{1}\"", nombreHojaCatalogo, campo.Nombre);
                return;
            }

            var hojaCatalogo = libro.Worksheets[nombreHojaCatalogo];
            var rangoCatalogo = hojaCatalogo.GetDataRange();
            var maximaCantidadFilas = rangoCatalogo.BottomRowIndex;
            var catalogo = (Catalogo) campo;

            for (var fila = 0; fila <= maximaCantidadFilas; fila++)
            {
                var celda = hojaCatalogo.Columns[0][fila];
                if (celda == null)
                    continue;

                // Los elementos del catalogo se insertan en minusculas
                // para resolver el problema de que los elementos del catalogo
                // no son sensibles a mayusculas y minusculas.
                catalogo.Elementos.Add(fila, ObtenerValorCelda(celda.Value).ToLowerInvariant());
            }
        }

        private static void ProcesarTabla(Campo campo, Workbook libro)
        {
            if (!(campo is Tabla) || libro == null)
                return;

            var tabla = (Tabla) campo;
            var nombreHojaTabla = string.Format("Tabla {0}", tabla.ID);
            if (!libro.Worksheets.Contains(nombreHojaTabla))
            {
                Debug.WriteLine("No existe la hoja \"{0}\" de la tabla \"{1}\"", nombreHojaTabla, campo.Nombre);
                return;
            }

            var hojaTabla = libro.Worksheets[nombreHojaTabla];
            var rangoTabla = hojaTabla.GetDataRange();
            var maximaCantidadFilas = rangoTabla.BottomRowIndex;
            var maximaCantidadColumnas = rangoTabla.RightColumnIndex;

            // Procesar Campos de la Tabla
            for (var columna = 0; columna <= maximaCantidadColumnas; columna++)
            {
                var strIdTipoCampo = ObtenerValorCelda(hojaTabla.Columns[columna][0].Value);
                var strIdCampo = ObtenerValorCelda(hojaTabla.Columns[columna][1].Value);
                var celdaNombreCampo = hojaTabla.Columns[columna][2];

                var campoTabla = Campo.FabricarPorTipo(strIdTipoCampo, true);
                campoTabla.ID = ConvertirCadenaAEntero(strIdCampo);
                campoTabla.Nombre = ObtenerValorCelda(celdaNombreCampo.Value);
                campoTabla.Posicion = new Posicion(nombreHojaTabla, celdaNombreCampo.LeftColumnIndex, celdaNombreCampo.TopRowIndex);

                // Procesar Registros de los Campos de la Tabla
                var esTipoCampoTablaHora = campoTabla is Hora;
                for (var fila = 3; fila <= maximaCantidadFilas; fila++)
                {
                    var celda = hojaTabla.Columns[columna][fila];

                    if (fila == 3)
                    {
                        if (campoTabla is Catalogo)
                            ProcesarCatalogo(campoTabla, libro, hojaTabla.DataValidations.GetDataValidation(celda));
                    }

                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var registro = new Registro();
                    registro.Numero = fila - 3;
                    registro.Posicion = new Posicion(nombreHojaTabla, celda.LeftColumnIndex, celda.TopRowIndex);
                    registro.Valor = ObtenerValorCelda(celda.Value, esTipoCampoTablaHora);
                    campoTabla.Registros.Add(registro);
                }

                tabla.Campos.Add(campoTabla);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Inicio: {0:G}\n", DateTime.Now);

            var argumento1 = string.Empty;
            if (args.Length > 0)
                argumento1 = args[0];

            var nombreLibro = string.IsNullOrWhiteSpace(argumento1) ? "Formato_Pruebas.xls" : argumento1;
            var libro = new Workbook();
            libro.LoadDocument(nombreLibro);

            const string nombreHojaFormato = "Reporte de Formatos";
            if (!libro.Worksheets.Contains(nombreHojaFormato))
            {
                Console.WriteLine(
                    "No podemos procesar este formato, no existe la hoja \"{0}\" lo que indica que la estructura del formato ha sido alterada.",
                    nombreHojaFormato);
                Console.WriteLine("\nFin: {0:G}", DateTime.Now);
                Console.ReadLine();
                return;
            }

            var hojaFormato = libro.Worksheets[nombreHojaFormato];
            var rangoFormato = hojaFormato.GetDataRange();
            var maximaCantidadFilas = rangoFormato.BottomRowIndex;
            var maximaCantidadColumnas = rangoFormato.RightColumnIndex;

            var idFormato = ConvertirCadenaAEntero(ObtenerValorCelda(hojaFormato.Columns[0][0].Value));
            var nombreFormato = ObtenerValorCelda(hojaFormato.Columns[1][2].Value);
            var formato = new Formato(idFormato, nombreFormato);

            // Procesar Campos del Formato
            for (var columna = 0; columna <= maximaCantidadColumnas; columna++)
            {
                var strIdTipoCampo = ObtenerValorCelda(hojaFormato.Columns[columna][3].Value);
                var strIdCampo = ObtenerValorCelda(hojaFormato.Columns[columna][4].Value);
                var celdaNombreCampo = hojaFormato.Columns[columna][6];

                var campo = Campo.FabricarPorTipo(strIdTipoCampo);
                campo.ID = ConvertirCadenaAEntero(strIdCampo);
                campo.Nombre = ObtenerValorCelda(celdaNombreCampo.Value);
                campo.Posicion = new Posicion(nombreHojaFormato, celdaNombreCampo.LeftColumnIndex, celdaNombreCampo.TopRowIndex);

                // Procesar Registros de los Campos del Formato
                var esTipoCampoHora = campo is Hora;
                for (var fila = 7; fila <= maximaCantidadFilas; fila++)
                {
                    var celda = hojaFormato.Columns[columna][fila];

                    // Si es el primer registro entonces revisamos 
                    // si debemos proecesar los registros de los catalogos 
                    // y de las tablas.
                    if (fila == 7)
                    {
                        if (campo is Catalogo)
                            ProcesarCatalogo(campo, libro, hojaFormato.DataValidations.GetDataValidation(celda));

                        if (campo is Tabla)
                            ProcesarTabla(campo, libro);
                    }

                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var registro = new Registro();
                    registro.Numero = fila - 7;
                    registro.Posicion = new Posicion(nombreHojaFormato, celda.LeftColumnIndex, celda.TopRowIndex);
                    registro.Valor = ObtenerValorCelda(celda.Value, esTipoCampoHora);
                    campo.Registros.Add(registro);
                }

                formato.Campos.Add(campo);
            }

            // Validar Formato
            var listaErrores = formato.Validar();

            File.WriteAllText("formato.json", JsonConvert.SerializeObject(formato, Formatting.Indented));
            File.WriteAllText("errores.txt", string.Join("\n", listaErrores.ToList()));

            Console.WriteLine("Total de Errores Encontrados: {0}", listaErrores.Count);
            Console.WriteLine("Criticos: {0}", listaErrores.Count(p => p.Tipo.Equals(TipoError.Critico)));
            Console.WriteLine("Graves: {0}", listaErrores.Count(p => p.Tipo.Equals(TipoError.Grave)));
            Console.WriteLine("Advertencias: {0}", listaErrores.Count(p => p.Tipo.Equals(TipoError.Advertencia)));
            Console.WriteLine("Informativos: {0}", listaErrores.Count(p => p.Tipo.Equals(TipoError.Informativo)));
            
            Console.WriteLine("\nFin: {0:G}", DateTime.Now);

            #if DEBUG
                Console.ReadLine();
            #endif
        }
    }
}

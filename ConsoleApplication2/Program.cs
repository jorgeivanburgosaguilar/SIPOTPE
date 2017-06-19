using System;
using System.Globalization;
using System.IO;
using System.Linq;
using ConsoleApplication2.SIPOTWS;
using ConsoleApplication2.SIPOTWS.Campos;
using DevExpress.Spreadsheet;

namespace ConsoleApplication2
{
    public class Program
    {
        private static string ObtenerValorCelda(CellValue valor, bool esHora = false)
        {
            if (valor == null)
                return string.Empty;

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

            var nombreHoja = criterio.Formula.Remove(0, 1);
            if (string.IsNullOrWhiteSpace(nombreHoja))
                return;

            var hojaCatalogo = libro.Worksheets[nombreHoja];
            var maximaCantidadFilas = hojaCatalogo.Rows.LastUsedIndex;
            var catalogo = (Catalogo) campo;

            for (var fila = 0; fila <= maximaCantidadFilas; fila++)
            {
                var celda = hojaCatalogo.Columns[0][fila];
                catalogo.Elementos.Add(fila, ObtenerValorCelda(celda.Value));
            }
        }

        private static void ProcesarTabla(Campo campo, Workbook libro)
        {
            if (!(campo is Tabla) || libro == null)
                return;

            var tabla = (Tabla) campo;
            var nombreHojaTabla = string.Format("Tabla {0}", tabla.ID);
            var hojaTabla = libro.Worksheets[nombreHojaTabla];
            var maximaCantidadFilas = hojaTabla.Rows.LastUsedIndex;
            var maximaCantidadColumnas = hojaTabla.Columns.LastUsedIndex;

            for (var columna = 0; columna <= maximaCantidadColumnas; columna++)
            {
                var strIdTipoCampo = ObtenerValorCelda(hojaTabla.Columns[columna][0].Value);
                var strIdCampo = ObtenerValorCelda(hojaTabla.Columns[columna][1].Value);
                var strNombreCampo = ObtenerValorCelda(hojaTabla.Columns[columna][2].Value);

                var campoTabla = Campo.FabricarPorTipo(strIdTipoCampo, true);
                campoTabla.ID = string.IsNullOrWhiteSpace(strIdCampo) ? 0 : Convert.ToInt32(strIdCampo);
                campoTabla.Nombre = strNombreCampo;

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
                    registro.Posicion.Hoja = nombreHojaTabla;
                    registro.Posicion.Columna = celda.LeftColumnIndex;
                    registro.Posicion.Fila = celda.TopRowIndex;
                    registro.Valor = ObtenerValorCelda(celda.Value, campoTabla is Hora);
                    campoTabla.Registros.Add(registro);
                }

                tabla.Campos.Add(campoTabla);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Inicio: {0:G}", DateTime.Now);

            var argumento1 = string.Empty;
            if (args.Length > 0)
                argumento1 = args[0];

            var nombreLibro = string.IsNullOrWhiteSpace(argumento1) ? "Formato_Pruebas.xls" : argumento1;
            var libro = new Workbook();
            libro.LoadDocument(nombreLibro);

            const string nombreHoja = "Reporte de Formatos";
            var hojaFormato = libro.Worksheets[nombreHoja];
            var maximaCantidadFilas = hojaFormato.Rows.LastUsedIndex;
            var maximaCantidadColumnas = hojaFormato.Columns.LastUsedIndex;

            var strIdFormato = ObtenerValorCelda(hojaFormato.Columns[0][0].Value);
            var idFormato = string.IsNullOrWhiteSpace(strIdFormato) ? 0 : Convert.ToInt32(strIdFormato);
            var nombreFormato = ObtenerValorCelda(hojaFormato.Columns[1][2].Value);
            var formato = new Formato(idFormato, nombreFormato);

            // Procesar Campos del Formato
            for (var columna = 0; columna <= maximaCantidadColumnas; columna++)
            {
                var strIdTipoCampo = ObtenerValorCelda(hojaFormato.Columns[columna][3].Value);
                var campo = Campo.FabricarPorTipo(strIdTipoCampo);

                var strIdCampo = ObtenerValorCelda(hojaFormato.Columns[columna][4].Value);
                campo.ID = string.IsNullOrWhiteSpace(strIdCampo) ? 0 : Convert.ToInt32(strIdCampo);
                campo.Nombre = ObtenerValorCelda(hojaFormato.Columns[columna][6].Value);

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
                    registro.Posicion.Hoja = nombreHoja;
                    registro.Posicion.Columna = celda.LeftColumnIndex;
                    registro.Posicion.Fila = celda.TopRowIndex;
                    registro.Valor = ObtenerValorCelda(celda.Value, esTipoCampoHora);
                    campo.Registros.Add(registro);
                }

                formato.Campos.Add(campo);
            }

            // Validar Formato
            var listaErrores = formato.Validar();

            File.WriteAllText("errores.txt", string.Join("\n", listaErrores.ToList()));
            Console.WriteLine("\nCantidad Errores Encontrados: {0}\n", listaErrores.Count);
            Console.WriteLine("Fin: {0:G}", DateTime.Now);
            Console.ReadLine();
        }
    }
}

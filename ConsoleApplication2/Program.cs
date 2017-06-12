using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ConsoleApplication2.SIPOTWS;
using ConsoleApplication2.SIPOTWS.Campos;
using DevExpress.Spreadsheet;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace ConsoleApplication2
{
    class Program
    {
        private static string ObtenerValorCelda(CellValue valor, bool esHora)
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

        
        static void Main(string[] args)
        {
            var argumento1 = string.Empty;
            if (args.Length > 0)
                argumento1 = args[0];

            var workbook = new Workbook();
            workbook.LoadDocument(string.IsNullOrWhiteSpace(argumento1) ? "INAIP_F08.xls" : "Formato_Pruebas.xls", DocumentFormat.Xls);
            var nombreHoja = "Reporte de Formatos";
            var hojaFormato = workbook.Worksheets[nombreHoja];
            var maximaCantidadFilas = hojaFormato.Rows.LastUsedIndex;
            var maximaCantidadColumnas = hojaFormato.Columns.LastUsedIndex;

            var idFormato = Convert.ToInt32(hojaFormato.Columns[0][0].Value.ToString());
            var nombreFormato = hojaFormato.Columns[1][2].Value.ToString();
            var formato = new Formato(idFormato, nombreFormato);
            var listaErrores = new List<Error>();

            for (var x = 0; x <= maximaCantidadColumnas; x++)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                var idTipoCampo = Convert.ToInt32(hojaFormato.Columns[x][3].Value.ToString());
                var campo = Campo.FabricarPorTipo(idTipoCampo);
                campo.ID = Convert.ToInt32(hojaFormato.Columns[x][4].Value.ToString());
                campo.Nombre = hojaFormato.Columns[x][6].Value.ToString();

                for (var y = 7; y <= maximaCantidadFilas; y++)
                {
                    var celda = hojaFormato.Columns[x][y];

                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var registro = new Registro();
                    registro.Numero = y - 7;
                    registro.Posicion.Hoja = nombreHoja;
                    registro.Posicion.X = celda.LeftColumnIndex;
                    registro.Posicion.Y = celda.TopRowIndex;
                    registro.Valor = ObtenerValorCelda(celda.Value, campo is Hora);
                    campo.Registros.Add(registro);
                }

                formato.Campos.Add(campo);
                listaErrores.AddRange(campo.ValidarRegistros());
            }


            File.WriteAllText("objeto.json", JsonConvert.SerializeObject(formato, Formatting.Indented));
            File.WriteAllText("errores.json", JsonConvert.SerializeObject(listaErrores, Formatting.Indented));
            Console.WriteLine("Cantidad Errores Encontrados: {0}", listaErrores.Count);
            Console.WriteLine("Fin");
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using ConsoleApplication2.SIPOTWS;
using ConsoleApplication2.SIPOTWS.Enumeradores;
using DevExpress.Spreadsheet;
using Newtonsoft.Json;

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
                case CellValueType.Boolean:
                case CellValueType.Numeric:
                    cadena = valor.ToString();
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
            var workbook = new Workbook();
            workbook.LoadDocument(false ? "INAIP_F08.xls" : "Formato_Pruebas.xls", DocumentFormat.Xls);
            var hojaFormato = workbook.Worksheets["Reporte de Formatos"];
            var maximaCantidadFilas = hojaFormato.Rows.LastUsedIndex;
            var maximaCantidadColumnas = hojaFormato.Columns.LastUsedIndex;

            var idFormato = Convert.ToInt32(hojaFormato.Columns[0][0].Value.ToString());
            var nombreFormato = hojaFormato.Columns[1][2].Value.ToString();
            var formato = new Formato(idFormato, nombreFormato);
            var listaErrores = new List<Error>();

            for (var x = 0; x <= maximaCantidadColumnas; x++)
            {
                var idTipoCampo = Convert.ToInt32(hojaFormato.Columns[x][3].Value.ToString());
                var idCampo = Convert.ToInt32(hojaFormato.Columns[x][4].Value.ToString());
                var etiquetaCampo = hojaFormato.Columns[x][6].Value.ToString();
                var campo = new Campo(idCampo, etiquetaCampo, idTipoCampo);

                for (var y = 7; y <= maximaCantidadFilas; y++)
                {
                    var celda = hojaFormato.Columns[x][y];

                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var registro = new Registro();
                    registro.Numero = y - 7;
                    registro.Posicion = string.Format("{0},{1}", celda.TopRowIndex, celda.LeftColumnIndex);
                    registro.Valor = ObtenerValorCelda(celda.Value, campo.Tipo.Equals(TipoCampo.Hora));
                    campo.Registros.Add(registro);
                }

                formato.Campos.Add(campo);
                listaErrores.AddRange(campo.ValidarRegistros());
            }


            File.WriteAllText("objeto.json", JsonConvert.SerializeObject(formato, Newtonsoft.Json.Formatting.Indented));
            File.WriteAllText("errores.json", JsonConvert.SerializeObject(listaErrores, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("Cantidad Errores Encontrados: {0}", listaErrores.Count);
            Console.WriteLine("Fin");
            Console.ReadLine();
        }
    }
}

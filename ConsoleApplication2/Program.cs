using System;
using System.Collections.Generic;
using System.IO;
using ConsoleApplication2.SIPOTWS;
using ConsoleApplication2.SIPOTWS.Enumeradores;
using DevExpress.Spreadsheet;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            var prueba = true;
            var workbook = new Workbook();
            workbook.LoadDocument(prueba ? "Formato_Pruebas.xls" : "INAIP_F08.xls", DocumentFormat.Xls);
            var hojaFormato = workbook.Worksheets["Reporte de Formatos"];
            var maximaCantidadFilas = hojaFormato.Rows.LastUsedIndex;
            var maximaCantidadColumnas = hojaFormato.Columns.LastUsedIndex;

            var idFormato = Convert.ToInt32(hojaFormato.Columns[0][0].Value.ToString());
            var nombreFormato = hojaFormato.Columns[1][2].Value.ToString();
            var formato = new Formato(idFormato, nombreFormato);
            var listaErrores = new List<string>();

            for (var x = 0; x <= maximaCantidadColumnas; x++)
            {
                var idTipoCampo = Convert.ToInt32(hojaFormato.Columns[x][3].Value.ToString());
                var idCampo = Convert.ToInt32(hojaFormato.Columns[x][4].Value.ToString());
                var etiquetaCampo = hojaFormato.Columns[x][6].Value.ToString();
                var campo = new Campo(idCampo, etiquetaCampo, idTipoCampo);

                for (var y = 7; y <= maximaCantidadFilas; y++)
                {
                    var celda = hojaFormato.Columns[x][y];

                    var registro = new Registro();
                    registro.Numero = y - 7;
                    registro.Posicion = string.Format("{0},{1}", celda.TopRowIndex, celda.LeftColumnIndex);
                    registro.EstablecerValor(celda.Value.ToString(), campo.TipoCampo);
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

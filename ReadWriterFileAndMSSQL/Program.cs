using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;
using System.Data;
using Microsoft.Extensions.Configuration;


namespace ReadWriterFileAndMSSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();

            while (true)
            {
                try
                {

                    Console.WriteLine("Выполнить команду? Y или N");
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Y)
                    {

                        
                        string pathTxtFile = @"input.txt";
                        var inputTxtLine = ReadText(pathTxtFile);

                        
                        
                        string connectionString = config.GetConnectionString("DBInfo");
                        var dateSql = ReadDb(connectionString, inputTxtLine);


                        string pathCsvFile = @"output.csv";
                        WriterCsv(pathCsvFile, dateSql);
                    }
                    else if (key.Key == ConsoleKey.N)
                    {
                        return;
                    }
                    else 
                    {
                        Console.WriteLine("Не та кнопка");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

        }




        private static void WriterCsv(string pathCsvFile, DataSet dateSql)
        {
            using (var writer = new StreamWriter(pathCsvFile))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {

                foreach (DataTable dt in dateSql.Tables)
                {
                    foreach (DataColumn column in dt.Columns)
                        csvWriter.WriteField(column.ColumnName);
                    csvWriter.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        var cells = row.ItemArray;
                        foreach (object cell in cells)
                            csvWriter.WriteField(cell);
                        csvWriter.NextRecord();
                    }
                }

            }
        }

        private static DataSet ReadDb(string connectionString, IEnumerable<string> sqlExpression)
        {
            DataSet dateSql = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                foreach(var i in sqlExpression)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(i, connection);
                    adapter.Fill(dateSql);
                }

            }

            return dateSql;
        }

        private static IEnumerable<string> ReadText(string pathTxtFile)
        {
            List<string> inputTxtLine = new List<string>();
            var line = File.ReadLines(pathTxtFile);
            foreach (var l in line)
            {
                inputTxtLine.Add(l);
            }
            return inputTxtLine;
        }
    }
}

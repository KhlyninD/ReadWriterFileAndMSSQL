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

                        List<string> inputTxtLine = new List<string>();
                        string pathTxtFile = @"input.txt";

                        inputTxtLine.AddRange(ReadText(pathTxtFile));


                        DataSet dateSql = new DataSet();
                        string sqlExpression = inputTxtLine[0];

                        string connectionString = config.GetConnectionString("ConnectionString");

                        ReadDb(connectionString, sqlExpression, dateSql);


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

        private static void ReadDb(string connectionString, string sqlExpression, DataSet dateSql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, connection);
                adapter.Fill(dateSql);
            }
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

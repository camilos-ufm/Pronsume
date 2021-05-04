using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CsvHelper;

namespace ProyectoFinal
{
    public class csv_reader
    {
        static List<String> read_csv(String filename)
        {
            List<String> stringList = new List<string>();
            using var streamReader = File.OpenText(filename);
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);
            // csvReader.Configuration.HasHeaderRecord = true;

            string value;

            while (csvReader.Read())
            {
                for (int i = 0; csvReader.TryGetField<string>(i, out value); i++)
                {
                    Console.Write($"{value}\n");
                    stringList.Add($"{value}");
                }

            }

            return stringList;
        }
    }
}
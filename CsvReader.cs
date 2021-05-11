using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CsvHelper;
using System.Linq;

namespace ProyectoFinal
{
    public class CsvReader
    {
        public static List<String> read_csv(String filename)
        {
            List<String> stringList = new List<string>();
            var lines = File.ReadLines(filename).Skip(1);
            // using var streamReader = File.OpenText(filename);
            // using var csvReader = new CsvHelper.CsvReader(streamReader, CultureInfo.CurrentCulture);
            // // csvReader.Configuration.HasHeaderRecord = true;

            // string value;

            // while (csvReader.Read())
            // {
            //     for (int i = 0; csvReader.TryGetField<string>(i, out value); i++)
            //     {
            //         Console.Write($"{value}\n");
            //         stringList.Add($"{value}");
            //     }

            // }

            // return stringList;

            foreach (string line in lines)
            {
                stringList.Add(line);
            }
            return stringList;
        }
    }
}
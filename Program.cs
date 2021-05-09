using System;
using System.Collections.Generic;

namespace ProyectoFinal
{
    class Program
    {
        static void Main(string[] args)
        {
            List<String> csv = csv_reader.read_csv("csv/personas.csv");
            Console.WriteLine("Hello World!");
            var sql_c = new sql_connector("localhost", "dbuser", "password", "db");
            sql_c.sqlConnect();
        }
    }
}

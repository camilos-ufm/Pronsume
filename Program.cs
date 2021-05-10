using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProyectoFinal
{
    class Program
    {
        public static int buffer_size { get; set; }
        static int producers { get; set; }
        static String consumers { get; set; }
        static int alt { get; set; }

        static SqlConnector sql_c { get; set; }
        static void Main(string[] args)
        {
            if (args.Length == 5)
            {
                try
                {
                    buffer_size = int.Parse(args[1]);
                    producers = int.Parse(args[2]);
                    consumers = args[3].ToString();
                    alt = int.Parse(args[4]);
                    List<String> csv = CsvReader.read_csv($"csv/{consumers}");
                    List<String> personasCsvList = CsvReader.read_csv($"csv/personas.csv");
                    var listOfPersons = personasCsvList.Select(line => new Person(line)).ToList();
                    Person.printPersons(listOfPersons);
                    sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                    sql_c.sqlConnect();
                    sql_c.createTable();
                }
                catch (System.FormatException)
                {
                    Console.WriteLine($"Error: Wrong parameter type! Please check your paramters!");
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    Console.WriteLine($"Error: No directory '{consumers}' found!");
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine($"Error: No file named '{consumers}' found!");
                }
                catch (MySql.Data.MySqlClient.MySqlException)
                {
                    Console.WriteLine($"Error: Wrong MySQL settings!");
                }
            }
            else
            {
                Console.WriteLine("Error: Wrong number of arguments!");
                Console.WriteLine("Parameters needed:\n\t1. Buffer size\n\t2. Producers\n\t3. Consumers\n\t4. Alternance");
            }
        }
    }
}

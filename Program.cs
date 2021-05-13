using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var watch = Stopwatch.StartNew();
            if (args.Length == 5)
            {
                try
                {
                    buffer_size = int.Parse(args[1]);
                    producers = int.Parse(args[2]);
                    consumers = args[3].ToString();
                    alt = int.Parse(args[4]);
                    List<String> csv = CsvReader.read_csv($"csv/{consumers}");
                    int consumersSize = csv.Count;
                    List<String> personasCsvList = CsvReader.read_csv($"csv/personas.csv");
                    var listOfPersons = personasCsvList.Select(line => new Person(line)).ToList();
                    // Person.printPersons(listOfPersons);
                    // start timer

                    sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                    // sql_c.sqlConnect();
                    sql_c.createTable();
                    Buffer.listOfPersons = listOfPersons;
                    Buffer.pronsume(producers, consumersSize, buffer_size, alt, watch);
                    // stop timer


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
            else if (args.Length == 4)
            {
                Console.WriteLine("==4");
                try
                {
                    buffer_size = int.Parse(args[1]);
                    producers = int.Parse(args[2]);
                    alt = int.Parse(args[3]);
                    List<String> csv = CsvReader.read_csv($"csv/def.csv");
                    int consumersSize = csv.Count;
                    List<String> personasCsvList = CsvReader.read_csv($"csv/personas.csv");
                    var listOfPersons = personasCsvList.Select(line => new Person(line)).ToList();
                    // Person.printPersons(listOfPersons);
                    sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                    // sql_c.sqlConnect();
                    sql_c.createTable();
                    Buffer.listOfPersons = listOfPersons;
                    Buffer.pronsume(producers, consumersSize, buffer_size, alt, watch);
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

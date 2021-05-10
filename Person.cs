using System;
using System.Collections.Generic;

namespace ProyectoFinal
{
    public class Person
    {
        public String id { get; set; }
        public String name { get; set; }
        public String phone { get; set; }
        public String date { get; set; }
        public String city { get; set; }
        public bool is_produced { get; set; }

        public Person(string line)
        {
            var split = line.Split(',');
            this.id = split[0];
            this.name = (split[1] + split[2]).Replace('"', ' ');
            this.phone = split[3];
            this.date = split[4];
            this.city = split[5];
            this.is_produced = false;

        }

        public static void printPersons(List<Person> listOfPersons)
        {
            foreach (var item in listOfPersons)
            {
                Console.WriteLine($"id: {item.id}");
                Console.WriteLine($"name:{item.name}");
                Console.WriteLine($"phone: {item.phone}");
                Console.WriteLine($"date: {item.date}");
                Console.WriteLine($"city: {item.city}");
                Console.WriteLine($"is_produced: {item.is_produced}");
                Console.WriteLine();
            }
        }

    }
}
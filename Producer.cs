using System;
using System.Collections.Generic;

namespace ProyectoFinal
{
    public class Producer
    {
        public string name { get; set; }
        public List<long> time;
        public int total { get; set; }

        public Producer(string Name)
        {
            this.name = Name;
            this.time = new List<long>();
            this.total = 0;
        }

        public long totalTime(){
            long totalTime = 0;
            foreach (var item in this.time)
            {
                // Console.WriteLine($"this.time: {item}");
                totalTime += item;
            }
            // Console.WriteLine($"Total: {this.total}");
            totalTime = totalTime / this.total;
            return totalTime;
        }
    }
}
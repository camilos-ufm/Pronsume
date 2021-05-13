using System.Diagnostics;

namespace ProyectoFinal
{
    public class Producer
    {
        string name { get; set; }
        Stopwatch time { get; set; }
        int total { get; set; }

        public Producer(string Name, Stopwatch Time, int Total)
        {
            this.name = Name;
            this.time = Time;
            this.total = Total;
        }
    }
}
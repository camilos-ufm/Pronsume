using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFinal
{
    public class Buffer
    {
        static Random rnd = new Random();
        static int producedCount = 0;
        static int productorsCount = 0;
        static float sumTimeProd = 0;
        static List<float> avgProdTime;

        public static List<Producer> listOfProducers = new List<Producer>();
        public static List<Person> listOfPersons { get; set; }
        public static void pronsume(int producersSize, int consumersSize, int bufferSize, int alternance, Stopwatch watch)
        {
            Semaphore fillCount = new Semaphore(0, bufferSize);
            Semaphore emptyCount = new Semaphore(bufferSize, bufferSize);
            Semaphore mutex = new Semaphore(1, 1);
            Semaphore bufferIsFull = new Semaphore(0, 1);
            BlockingCollection<Person> productsBuffer = new BlockingCollection<Person>(bufferSize);
            int numOfThreads = producersSize;


            Action producer = () =>
            {
                while (true)
                {
                    var watch = Stopwatch.StartNew();
                    emptyCount.WaitOne();
                    mutex.WaitOne();
                    try
                    {
                        var nonProducedPersons = from myobject in listOfPersons
                                                 where myobject.is_produced == false
                                                 select myobject;


                        int r = rnd.Next(nonProducedPersons.Count());
                        Person randomPerson = nonProducedPersons.ElementAt(r);
                        randomPerson.is_produced = false;
                        randomPerson.produced_by = Thread.CurrentThread.Name.ToString();
                        Console.WriteLine($"Person Selected {randomPerson.name} {Thread.CurrentThread.Name}");
                        productsBuffer.Add(randomPerson);
                        listOfPersons.Remove(randomPerson);
                    }
                    catch
                    {
                        Console.WriteLine("No more persons on memory!.");
                    }
                    var currentProductor = from myobject in listOfProducers
                                           where myobject.name.Equals(Thread.CurrentThread.Name.ToString())
                                           select myobject;
                    Producer p = currentProductor.First();
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    p.time.Add(elapsedMs);
                    p.total += 1;
                    // select lista de producers where name == currenthread
                    // set valor de tiempo = esto que yo tengo
                    // totel de producidos +=1
                    mutex.Release();
                    fillCount.Release();
                    producedCount++;
                    if (producedCount == bufferSize)
                    {
                        Console.WriteLine("******BUFFER FILLED UP******");
                        if (alternance == 1)
                            bufferIsFull.Release();
                        // fillCount.Release();
                    }

                    // Console.WriteLine($"Total elapsed time: {elapsedMs}ms");
                }
            };
            Action consumer = () =>
            {
                if (alternance == 1)
                    bufferIsFull.WaitOne();
                while (true)
                {
                    Console.WriteLine("ANDRES Y SU PSICO");
                    fillCount.WaitOne();
                    mutex.WaitOne();
                    Console.WriteLine($"Consuming {Thread.CurrentThread.Name}");
                    Person personConsumed;
                    if (productsBuffer.TryTake(out personConsumed))
                    {
                        Console.WriteLine($"Person to consume: {personConsumed.name} Cnt:{productsBuffer.Count}");
                        var sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                        sql_c.insertIntoTable(personConsumed, Thread.CurrentThread.Name);
                        producedCount--;
                    }
                    else
                    {
                        watch.Stop();
                        var elapsedMs = watch.ElapsedMilliseconds;
                        foreach (var item in listOfProducers)
                        {
                            Console.WriteLine($"Producer.{item.name}: {item.averageTime()}ms");
                        }
                        Console.WriteLine($"Total elapsed time: {elapsedMs}ms");
                        System.Environment.Exit(0);
                    }
                    mutex.Release();
                    emptyCount.Release();
                }
            };


            List<Thread> listOfThreads = new List<Thread>();
            List<Thread> listOfThreads2 = new List<Thread>();
            var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
            var handle2 = new EventWaitHandle(false, EventResetMode.ManualReset);
            for (int i = 0; i < numOfThreads; i++)
            {
                string name = $"P{i + 1}";
                Producer p = new Producer(name);
                ThreadStart ts = new ThreadStart(producer);
                Thread t = new Thread(ts);
                t.Name = name;
                listOfProducers.Add(p);
                t.Start();
                listOfThreads.Add(t);
            }
            for (int i = 0; i < consumersSize; i++)
            {
                ThreadStart ts = new ThreadStart(consumer);
                Thread t = new Thread(ts);
                t.Name = $"C{i + 1}";
                t.Start();
                listOfThreads2.Add(t);

            }
        }

    }
}
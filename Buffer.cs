using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFinal
{
    public class Buffer
    {
        public static void pronsume(int producersSize, int consumersSize, int bufferSize, List<Person> listOfPersons)
        {
            var syncRoot = new object();
            // var productsBuffer = new List<int>();
            BlockingCollection<Person> productsBuffer = new BlockingCollection<Person>(bufferSize);
            int numOfThreads = producersSize;
            WaitHandle[] waitHandles = new WaitHandle[numOfThreads];
            var producedCount = 0;


            Action producer = () =>
            {
                lock (syncRoot)
                {

                    while (true)
                    {
                        Monitor.Pulse(syncRoot);
                        try
                        {
                            var nonProducedPersons = from myobject in listOfPersons
                                                     where myobject.is_produced == false
                                                     select myobject;
                            foreach (var nonProducedPerson in nonProducedPersons)
                            {
                                Thread.Sleep(500);
                                if (producedCount == bufferSize)
                                {
                                    Console.WriteLine($"*****Buffer has filled up {productsBuffer.Count} == Cnt: {producedCount}*****");
                                    Monitor.Wait(syncRoot);
                                    Monitor.Exit(syncRoot);
                                    // productsBuffer.CompleteAdding();

                                }
                                else
                                {
                                    Monitor.Enter(syncRoot);
                                    Monitor.Pulse(syncRoot);
                                    productsBuffer.Add(nonProducedPerson);
                                    Console.WriteLine($"Producing {producedCount} {nonProducedPerson.name} w/ {Thread.CurrentThread.Name}");
                                    nonProducedPerson.is_produced = true;
                                    producedCount++;
                                }
                            }
                        }
                        catch (System.Exception e)
                        {

                            Console.WriteLine(e.Message);
                        }
                        Monitor.Pulse(syncRoot);
                        Monitor.Wait(syncRoot);
                    }
                }
            };

            Action consumer = () =>
            {
                lock (syncRoot)
                    while (true)
                    {
                        // Thread.Sleep(500);
                        Console.WriteLine($"Consuming w/ thread: {Thread.CurrentThread.Name}");
                        Console.WriteLine($"producedCount: {producedCount}");
                        if (producedCount == 5)
                        {
                            foreach (Person personConsumed in productsBuffer.GetConsumingEnumerable())
                            {
                                Monitor.Enter(syncRoot);
                                Monitor.Pulse(syncRoot);
                                Thread.Sleep(500);
                                var sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                                Console.WriteLine($"Person to consume: {personConsumed.name} Cnt: {productsBuffer.Count}");
                                sql_c.insertIntoTable(personConsumed, Thread.CurrentThread.Name);
                                producedCount--;
                                Console.WriteLine($"Produced Count 0 == {producedCount}");
                                Monitor.Exit(syncRoot);
                                Monitor.Pulse(syncRoot);
                            }
                            Console.WriteLine("Trying to takeout every element of collection.");
                        }

                        Monitor.Wait(syncRoot);
                    }
            };

            var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
            for (int i = 0; i < numOfThreads; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = $"P{i}";
                    producer();
                });
            }
            for (int i = 0; i < consumersSize; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = $"C{i}";
                    consumer();
                });
            }

            Console.ReadLine();
        }

    }
}
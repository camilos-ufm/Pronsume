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
        static Random rnd = new Random();
        static int producedCount = 0;
        public static void pronsume(int producersSize, int consumersSize, int bufferSize, List<Person> listOfPersons)
        {
            Semaphore fillCount = new Semaphore(0, bufferSize);
            Semaphore emptyCount = new Semaphore(bufferSize, bufferSize);
            Semaphore mutex = new Semaphore(1, 1);
            BlockingCollection<Person> productsBuffer = new BlockingCollection<Person>(bufferSize);
            int numOfThreads = producersSize;

            Action producer = () =>
            {
                emptyCount.WaitOne();
                var nonProducedPersons = from myobject in listOfPersons
                                         where myobject.is_produced == false
                                         select myobject;


                int r = rnd.Next(nonProducedPersons.Count());
                Person randomPerson = nonProducedPersons.ElementAt(r);
                randomPerson.is_produced = false;
                Console.WriteLine($"Person Selected {randomPerson.name} {Thread.CurrentThread.Name}");
                productsBuffer.Add(randomPerson);
                emptyCount.Release();
                producedCount++;
                if (producedCount == bufferSize)
                {
                    Console.WriteLine("******BUFFER FILLED UP******");
                    fillCount.Release();
                }
            };
            Action consumer = () =>
            {
                fillCount.WaitOne();
                Console.WriteLine($"Consuming {Thread.CurrentThread.Name}");
                Person personConsumed = productsBuffer.Take();
                Console.WriteLine($"Person to consume: {personConsumed.name} Cnt:{productsBuffer.Count}");
                var sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                sql_c.insertIntoTable(personConsumed, Thread.CurrentThread.Name);
                producedCount--;
                // Thread.Sleep(500);
                // foreach (var item in productsBuffer.GetConsumingEnumerable())
                // {
                //     Console.WriteLine($"Emptying item: {item.name} cnt: {productsBuffer.Count}");
                // }
                // fillCount.Release();
            };

            // var syncRoot = new object();
            // // var productsBuffer = new List<int>();
            // BlockingCollection<Person> productsBuffer = new BlockingCollection<Person>(bufferSize);
            // int numOfThreads = producersSize;
            // WaitHandle[] waitHandles = new WaitHandle[numOfThreads];
            // var producedCount = 0;


            // Action producer = () =>
            // {
            //     // lock (syncRoot)
            //     // {

            //     Monitor.Enter(syncRoot);
            //     Monitor.Pulse(syncRoot);
            //     while (true)
            //     {
            //         try
            //         {
            //             var nonProducedPersons = from myobject in listOfPersons
            //                                      where myobject.is_produced == false
            //                                      select myobject;
            //             foreach (var nonProducedPerson in nonProducedPersons)
            //             {
            //                 Thread.Sleep(500);
            //                 if (producedCount == bufferSize)
            //                 {
            //                     Console.WriteLine($"*****Buffer has filled up {productsBuffer.Count} == Cnt: {producedCount}*****");
            //                     Monitor.Wait(syncRoot);
            //                     Monitor.Pulse(syncRoot);
            //                     // Monitor.Exit(syncRoot); 
            //                     // productsBuffer.CompleteAdding();

            //                 }
            //                 else
            //                 {
            //                     Monitor.Pulse(syncRoot);
            //                     Monitor.Enter(syncRoot);
            //                     nonProducedPerson.is_produced = true;
            //                     Console.WriteLine($"Producing {producedCount} {nonProducedPerson.name} {nonProducedPerson.is_produced} w/ {Thread.CurrentThread.Name}");
            //                     productsBuffer.Add(nonProducedPerson);
            //                     producedCount++;
            //                 }
            //             }
            //         }
            //         catch (System.Exception e)
            //         {

            //             Console.WriteLine(e.Message);
            //         }
            //         finally
            //         {

            //             Monitor.Exit(syncRoot);
            //             Monitor.Pulse(syncRoot);
            //         }
            //         // Monitor.Pulse(syncRoot);
            //         // Monitor.Wait(syncRoot);
            //     }
            //     // }
            // };

            // Action consumer = () =>
            // {
            //     // lock (syncRoot)
            //     // Monitor.Enter(syncRoot);
            //     Thread.Sleep(500);
            //     while (true)
            //     {
            //         Console.WriteLine($"Consuming w/ thread: {Thread.CurrentThread.Name}");
            //         Console.WriteLine($"producedCount: {producedCount}");
            //         // if (producedCount == 5)
            //         // {
            //         foreach (Person personConsumed in productsBuffer.GetConsumingEnumerable())
            //         {
            //             // Monitor.Enter(syncRoot);
            //             // Monitor.Pulse(syncRoot);
            //             // Thread.Sleep(500);
            //             var sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
            //             Console.WriteLine($"Person to consume: {personConsumed.name} Cnt: {productsBuffer.Count}");
            //             sql_c.insertIntoTable(personConsumed, Thread.CurrentThread.Name);
            //             producedCount--;
            //             Console.WriteLine($"Produced Count 0 == {producedCount}");
            //             Monitor.Exit(syncRoot);
            //             Monitor.Pulse(syncRoot);
            //         }
            //         Console.WriteLine("Trying to takeout every element of collection.");
            //         // }
            //         if (producedCount == 0)
            //         {
            //             Console.WriteLine("ProduceCount 000000000");
            //             Monitor.Exit(syncRoot);
            //             Monitor.Pulse(syncRoot);
            //             // Thread.Sleep(500);
            //             // Monitor.Wait(syncRoot);
            //         }

            //         // Monitor.Wait(syncRoot);
            //     }
            // };

            var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
            var handle2 = new EventWaitHandle(false, EventResetMode.ManualReset);
            for (int i = 0; i < numOfThreads; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = $"P{i}";
                    producer();
                    handle.Set();
                });
            }
            for (int i = 0; i < consumersSize; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = $"C{i}";
                    consumer();
                    handle2.Set();
                });
            }
            handle.WaitOne();
            handle2.WaitOne();

            Console.ReadLine();
        }

    }
}
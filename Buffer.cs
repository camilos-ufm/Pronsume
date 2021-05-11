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
                        // Console.WriteLine("Producing");
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
                                    // Console.WriteLine($"Buffer not yet filled up {productsBuffer.Count}");
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
                        // Monitor.Pulse(syncRoot);
                        // Monitor.Wait(syncRoot);
                        // productsBuffer.ForEach(Console.WriteLine);
                        // productsBuffer.ForEach(delegate (Person person) { sql_c.insertIntoTable(person, Thread.CurrentThread.Name); });
                        // productsBuffer.ForEach(delegate (Person person) { Console.WriteLine(person.name); });
                        // foreach (Person personConsumed in productsBuffer)
                        // {
                        //     var sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                        //     Console.WriteLine($"Person to consume: {personConsumed.name}");
                        //     sql_c.insertIntoTable(personConsumed, Thread.CurrentThread.Name);
                        //     // if (personConsumed.id == productsBuffer.Last().id)
                        //     // {
                        //     //     lastItemInBuffer = true;
                        //     //     Console.WriteLine($"*********Consuming last person in buffer*********");
                        //     // }
                        // }
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
                                // if (personConsumed.id == productsBuffer.Last().id)
                                // {
                                //     lastItemInBuffer = true;
                                //     Console.WriteLine($"*********Consuming last person in buffer*********");
                                // }
                                Monitor.Exit(syncRoot);
                                Monitor.Pulse(syncRoot);
                            }
                            Console.WriteLine("Trying to takeout every element of collection.");
                            // Monitor.Enter(syncRoot);
                            // Monitor.Pulse(syncRoot);
                            // while (productsBuffer.TryTake(out _)) { }
                            // foreach (var person in productsBuffer.GetConsumingEnumerable())
                            // {
                            //     Console.WriteLine($"REMOVING: {person.name} Count: {productsBuffer.Count}");
                            // }
                            // Console.WriteLine(productsBuffer.Count);
                            // producedCount = 0;

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

        // public Array<int> buffer { get; set; }

        // public List<Person> buffer { get; set; }

        // private Semaphore semProducer = new Semaphore(1, 1);

        // private Semaphore semConsumer = new Semaphore(0, 1);

        // public Buffer(int buffer_size)
        // {

        // }
        // // https://stackoverflow.com/a/42197839

        // private BlockingCollection<Person> mReceivingThreadQueue = new BlockingCollection<Person>();
        // private BlockingCollection<Person> mSendingThreadQueue = new BlockingCollection<Person>();

        // public void Stop()
        // {
        //     // No need for mIsRunning. Makes the enumerables in the GetConsumingEnumerable() calls
        //     // below to complete.
        //     mReceivingThreadQueue.CompleteAdding();
        //     mSendingThreadQueue.CompleteAdding();
        // }

        // private void ReceivingThread()
        // {
        //     foreach (Person item in mReceivingThreadQueue.GetConsumingEnumerable())
        //     {
        //         consume(item);
        //     }
        // }

        // private void SendingThread()
        // {
        //     foreach (Person item in mSendingThreadQueue.GetConsumingEnumerable())
        //     {
        //         produce(item);
        //     }
        // }

        // internal void EnqueueRecevingData(Person info)
        // {
        //     // You can also use TryAdd() if there is a possibility that you
        //     // can add items after you have stopped. Otherwise, this can throw an
        //     // an exception after CompleteAdding() has been called.
        //     mReceivingThreadQueue.Add(info);
        // }

        // public void EnqueueSend(Person info)
        // {
        //     mSendingThreadQueue.Add(info);
        // }

        // public void produce(Person value)
        // {
        //     try
        //     {
        //         semProducer.WaitOne();
        //     }
        //     catch (SystemException ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //     }

        //     this.buffer.Add(value);
        //     Console.WriteLine("Producer push " + value + " to buffer in " + Thread.CurrentThread);

        //     semConsumer.Release();
        // }

        // public void consume(Person value)
        // {
        //     try
        //     {
        //         this.semConsumer.WaitOne();
        //     }
        //     catch (SystemException ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //     }

        //     Console.WriteLine("Consumer is consuming element " + this.buffer + " in " + Thread.CurrentThread.Name);

        //     semProducer.Release();
        // }

    }
}
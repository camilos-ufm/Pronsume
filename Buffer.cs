using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFinal
{
    public class Buffer
    {
        public static void pronsume(int producers, List<Person> listOfPersons)
        {
            var syncRoot = new object();
            // var productsBuffer = new List<int>();
            List<Person> productsBuffer = new List<Person>(10);
            int numOfThreads = producers;
            WaitHandle[] waitHandles = new WaitHandle[numOfThreads];


            Action producer = () =>
            {
                lock (syncRoot)
                {
                    var counter = 0;
                    while (true)
                    {
                        // Console.WriteLine("Producing");
                        try
                        {
                            var results = from myobject in listOfPersons
                                          where myobject.is_produced == false
                                          select myobject;
                            foreach (var item in results)
                            {
                                Console.WriteLine($"P. Adding {item.name}");
                                productsBuffer.Add(item);
                                item.is_produced = true;
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
                        Console.WriteLine("Consuming");
                        Monitor.Pulse(syncRoot);
                        // productsBuffer.ForEach(Console.WriteLine);
                        // productsBuffer.ForEach(delegate (Person person) { sql_c.insertIntoTable(person, Thread.CurrentThread.Name); });
                        // productsBuffer.ForEach(delegate (Person person) { Console.WriteLine(person.name); });

                        Thread.Sleep(500);
                        foreach (Person personConsumed in productsBuffer)
                        {
                            var sql_c = new SqlConnector("localhost", "dbuser", "password", "db");
                            // sql_c.sqlConnect();
                            Console.WriteLine("test");
                            sql_c.insertIntoTable(personConsumed, Thread.CurrentThread.Name);
                            Console.WriteLine($"Person to consume: {personConsumed.name}");
                        }
                        productsBuffer.Clear();
                        Thread.Sleep(500);
                        Console.WriteLine($"Consumer thread: {Thread.CurrentThread.Name}");

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
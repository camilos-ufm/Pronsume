using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;

namespace ProyectoFinal
{
    public class Buffer
    {

        // public Array<int> buffer { get; set; }

        public List<Person> buffer { get; set; }

        private Semaphore semProducer = new Semaphore(1, 1);

        private Semaphore semConsumer = new Semaphore(0, 1);

        public Buffer(int buffer_size)
        {
            
        }
        // https://stackoverflow.com/a/42197839

        private BlockingCollection<Person> mReceivingThreadQueue = new BlockingCollection<Person>();
        private BlockingCollection<Person> mSendingThreadQueue = new BlockingCollection<Person>();

        public void Stop()
        {
            // No need for mIsRunning. Makes the enumerables in the GetConsumingEnumerable() calls
            // below to complete.
            mReceivingThreadQueue.CompleteAdding();
            mSendingThreadQueue.CompleteAdding();
        }

        private void ReceivingThread()
        {
            foreach (Person item in mReceivingThreadQueue.GetConsumingEnumerable())
            {
                consume(item);
            }
        }

        private void SendingThread()
        {
            foreach (Person item in mSendingThreadQueue.GetConsumingEnumerable())
            {
                produce(item);
            }
        }

        internal void EnqueueRecevingData(Person info)
        {
            // You can also use TryAdd() if there is a possibility that you
            // can add items after you have stopped. Otherwise, this can throw an
            // an exception after CompleteAdding() has been called.
            mReceivingThreadQueue.Add(info);
        }

        public void EnqueueSend(Person info)
        {
            mSendingThreadQueue.Add(info);
        }

        public void produce(Person value)
        {
            try
            {
                semProducer.WaitOne();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }

            this.buffer.Add(value);
            Console.WriteLine("Producer push " + value + " to buffer in " + Thread.CurrentThread);

            semConsumer.Release();
        }

        public void consume(Person value)
        {
            try
            {
                this.semConsumer.WaitOne();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Consumer is consuming element " + this.buffer + " in " + Thread.CurrentThread.Name);

            semProducer.Release();
        }

    }
}
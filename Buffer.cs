using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace ProyectoFinal
{
    public class Buffer
    {

        // public Array<int> buffer { get; set; }

        ArrayList buffer;

        private Semaphore semProducer = new Semaphore(1, 1);

        private Semaphore semConsumer = new Semaphore(0, 1);

        public Buffer(int buffer_size)
        {
            
        }

        public void produce(int value)
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

        public void consume()
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
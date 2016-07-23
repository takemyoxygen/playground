using Binary.Library;
using System;
using System.Collections.Generic;

namespace Binary.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new DummyConnection();

            var typeMap = new DictionaryTypeMap(
                new Dictionary<Type, uint>
                {
                    {typeof(Message), 13}
                });


            var sender = new Sender<Message>(connection, typeMap);
            var receiver = new Receiver<Message>(connection, typeMap);
            receiver.MessageReceived += m => Console.WriteLine($"Received message \"{m.Text}\"");

            var msg = new Message("message text");

            sender.Send(msg);

            Console.WriteLine("Press <Enter> to terminate the application...");
            Console.ReadLine();
        }
    }
}

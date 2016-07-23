using Binary.Library;
using System;

namespace Binary.Client
{
    internal sealed class DummyConnection : IConnection
    {
        public event Action<uint, byte[]> MessageReceived;

        public void Send(uint type, byte[] content)
        {
            Console.WriteLine($"Sending message of type {type} consisting of {content.Length} bytes");
            this.MessageReceived?.Invoke(type, content);
        }
    }
}

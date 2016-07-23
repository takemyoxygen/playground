using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Binary.Library
{
    public sealed class Receiver<T> : IDisposable where T : IMessage
    {
        private readonly IConnection connection;

        private readonly Func<byte[], T> deserialize;

        private readonly uint binaryType;

        public event Action<T> MessageReceived;

        public Receiver(IConnection connection, ITypeMap typeMap)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (typeMap == null) throw new ArgumentNullException(nameof(typeMap));

            this.connection = connection;
            this.connection.MessageReceived += this.OnMessageReceived;
            this.deserialize = this.CreateDeserializer();
            this.binaryType = typeMap.BinaryTypeFor<T>();
        }

        private Func<byte[], T> CreateDeserializer()
        {
            var method = typeof(T).GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public);
            var param = Expression.Parameter(typeof(byte[]));
            var parseFrom = Expression.Call(method, param);

            return Expression
                .Lambda<Func<byte[], T>>(parseFrom, param)
                .Compile();
        }

        private void OnMessageReceived(uint type, byte[] content)
        {
            if (type == this.binaryType)
            {
                this.MessageReceived?.Invoke(this.deserialize(content));
            }
        }

        public void Dispose()
        {
            this.connection.MessageReceived -= this.OnMessageReceived;
        }
    }
}

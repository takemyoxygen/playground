using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Binary.Library
{
    public sealed class Sender<T> where T : IMessage
    {
        private readonly IConnection connection;

        private readonly uint binaryType;

        public Sender(IConnection connection, ITypeMap typeMap)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (typeMap == null) throw new ArgumentNullException(nameof(typeMap));

            this.connection = connection;
            this.binaryType = typeMap.BinaryTypeFor<T>();
        }

        public void Send(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            this.connection.Send(this.binaryType, message.Serialize());
        }
    }
}

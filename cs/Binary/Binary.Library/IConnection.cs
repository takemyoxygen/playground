using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binary.Library
{
    public interface IConnection
    {
        event Action<uint, byte[]> MessageReceived;

        void Send(uint type, byte[] content);
    }
}

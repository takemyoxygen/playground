using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binary.Library
{
    public interface IMessage
    {
        byte[] Serialize();
    }
}

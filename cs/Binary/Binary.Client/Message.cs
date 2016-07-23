using Binary.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binary.Client
{
    internal sealed class Message : IMessage
    {
        public string Text { get; }

        public Message(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            this.Text = text;
        }

        public byte[] Serialize() => Encoding.UTF8.GetBytes(this.Text);

        public static Message Deserialize(byte[] content) =>
            new Message(Encoding.UTF8.GetString(content));

    }
}

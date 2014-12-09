using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatUI.Backend
{
    /// <summary>
    /// This class is a container for a chatMessage and the chatID it belongs to.
    /// It can return a byte array containing both information before sending to the stream.
    /// It can also parse a byte array from that same stream to return both values.
    /// </summary>
    public class Message : EventArgs
    {
        private int chatID;
        private byte[] msg;

        public Message(int id, string message)
        {
            chatID = id;
            msg = Encoding.ASCII.GetBytes(message);
        }

        // reads a raw message byte array containing the message and the chatID appended to the end
        public Message(byte[] msg)
        {
            chatID = BitConverter.ToInt32(msg, msg.Length - 4);
            byte[] msgToSend = new byte[msg.Length - 4];
            Array.Copy(msg, msgToSend, msg.Length - 4);
        }

        // returns the chatID the message belongs to
        public int getChatID()
        {
            return chatID;
        }

        // returns actual message
        public string getMessage()
        {
            return Encoding.ASCII.GetString(msg);
        }

        // returns bytes with chatID appended to the end
        public byte[] toBytes()
        {
            byte[] id = BitConverter.GetBytes(chatID);
            byte[] message = new byte[msg.Length + id.Length];

            msg.CopyTo(message, 0);
            id.CopyTo(message, msg.Length);

            return message;
        }
    }
}

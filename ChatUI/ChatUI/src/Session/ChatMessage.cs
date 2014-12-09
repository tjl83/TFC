using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSession
{
    /// <summary>
    /// This class is a container for a chatMessage and the chatID it belongs to.
    /// It can return a byte array containing both information before sending to the stream.
    /// It can also parse a byte array from that same stream to return both values.
    /// </summary>
    public class ChatMessage : EventArgs
    {
        private int chatID;
        private byte[] chatMsg;

        public ChatMessage(int id, byte[] msg)
        {
            chatID = id;
            chatMsg = msg;
        }

        // reads a raw message byte array containing the message and the chatID appended to the end
        public ChatMessage(byte[] msg)
        {
            chatID = BitConverter.ToInt32(msg, msg.Length - 4);
            chatMsg = new byte[msg.Length - 4];
            Array.Copy(msg, chatMsg, msg.Length - 4);
        }

        // returns the chatID the message belongs to
        public int getChatID()
        {
            return chatID;
        }

        // returns actual message
        public byte[] getChatMessage()
        {
            return chatMsg;
        }

        // returns bytes with chatID appended to the end
        public byte[] toBytes()
        {
            byte[] msg = new byte[chatMsg.Length + 4];

            Array.Copy(chatMsg, msg, chatMsg.Length);

            byte[] id = BitConverter.GetBytes(chatID);
            id.CopyTo(msg, chatMsg.Length - 1);

            return msg;
        }
    }
}

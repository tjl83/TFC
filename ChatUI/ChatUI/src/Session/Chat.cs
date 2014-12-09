using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatSession
{
    public class Chat
    {
        public int chatID { get; set; }
        private HashSet<TcpClient> chatMembers;

        public Chat()    // Modify Constructor to generate "random" chatID
        {
            chatID = 0;
            chatMembers = new HashSet<TcpClient>();
        }

        public void addMember(TcpClient newUser)
        {
            foreach (TcpClient member in chatMembers)
            {
                chatMembers.Add(newUser);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void message(byte[] message)
        {
            foreach (TcpClient member in chatMembers)
            {
                ChatMessage messg = new ChatMessage(chatID, message);
                NetworkModule.networkModule.message(member, msgType.Chat, messg.toBytes());
            }
        }
    }
}

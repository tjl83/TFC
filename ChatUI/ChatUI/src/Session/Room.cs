using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatSession
{
    abstract class Room
    {
        private static NetworkModule nModule = NetworkModule.networkModule;

        private int chatID;
        private HashSet<TcpClient> chatMembers;

        public Room()                                                                                                 // Modify Constructor to generate "random" chatID
        {
            chatID = 0;
            chatMembers = new HashSet<TcpClient>();
        }

        public void addMember(TcpClient newUser)
        {
            foreach(TcpClient member in chatMembers){

            }
            chatMembers.Add(newUser);
        }

        public int getID()
        {
            return chatID;
        }

        public void setID(int id)
        {
            chatID = id;
        }

        public HashSet<TcpClient> getMembers()
        {
            return chatMembers;
        }

        public void setMembers(HashSet<TcpClient> members)
        {
            chatMembers = members;
        }

        abstract public void message(byte[] msg);

    }

    public class ChatRoom : Room
    {

        /// <summary>
        /// For chat messages only
        /// </summary>
        /// <param name="message"></param>
        public void message(byte[] msg)
        {
            foreach (TcpClient member in getMembers())
            {
                ChatMessage messg = new ChatMessage(getID(), msg);
                NetworkModule.networkModule.message(member, msgType.Chat, messg.toBytes());
            }
        }
    }

    class InitRoom : Room
    {

        /// <summary>
        /// For verification only
        /// </summary>
        /// <param name="message"></param>
        public void message(byte[] message)
        {
            foreach (TcpClient member in getMembers())
            {
                ChatMessage messg = new ChatMessage(getID(), message);
                NetworkModule.networkModule.message(member, msgType.Chat, messg.toBytes());
            }
        }
    }
}

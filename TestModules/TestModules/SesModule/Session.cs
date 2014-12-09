using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestModules.SesModule
{
    public class Session
    {
        private NetworkModule nModule;

        private string username;

        private List<string> verifiedUsernames;
        private Dictionary<string, TcpClient> verifiedUsers;
        private HashSet<TcpClient> unverifiedUsers;

        public Session(string my_name, int port)
        {
            nModule = new NetworkModule(this, port);
            username = my_name;

            verifiedUsernames = new List<string>();
            verifiedUsers = new Dictionary<string, TcpClient>();
            unverifiedUsers = new HashSet<TcpClient>();
        }

        public List<string> getUsers()
        {
            return verifiedUsernames;
        }

        /// <summary>
        /// This is the function the UI calls to add a new user
        /// </summary>
        /// <param name="ip">The IP address the user is located at</param>
        public void findUser(String ip)
        {
            findUser(ip, 420);
        } 

        public void findUser(String ip, int port)
        {
            TcpClient newUser = nModule.findUser(ip);
            if (newUser != null)
            {
                IPAddress foo = ((IPEndPoint)newUser.Client.RemoteEndPoint).Address;
                ip = foo.ToString();
                Console.Out.WriteLine("Client at " + ip + " found.");
                unverifiedUsers.Add(newUser);
                beginVerify(newUser);
            }
            else
            {
                Console.Out.WriteLine("Client at " + ip + " not found.");
            }
        }

        /// <summary>
        /// This is the signaled function that the NetworkModule calls when it receives a new connection 
        /// (a new user initiates a connection to you).
        /// </summary>
        /// <param name="newUser"></param>
        public void signalNewUser(TcpClient newUser)
        {
            // TODO: Check to see if the user was already verified.
            unverifiedUsers.Add(newUser);
            beginVerify(newUser);
        }


        private void beginVerify(TcpClient client)
        {
            byte[] usernameBytes = Encoding.ASCII.GetBytes(username);
            nModule.message(client, msgType.Verification, usernameBytes);
        }

        /// <summary>
        /// This is the signaled function that the NetworkModule calls when it receives a message.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void signalNewMessage(TcpClient client, msgType type, byte[] msg)
        {
            //Start HANDLE MESSAGE
            IPAddress foo = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            String ip = foo.ToString();
            String message = Encoding.ASCII.GetString(msg);
            String messageType;
            switch (type)
            {
                case msgType.Chat:
                    messageType = "Chat";
                    break;
                case msgType.Internal:
                    messageType = "Chat";
                    break;
                case msgType.Verification:
                    messageType = "Chat";
                    break;
                default:
                    messageType = "";
                    break;
            }
            Console.Out.WriteLine(ip + ":" + "(" + messageType + ")" + message);
            //End HANDLEMESSAGE
        }
        private void receiveVerification(byte[] msg)
        {
            string message = Encoding.ASCII.GetString(msg);

            verifiedUsernames.Add(message);
        }

        private void checkInternal(Message message)
        {

        }

        private void processChat(Message message)
        {

        }

        public void close()
        {
            nModule.close();
        }
    }
}

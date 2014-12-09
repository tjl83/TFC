using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatUI.Backend
{
    public enum msgType { Verification, Internal, Chat }

    public class NetworkModule
    {
        public static NetworkModule networkModule;

        private static Session cSess = Session.currentSession;

        bool isOnline;
        int portnum = 420;

        TcpListener portListener;
        HashSet<TcpClient> connectedUsers;

        HashSet<IPAddress> ignoreList;

        public NetworkModule()
        {
            initialize();
        }

        public NetworkModule(int port)
        {
            portnum = port;
            initialize();
        }

        private void initialize()
        {
            if (networkModule == null)
            {
                networkModule = this;
            }
            else
            {
                throw new Exception("Attempting to create a second network module.");
            }

            cSess = Session.currentSession;

            isOnline = true;

#pragma warning disable 618     //ignores the deprecated TcpListener constructor warning
            portListener = new TcpListener(portnum);
#pragma warning restore 618

            connectedUsers = new HashSet<TcpClient>();
            ignoreList = new HashSet<IPAddress>();

            Thread portListenerThread = new Thread(() => listenForConnection(portListener));
            portListenerThread.Name = "Port Listener";
            portListenerThread.Start();
        }

        /// <summary>
        /// Is the thread listening on the default or otherwise specified socket for connections
        /// </summary>
        /// <param name="online">Value passed to alert when the thread should close.</param>
        /// <param name="pListener">TcpListener that waits for other clients to attempt to make a connection.</param>
        private void listenForConnection(TcpListener pListener)
        {

            pListener.Start();

            while (isOnline)
            {
                try {
                    TcpClient nextClient = pListener.AcceptTcpClient();
                    IPAddress nextClientIP = ((IPEndPoint)nextClient.Client.RemoteEndPoint).Address;
                    if (ignoreList.Contains(nextClientIP))
                    {
                        nextClient.Close();
                        continue;
                    }
                    addClient(nextClient);
                    cSess.signalNewUser(nextClient);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }

            pListener.Stop();
        }

        public TcpClient findUser(string ip)
        {
            return findUser(ip, portnum);
        }

        /// <summary>
        /// Attempts to connect to a user at specified ip address and port number
        /// </summary>
        /// <param name="ip">The IP address of the user</param>
        /// <param name="port">The port number the user's client is using</param>
        /// <returns>TcpClient of User at IP address/port specified if the connection was successful, otherwise null.</returns>
        public TcpClient findUser(string ip, int port)
        {
            TcpClient client = null;
            try {
                client = new TcpClient(ip, port);
                addClient(client);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return client;
        }

        private void addClient(TcpClient client)
        {
            connectedUsers.Add(client);
            Thread cThread = new Thread(() => clientListener(client));
            cThread.Name = "Client Listener" + DateTime.Now.ToLongTimeString();
            cThread.Start();
        }

        /// <summary>
        /// Listens to the network stream from specified socket and writes data to output stream
        /// </summary>
        /// <param name="online">The isOnline boolean needed to shutdown threads</param>
        /// <param name="clSocket">Socket of Client</param>
        private void clientListener(TcpClient clSocket)
        {
            NetworkStream networkStream;
            byte[] bytesReceived = new byte[clSocket.ReceiveBufferSize];

            while (isOnline && clSocket.Connected)
            {
                networkStream = clSocket.GetStream();
                networkStream.Read(bytesReceived, 0, clSocket.ReceiveBufferSize);

                //Find end of message (find first byte of size 0 in array)
                int msgSize = 1;
                foreach (byte byt in bytesReceived)
                {
                    if (byt == 0)
                    {
                        break;
                    }
                    msgSize++;
                }

                //Shorten message to end of message
                byte[] msgBytes = new byte[msgSize];
                Array.Copy(bytesReceived, msgBytes, msgSize);

                //Get message type off end of message
                msgType type = msgType.Internal;
                switch (msgBytes[msgSize - 1])
                {
                    case 1:
                        type = msgType.Verification;
                        break;
                    case 2:
                        type = msgType.Internal;
                        break;
                    case 3:
                        type = msgType.Chat;
                        break;
                    default:
                        break;
                }

                //Shorten message to actual message
                byte[] msg = new byte[msgSize - 1];
                Array.Copy(msgBytes, msg, msg.Length);

                cSess.signalNewMessage(clSocket, type, msg);
            }
        }

        /// <summary>
        /// Helper function to append a single byte to the end of an array of bytes
        /// </summary>
        /// <param name="array">The array of bytes for the byte to be appended to.</param>
        /// <param name="thing">The byte to be appended to the array of bytes.</param>
        /// <returns>Returns the new array with the specified byte appended to the end</returns>
        private byte[] appendToEnd(byte[] array, byte thing)
        {
            int bytesOfmsg = array.Length + 1;
            byte[] bytesToSend = new byte[bytesOfmsg];

            array.CopyTo(bytesToSend, 0);
            bytesToSend.SetValue(thing, array.Length);
            return bytesToSend;
        }

        /// <summary>
        /// Writes message to network stream of specified client's socket
        /// </summary>
        /// <param name="client">TcpClient in online list.</param>
        /// <param name="type">Type of Message to be sent</param>
        /// <param name="msg">Message to be sent</param>
        public void message(TcpClient client, msgType type, byte[] msg)
        {
            switch (type)
            {
                case msgType.Verification:
                    msg = appendToEnd(msg, 1);
                    break;
                case msgType.Internal:
                    msg = appendToEnd(msg, 2);
                    break;
                case msgType.Chat:
                    msg = appendToEnd(msg, 3);
                    break;
                default:
                    break;
            }

            Thread mThread = new Thread(() => messageThread(client, msg));
            mThread.Start();
        }

        private void messageThread(TcpClient client, byte[] msg)
        {
            msg = appendToEnd(msg, 0);

            NetworkStream networkStream = client.GetStream();
            networkStream.Write(msg, 0, msg.Length);
            networkStream.Flush();
        }

        public void ignore(TcpClient client)
        {
            ignoreList.Add(((IPEndPoint)client.Client.RemoteEndPoint).Address);
            connectedUsers.Remove(client);
            client.Close();
        }

        public void close()
        {
            isOnline = false;

            Thread.Sleep(1);

            portListener.Stop();
            foreach (TcpClient client in connectedUsers)
            {
                client.Close();
            }
        }
    }
}

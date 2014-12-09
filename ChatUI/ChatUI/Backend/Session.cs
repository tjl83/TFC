using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using OTR.Interface;

using ChatUI.Dialogue;

namespace ChatUI.Backend
{
    public static class InternalMsgType{
        public const string SignOff = "0";
        public const string BeginConvo = "1";
        public const string EndConvo = "2";
    }
    public class Session
    {
        private static NetworkModule nModule;
        private static ClientWindow cWindow;

        private string AlicesID;

        OTRSessionManager AliceSessionManager;

        public Dictionary<string, TcpClient> usersbyUsername;
        public Dictionary<TcpClient, string> usersbyTcpClient;

        public Session(ClientWindow window, string my_name, int port)
        {
            nModule = new NetworkModule(this, port);
            cWindow = window;
            AlicesID = my_name;
            AliceSessionManager = new OTRSessionManager(AlicesID);

            usersbyUsername = new Dictionary<string, TcpClient>();
            usersbyTcpClient = new Dictionary<TcpClient, string>();
        }

        /// <summary>
        /// This is the function the UI calls to add a new user
        /// </summary>
        /// <param name="ip">The IP address the user is located at</param>
        public void findUser(String ip, int port)
        {
            TcpClient newUser = nModule.findUser(ip, port);
            if (newUser != null)
            {
                sendUsername(newUser);
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
            sendUsername(newUser);
        }


        private void sendUsername(TcpClient client)
        {
            byte[] usernameBytes = Encoding.ASCII.GetBytes(AlicesID);
            nModule.message(client, msgType.Initial, usernameBytes);
        }

        public void beginConversation(String user)
        {
            sendMessage(user, msgType.Internal, InternalMsgType.BeginConvo);
            beginOTRSession(user);
        }

        public void beginOTRSession(String AlicesFriendID)
        {
            AliceSessionManager.OnOTREvent += new OTREventHandler(OnAliceOTRManagerEventHandler);

            AliceSessionManager.CreateOTRSession(AlicesFriendID, true);

            //Person with higher/lower IP is host.
            if (AlicesFriendID.CompareTo(AlicesID) < 0)
            {
                AliceSessionManager.RequestOTRSession(AlicesFriendID, OTRSessionManager.GetSupportedOTRVersionList()[0]);
            }
        }

        /// <summary>
        /// This is the signaled function that the NetworkModule calls when it receives a message.
        /// </summary>
        /// <param name="client">From which client the message is from</param>
        /// <param name="type">What type of message it is: Verification for initial messages, Internal for beginning a conversation, and Chat for chat-related messages</param>
        /// <param name="msg"></param>
        public void signalNewMessage(TcpClient client, msgType type, byte[] msg)
        {
            String message = Encoding.ASCII.GetString(msg);
            String user;
            usersbyTcpClient.TryGetValue(client, out user);

            switch (type)
            {
                case msgType.Initial:
                    receiveUsername(client, message);
                    break;
                case msgType.Internal:
                    checkInternal(user, message);
                    break;
                case msgType.Chat:
                    AliceSessionManager.ProcessOTRMessage(user, message);
                    break;
            }
        }

        /// <summary>
        /// This is the method called when a Verification message is received from another client.
        /// The first verification message received would be the username.
        /// </summary>
        /// <param name="client">From which client the message is from</param>
        /// <param name="msg">The verification message (username)</param>
        private void receiveUsername(TcpClient client, String AlicesFriendID)
        {
            usersbyUsername.Add(AlicesFriendID, client);
            usersbyTcpClient.Add(client, AlicesFriendID);

            cWindow.Dispatcher.Invoke(new Action(delegate()
            {
                cWindow.OnlineUsers.Items.Add(AlicesFriendID);
            }));
        }

        /// <summary>
        /// A "poke" to initiate a conversation which starts up a ChatWindow.
        /// </summary>
        /// <param name="client">From which client the message is from</param>
        private void checkInternal(String user, String InternalMsgTypeValue)
        {
            switch (InternalMsgTypeValue)
            {
                case InternalMsgType.SignOff:
                    
                    break;
                case InternalMsgType.BeginConvo:
                    cWindow.Dispatcher.Invoke(new Action(delegate()
                    {
                        cWindow.begin_Conversation(user);
                    }));
                    break;
                case InternalMsgType.EndConvo:
                    displayMessage(user, "has left chat.");
                    ChatWindow convo = null;
                    if (cWindow.chats.TryGetValue(user, out convo))
                    {
                        convo.Dispatcher.Invoke(new Action(delegate()
                        {
                            convo.textBoxEntryField.IsReadOnly = true;
                        }));
                    }
                    if (user.CompareTo(AlicesID) < 0)
                    {
                        AliceSessionManager.CloseSession(user);
                    }
                    break;
                default:
                    break;
            }
                
        }

        private void userSignOff(String user){
            if (user != null)
            {
                ChatWindow chat = null;
                if (cWindow.chats.TryGetValue(user, out chat))
                {
                    chat.Dispatcher.Invoke(new Action(delegate()
                    {
                        chat.Close();
                    }));
                }
                cWindow.Dispatcher.Invoke(new Action(delegate()
                {
                    cWindow.OnlineUsers.Items.Remove(user);
                }));
                if (user.CompareTo(AlicesID) < 0)
                {
                    AliceSessionManager.CloseSession(user);
                }
                TcpClient client = null;
                if (usersbyUsername.TryGetValue(user, out client))
                {
                    usersbyTcpClient.Remove(client);
                    usersbyUsername.Remove(user);
                }
            }
            else
            {

            }
        }

        private void displayMessage(String AlicesFriendID, String message)
        {
            ChatWindow chat = null;
            if (cWindow.chats.TryGetValue(AlicesFriendID, out chat))
            {
                chat.Dispatcher.Invoke(new Action(delegate()
                {
                    chat.DisplayReceivedMessage(message);
                }));
            }
        }

        /// <summary>
        /// This method is for sending chat-related messages from the UI
        /// </summary>
        /// <param name="user">Which user this message is to be sent to</param>
        /// <param name="message">The message to be sent</param>
        public void sendChatMessage(String user, String message)
        {
            AliceSessionManager.EncryptMessage(user, message);
        }

        private void sendMessage(String user, msgType type, String message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);

            TcpClient client = null;
            if (usersbyUsername.TryGetValue(user, out client))
            {
                nModule.message(client, type, msg);
            }
        }

        public void closeChat(String user)
        {
            sendMessage(user, msgType.Internal, InternalMsgType.EndConvo);
            if (user.CompareTo(AlicesID) < 0)
            {
                AliceSessionManager.CloseSession(user);
            }
        }

        /// <summary>
        /// This should close all background threads.
        /// </summary>
        public void close()
        {
            foreach(String user in usersbyUsername.Keys){
                sendMessage(user, msgType.Internal, InternalMsgType.SignOff);
            }

            Thread.Sleep(1000);

            nModule.close();
        }


        #region OTRAdditions

        //TODO: This manager needs to be updated to use the network traffic
        private void OnAliceOTRManagerEventHandler(object source, OTREventArgs e)
        {

            switch (e.GetOTREvent())
            {
                case OTR_EVENT.MESSAGE:
                    //This event happens when a message is decrypted successfully
                    //Console.WriteLine("{0}: {1} \n", e.GetSessionID(), e.GetMessage());
                    displayMessage(e.GetSessionID(), e.GetMessage());
                    break;
                case OTR_EVENT.SEND:
                    //This is where you would send the data on the network. Next line is just a dummy line. e.GetMessage() will contain message to be sent
                    sendMessage(e.GetSessionID(), msgType.Chat, e.GetMessage());
                    break;
                case OTR_EVENT.ERROR:
                    //Some sort of error occurred. We should use these errors to decide if it is fatal (failure to verify key) or benign (message did not decrypt)
                    Console.Error.WriteLine("Alice: OTR Error: {0} \n", e.GetErrorMessage());
                    Console.Error.WriteLine("Alice: OTR Error Verbose: {0} \n", e.GetErrorVerbose());
                    break;
                case OTR_EVENT.READY:
                    //Fires when each user is ready for communication. Can't communicate prior to this.
                    Console.Out.WriteLine("TFC_SYSTEM_MESSAGE: Encrypted OTR session with {0} established \n", e.GetSessionID());
                    AliceSessionManager.EncryptMessage(e.GetSessionID(), "If you can read this, encryption is successful.\n");
                    break;
                case OTR_EVENT.DEBUG:
                    //Just for debug lines. Flagged using a true flag in the session manager construction
                    //cout.WriteLine("DEBUG: " + e.GetMessage() + "\n");
                    //cout.Flush();
                    break;
                case OTR_EVENT.EXTRA_KEY_REQUEST:
                    //Allow for symmetric AES key usage. Only for OTR v3+.
                    //I doubt we need this.
                    break;
                case OTR_EVENT.SMP_MESSAGE:
                    //Fires after SMP process finishes
                    Console.Out.WriteLine("Authentication Notice: " + e.GetMessage() + "\n");
                    break;
                case OTR_EVENT.CLOSED:
                    //Fires when OTR session closes
                    Console.Out.WriteLine("TFC_SYSTEM_MESSAGE: Encrypted OTR session with {0} closed \n", e.GetSessionID());
                    break;
            }
        }

        #endregion
    }
}

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
    public class Session
    {
        private static NetworkModule nModule;
        private static ClientWindow cWindow;

        private string username;

        private OTRSessionManager otrManager;

        public Dictionary<string, TcpClient> verifiedUsersNames;
        public Dictionary<TcpClient, string> verifiedUsersClients;
        private HashSet<TcpClient> unverifiedUsers;

        public Session(ClientWindow window, string my_name, int port)
        {
            nModule = new NetworkModule(this, port);
            cWindow = window;
            username = my_name;
            otrManager = new OTRSessionManager(username);

            verifiedUsersNames = new Dictionary<string, TcpClient>();
            verifiedUsersClients = new Dictionary<TcpClient, string>();
            unverifiedUsers = new HashSet<TcpClient>();
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
                unverifiedUsers.Add(newUser);
                beginVerify(newUser);
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

        public void beginConversation(String user)
        {
            TcpClient client = null;
            if (verifiedUsersNames.TryGetValue(user, out client))
            {
                nModule.message(client, msgType.Internal, new byte[]{});
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
            switch (type)
            {
                case msgType.Verification:
                    receiveVerification(client, msg);
                    break;
                case msgType.Internal:
                    checkInternal(client);
                    break;
                case msgType.Chat:
                    processChat(client, msg);
                    break;
            }
        }

        /// <summary>
        /// This is the method called when a Verification message is received from another client.
        /// The first verification message received would be the username.
        /// </summary>
        /// <param name="client">From which client the message is from</param>
        /// <param name="msg">The verification message (username)</param>
        private void receiveVerification(TcpClient client, byte[] msg)
        {
            string user = Encoding.ASCII.GetString(msg);

                                                                                            //Username is received here, begin KeySwapping

            verifiedUsersNames.Add(user, client);
            verifiedUsersClients.Add(client, user);

            cWindow.Dispatcher.Invoke(new Action(delegate()
            {
                cWindow.OnlineUsers.Items.Add(user);
            }));

            unverifiedUsers.Remove(client);
        }

        /// <summary>
        /// A "poke" to initiate a conversation which starts up a ChatWindow.
        /// </summary>
        /// <param name="client">From which client the message is from</param>
        private void checkInternal(TcpClient client)
        {
            String user = null;
            if(verifiedUsersClients.TryGetValue(client, out user)){
                cWindow.Dispatcher.Invoke(new Action(delegate()
                {
                    cWindow.begin_Conversation(user);
                }));
            }
        }

        /// <summary>
        /// Chat messages received.
        /// </summary>
        /// <param name="client">From which client the message is from</param>
        /// <param name="msg">The chat message to be displayed</param>
        private void processChat(TcpClient client, byte[] msg)
        {
            String message = Encoding.ASCII.GetString(msg);

            String username;
            if(verifiedUsersClients.TryGetValue(client, out username)){
                ChatWindow chat = null;
                if (cWindow.chats.TryGetValue(username, out chat))
                {
                    chat.Dispatcher.Invoke(new Action(delegate()
                    {
                        chat.DisplayReceivedMessage(message);
                    }));
                }
            }
        }

        /// <summary>
        /// This method is for sending chat-related messages from the UI
        /// </summary>
        /// <param name="user">Which user this message is to be sent to</param>
        /// <param name="message">The message to be sent</param>
        public void sendChatMessage(String user, String message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);

            TcpClient client = null;
            if (verifiedUsersNames.TryGetValue(user, out client))
            {
                nModule.message(client, msgType.Chat, msg);
            }
        }

        /// <summary>
        /// This should close all background threads.
        /// </summary>
        public void close()
        {
            nModule.close();
        }

        #region OTRAdditions

        public void encryptFromGUI(String AlicesFriendID, String input)
        {
            otrManager.EncryptMessage(AlicesFriendID, input);
        }

        public void openOTRSession(String buddy_ID)
        {
            string _my_buddy_unique_id = buddy_ID; //Something like "Bob"


            /* Create OTR session and Request OTR session */
            //AliceSessionManager = new OTRSessionManager(_my_unique_id);

            otrManager.OnOTREvent += new OTREventHandler(OnAliceOTRManagerEventHandler);

            //AliceSessionManager.CreateOTRSession(_my_buddy_unique_id, true);
            otrManager.CreateOTRSession(_my_buddy_unique_id);

            otrManager.RequestOTRSession(_my_buddy_unique_id, OTRSessionManager.GetSupportedOTRVersionList()[0]);


        }

        //TODO: This manager needs to be updated to use the network traffic
        private void OnAliceOTRManagerEventHandler(object source, OTREventArgs e)
        {
/*            switch (e.GetOTREvent())
            {
                case OTR_EVENT.MESSAGE:
                    //This event happens when a message is decrypted successfully
                    //Console.WriteLine("{0}: {1} \n", e.GetSessionID(), e.GetMessage());
                    cout.Write("Client-" + e.GetSessionID() + ":\t" + e.GetMessage());
                    cout.Flush();
                    break;
                case OTR_EVENT.SEND:
                    //This is where you would send the data on the network. Next line is just a dummy line. e.GetMessage() will contain message to be sent
                    message(0, e.GetMessage());
                    break;
                case OTR_EVENT.ERROR:
                    //Some sort of error occurred. We should use these errors to decide if it is fatal (failure to verify key) or benign (message did not decrypt)
                    cout.WriteLine("Alice: OTR Error: {0} \n", e.GetErrorMessage());
                    cout.WriteLine("Alice: OTR Error Verbose: {0} \n", e.GetErrorVerbose());
                    cout.Flush();
                    break;
                case OTR_EVENT.READY:
                    //Fires when each user is ready for communication. Can't communicate prior to this.
                    cout.WriteLine("TFC_SYSTEM_MESSAGE: Encrypted OTR session with {0} established \n", e.GetSessionID());
                    //cout.Flush();
                    otrManager.EncryptMessage(AlicesFriendID, "If you can read this, encryption is successful.");
                    cout.Flush();
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
                    cout.WriteLine("Authentication Notice: " + e.GetMessage() + "\n");
                    cout.Flush();
                    break;
                case OTR_EVENT.CLOSED:
                    //Fires when OTR session closes
                    cout.WriteLine("TFC_SYSTEM_MESSAGE: Encrypted OTR session with {0} closed \n", e.GetSessionID());
                    cout.Flush();
                    break;
            }*/
        }
        #endregion
    }
}

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
        public static Session currentSession;

        private static NetworkModule nModule = NetworkModule.networkModule;
        private static ClientWindow cWindow;

        private string username;

        private OTRSessionManager otrManager;

        private List<string> verifiedUsernames;
        private Dictionary<string, TcpClient> verifiedUsers;
        private HashSet<TcpClient> unverifiedUsers;

        public Session(ClientWindow window, string my_name, int port)
        {
            if (currentSession == null)
            {
                currentSession = this;
            }
            else
            {
                throw new Exception("Attempting to create a second session.");
            }

            nModule = new NetworkModule(port);
            cWindow = window;
            username = my_name;
            otrManager = new OTRSessionManager(username);

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
            TcpClient newUser = nModule.findUser(ip);
            unverifiedUsers.Add(newUser);
            beginVerify(newUser);
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
            switch (type)
            {
                case msgType.Verification:
                    receiveVerification(msg);
                    break;
                case msgType.Internal:
                    Message message = new Message(msg);
                    checkInternal(message);
                    break;
                case msgType.Chat:
                    break;
            }
        }
        private void receiveVerification(byte[] msg)
        {
            string message = Encoding.ASCII.GetString(msg);
            UserFoundDialogue dialogue = new UserFoundDialogue(message);
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

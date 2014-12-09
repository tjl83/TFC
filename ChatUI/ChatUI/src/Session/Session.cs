using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace ChatSession
{
    // A delegate type for hooking up change notifications
    public delegate void MessageSentHandler(object sender, EventArgs e);

    public class Session
    {
        public static Session currentSession;

        private static NetworkModule nModule;

        private HashSet<TcpClient> verifiedUsers;
        private HashSet<TcpClient> unverifiedUsers;

        private Dictionary<int, Chat> verificationProcesses;
        public Dictionary<int, Chat> chats;

        // An event 
        public event MessageSentHandler messageSent;

        public Session()
        {
            if (currentSession == null)
            {
                currentSession = this;
            }
            else
            {
                throw new Exception("Attempting to create a second session.");
            }

            nModule = new NetworkModule();

            verifiedUsers = new HashSet<TcpClient>();
            unverifiedUsers = new HashSet<TcpClient>();

            verificationProcesses = new Dictionary<int, Chat>();
            chats = new Dictionary<int, Chat>();
        }

        /// <summary>
        /// This is the function the UI calls to add a new user
        /// </summary>
        /// <param name="ip">The IP address the user is located at</param>
        public void addUser(String ip)
        {
            TcpClient man = nModule.findUser(ip);
            unverifiedUsers.Add(man);

            Chat verificationProcess = new Chat();
            verificationProcesses.Add(0, verificationProcess);

            verificationProcess.message(new byte[]{});                                                             
            // Frank some validation process begins here.
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
        }

                                                                                                                    
        // FRAAAAAAANK FIIIITTZZZZZ Messages are processed here (all types in switch-case)

        /// <summary>
        /// This is the signaled function that the NetworkModule calls when it receives a message.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public void signalNewMessage(msgType type, byte[] msg)
        {
            ChatMessage message = new ChatMessage(msg);
            Chat chat = null;
            if (chats.TryGetValue(message.getChatID(), out chat))
            {

            }
            switch (type)
            {
                case msgType.Verification:

                    break;
                case msgType.Internal:

                    break;
                case msgType.Chat:                                                                                   
                    // FITZ Here's to you kid. Should signal some function in UI to output message.
                    messageSent(this, message);
                    break;
            }
        }
    }


    //#region OTRAdditions

    //public void openOTRSession(String native_user_ID, String buddy_ID)
    //{
    //    /* Declare OTR variables*/
    //    OTRSessionManager _alice_otr_session_manager = null;

    //    string _my_unique_id = native_user_ID; //Something like "Alice"
    //    string _my_buddy_unique_id = buddy_ID; //Something like "Bob"


    //    /* Create OTR session and Request OTR session */
    //    _alice_otr_session_manager = new OTRSessionManager(_my_unique_id);

    //    _alice_otr_session_manager.OnOTREvent += new OTREventHandler(OnAliceOTRMangerEventHandler);

    //    _alice_otr_session_manager.CreateOTRSession(_my_buddy_unique_id);

    //    _alice_otr_session_manager.RequestOTRSession(_my_buddy_unique_id, OTRSessionManager.GetSupportedOTRVersionList()[0]);


    //}

    ////TODO: This manager needs to be updated to use the network traffic
    //private void OnAliceOTRManagerEventHandler(object source, OTREventArgs e)
    //{

    //    switch (e.GetOTREvent())
    //    {
    //        case OTR_EVENT.MESSAGE:
    //            //This event happens when a message is decrypted successfully
    //            Console.WriteLine("{0}: {1} \n", e.GetSessionID(), e.GetMessage());
    //            break;
    //        case OTR_EVENT.SEND:
    //            //This is where you would send the data on the network. Next line is just a dummy line. e.GetMessage() will contain message to be sent
    //            SendDataOnNetwork(AliceID, e.GetMessage());
    //            break;
    //        case OTR_EVENT.ERROR:
    //            //Some sort of error occurred. We should use these errors to decide if it is fatal (failure to verify key) or benign (message did not decrypt)
    //            Console.WriteLine("Alice: OTR Error: {0} \n", e.GetErrorMessage());
    //            Console.WriteLine("Alice: OTR Error Verbose: {0} \n", e.GetErrorVerbose());
    //            break;
    //        case OTR_EVENT.READY:
    //            //Fires when each user is ready for communication. Can't communicate prior to this.
    //            Console.WriteLine("Alice: Encrypted OTR session with {0} established \n", e.GetSessionID());
    //            _alice_otr_session_manager.EncryptMessage(AlicesFriendID, "HI FIRST MESSAGE");
    //            break;
    //        case OTR_EVENT.DEBUG:
    //            //Just for debug lines. Flagged using a true flag in the session manager construction
    //            Console.WriteLine("Alice: " + e.GetMessage() + "\n");
    //            break;
    //        case OTR_EVENT.EXTRA_KEY_REQUEST:
    //            //Allow for symmetric AES key usage. Only for OTR v3+.
    //            //I doubt we need this.
    //            break;
    //        case OTR_EVENT.SMP_MESSAGE:
    //            //Fires after SMP process finishes
    //            Console.WriteLine("Alice: " + e.GetMessage() + "\n");
    //            break;
    //        case OTR_EVENT.CLOSED:
    //            //Fires when OTR session closes
    //            Console.WriteLine("Alice: Encrypted OTR session with {0} closed \n", e.GetSessionID());
    //            break;
    //    }
    //}
    //#endregion
}

﻿using System;
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

        private string AlicesID;

        private OTRSessionManager AliceSessionManager;

        public Dictionary<string, TcpClient> verifiedUsersNames;
        public Dictionary<TcpClient, string> verifiedUsersClients;
        private HashSet<TcpClient> unverifiedUsers;

        public Session(ClientWindow window, string my_name, int port)
        {
            nModule = new NetworkModule(this, port);
            cWindow = window;
            AlicesID = my_name;
            AliceSessionManager = new OTRSessionManager(AlicesID);

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
            byte[] usernameBytes = Encoding.ASCII.GetBytes(AlicesID);
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
            string AlicesFriendID = Encoding.ASCII.GetString(msg);

            AliceSessionManager.CreateOTRSession(AlicesFriendID, true);

            //Person with higher/lower IP is host.
            if (AlicesFriendID.CompareTo(AlicesID) < 0)
            {
                AliceSessionManager.RequestOTRSession(AlicesFriendID, OTRSessionManager.GetSupportedOTRVersionList()[0]);
            }

            verifiedUsersNames.Add(AlicesFriendID, client);
            verifiedUsersClients.Add(client, AlicesFriendID);

            cWindow.Dispatcher.Invoke(new Action(delegate()
            {
                cWindow.OnlineUsers.Items.Add(AlicesFriendID);
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


            String AlicesFriendID;
            if (verifiedUsersClients.TryGetValue(client, out AlicesFriendID))
            {
                AliceSessionManager.ProcessOTRMessage(AlicesFriendID, message);
                ChatWindow chat = null;
                if (cWindow.chats.TryGetValue(AlicesFriendID, out chat))
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
            AliceSessionManager.EncryptMessage(user, message);
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
    }
}

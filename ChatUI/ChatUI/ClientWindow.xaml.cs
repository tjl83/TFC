using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ChatUI.Backend;
using ChatUI.Dialogue;

namespace ChatUI
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private Session currentSession;
        public Dictionary<String, ChatWindow> chats;

        public ClientWindow(string username, short port)
        {
            InitializeComponent();

            currentSession = new Session(this, username, port);
            chats = new Dictionary<string, ChatWindow>();
        }

        private void Find_User(object sender, RoutedEventArgs args)
        {
            FindUserDialogue findUserDialogue = new FindUserDialogue(currentSession);
            if (findUserDialogue.ShowDialog() == true)
                return;
        }

        void OnlineUsers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnlineUsers.SelectedItem != null)
            {
                begin_Conversation(OnlineUsers.SelectedItem.ToString());
            }
        }

        void MenuItemStartConversation_Click(object sender, RoutedEventArgs args)
        {
            if (OnlineUsers.SelectedItem != null)
            {
                begin_Conversation(OnlineUsers.SelectedItem.ToString());
            }
        }

        public void begin_Conversation(String username)
        {
            if (!chats.ContainsKey(username))
            {
                ChatWindow chat = new ChatWindow(currentSession, username);
                chats.Add(username, chat);
                currentSession.beginConversation(username);
                chat.Show();
            }
            else
            {
                ChatWindow chat = null;
                if (chats.TryGetValue(username, out chat))
                {
                    chat.BringIntoView();
                }
            }
        }

        private void Exit(object sender, CancelEventArgs e)
        {
            foreach (ChatWindow chat in chats.Values)
            {
                chat.Close();
            }
            currentSession.close();
        }
    }
}

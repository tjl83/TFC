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
        public ClientWindow(string username, short port)
        {
            InitializeComponent();
            Session.currentSession = new Session(this, username, port);
            OnlineUsers.ItemsSource = Session.currentSession.getUsers();
        }

        private void Find_User(object sender, RoutedEventArgs args)
        {
            FindUserDialogue findUserDialogue = new FindUserDialogue();
            if (findUserDialogue.ShowDialog() == true)
                return;
        }

        void OnlineUsers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnlineUsers.SelectedItem != null)
            {
                MessageBox.Show(OnlineUsers.SelectedItem.ToString());
            }
        }

        private void MenuItemStartConversation_Click(object sender, RoutedEventArgs args)
        {

        }

        private void Exit(object sender, CancelEventArgs e)
        {
            Session.currentSession.close();
        }
    }
}

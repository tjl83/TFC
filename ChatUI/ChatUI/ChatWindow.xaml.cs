using System;
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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Threading;

using ChatUI.Backend;
using System.ComponentModel;

namespace ChatUI
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private Session cSession;
        private String username;
        private bool firstMsgRcv;

        public ChatWindow(Session currentSession, String user)
        {
            InitializeComponent();
            cSession = currentSession;
            username = user;
            firstMsgRcv = false;
            this.Title = "Chat with " + user;
        }

        private void DisplayMessage(string message)
        {
            textBoxChatPane.Text += (message);
            ScrollChat.ScrollToBottom();
        }

        public void DisplayReceivedMessage(string message)
        {
            if (!firstMsgRcv)
            {
                firstMsgRcv = true;
                sendBtn.IsEnabled = true;
                textBoxEntryField.IsEnabled = true;
            }
            DisplayMessage(username + ": " + message);
        }

        private void sendMessage()
        {
            DisplayMessage("You: " + textBoxEntryField.Text + Environment.NewLine);
            cSession.sendChatMessage(username, textBoxEntryField.Text + Environment.NewLine);
            textBoxEntryField.Clear();
        }

        private void textBoxEntryField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                sendMessage();
            }
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        public void Exit(object sender, CancelEventArgs e)
        {
            cSession.closeChat(username);
        }
    }
}

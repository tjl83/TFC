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

using ChatUI.Backend;

namespace ChatUI.Dialogue
{
    /// <summary>
    /// Interaction logic for FindUserDialogue.xaml
    /// </summary>
    public partial class FindUserDialogue : Window
    {
        public FindUserDialogue()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            int portNum = 420;
            try{
                portNum = Int32.Parse(port.Text);
            }
            catch(Exception err){
                Console.Error.WriteLine(err.Message);
            }
            Session.currentSession.findUser(ip.Text,portNum);
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ip.SelectAll();
            ip.Focus();
        }

        public void Exit(object sender, RoutedEventArgs args)
        {
            this.Close();
        }
    }
}

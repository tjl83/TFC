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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit;

namespace ChatUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Attempt_Login(object sender, RoutedEventArgs args)
        {
            string user = Username.Text;
            short port = Int16.Parse(Port.Text);
            ClientWindow client = new ClientWindow(user, port);
            this.Close();
            client.Show();
        }

        public void Exit(object sender, RoutedEventArgs args)
        {
            this.Close();
        }
    }
}

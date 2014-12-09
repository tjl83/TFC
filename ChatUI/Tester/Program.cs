using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChatUI;
using ChatUI.Backend;
using System.Net.Sockets;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            short portNum = Int16.Parse(Console.ReadLine());

            Session cSess = Session.currentSession;
            cSess = new Session(new ClientWindow("Jim", portNum),"Jim",portNum);
        }
    }
}

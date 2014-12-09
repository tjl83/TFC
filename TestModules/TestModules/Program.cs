using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using TestModules.NetModule;
using TestModules.SesModule;

namespace TestModules
{
    class Program
    {
        static void Main(string[] args)
        {
            testNetworkModule();
        }

        static void testNetworkModule()
        {
            TestModules.NetModule.NetworkModule nModule1 = new TestModules.NetModule.NetworkModule(420);
            TestModules.NetModule.NetworkModule nModule2 = new TestModules.NetModule.NetworkModule(421);

            System.Net.Sockets.TcpClient client2 = nModule1.findUser("127.0.0.1", 421);

            String message = "Hello C";
            byte[] msg = Encoding.ASCII.GetBytes(message);
            nModule1.message(client2, TestModules.NetModule.msgType.Chat, msg);

            message = "Hello I";
            msg = Encoding.ASCII.GetBytes(message);
            nModule1.message(client2, TestModules.NetModule.msgType.Internal, msg);

            message = "Hello V";
            msg = Encoding.ASCII.GetBytes(message);
            nModule1.message(client2, TestModules.NetModule.msgType.Verification, msg);
        }

        static void testSessionModule()
        {
            Session session1 = new Session("Alice", 420);
            Session session2 = new Session("Bob", 421);

            session1.findUser("127.0.0.1", 421);
        }
    }
}

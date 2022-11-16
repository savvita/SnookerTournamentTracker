//using SnookerTournamentTracker.ConnectionLibrary;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace SnookerTournamentTracker.Model
//{
//    internal class ClientModel
//    {
//        private TcpClient client;
//        public ClientModel()
//        {
//            client = new TcpClient(Connection.Host, Connection.Port);
//        }
//        public async Task SendMessage(Message msg)
//        {
//            client.Connect(Connection.Host, Connection.Port);
//            await Connection.SendMessageAsync(client.GetStream(), msg);
//            //client.Close();

//        }
//    }
//}

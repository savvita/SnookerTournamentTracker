using SnookerTournamentTracker.ConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer.Model
{
    internal class ServerModel : IDisposable
    {
        private TcpListener? listener;

        public ServerModel()
        {
            listener = new TcpListener(IPAddress.Any, Connection.Port);
            Task.Factory.StartNew(Listen);
        }

        private void Listen()
        {
            if(listener == null)
            {
                return;
            }

            listener.Start();

            do
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Factory.StartNew((obj) => HandleClient(obj), client);
            } while (true);
        }

        private Message? HandleClient(object? obj)
        {
            if (obj is TcpClient tcpClient)
            {
                try
                {
                    NetworkStream stream = tcpClient.GetStream();
                    Message? msg = Connection.ReceiveMessage(stream);

                    return msg;
                }
                catch (Exception ex)
                {
                    
                }
            }

            return null;
        }

        public void Dispose()
        {
            listener?.Stop();
        }
    }
}

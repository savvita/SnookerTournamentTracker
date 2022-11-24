using SnookerTournamentTracker.ConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTracker.Model
{
    internal class ClientModel
    {
        //TcpClient? client = null;
        //public void SendMessage(Message msg)
        //{
        //    if(client == null)
        //    {
        //        client = new TcpClient();
        //    }

        //    if(!client.Connected)
        //    {
        //        client.Connect(Connection.Host, Connection.Port);
        //    }
        //    Connection.SendMessage(client.GetStream(), msg);
            
        //    //client.Close();
        //}

        //public Message? ReceiveMessage()
        //{
        //    //cpClient client = new TcpClient();
        //    if (client == null)
        //    {
        //        client = new TcpClient();
        //    }

        //    if (!client.Connected)
        //    {
        //        client.Connect(Connection.Host, Connection.Port);
        //    }
        //    //Message? msg = await Connection.ReceiveMessageAsync(client.GetStream());
        //    Message? msg = Connection.ReceiveMessage(client.GetStream());
        //    //client.Close();

        //    return msg;
        //}

        public Message? GetResponse(Message request)
        {
            TcpClient client = new TcpClient();
            client.Connect(Connection.Host, Connection.Port);
            Connection.SendMessage(client.GetStream(), request);

            Message? response = Connection.ReceiveMessage(client.GetStream());
            client.Close();

            return response;
        }

        //public void Close()
        //{
        //    if(client != null && client.Connected)
        //    {
        //        client.Close();
        //        client = null;
        //    }
        //}
    }
}

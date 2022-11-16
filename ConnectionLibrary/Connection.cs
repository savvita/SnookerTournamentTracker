using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnookerTournamentTracker.ConnectionLibrary
{
    public static class Connection
    {
        /// <summary>
        /// Port to connect
        /// </summary>
        public static int Port { get; } = 8008;

        /// <summary>
        /// IP of the server
        /// </summary>
        public static string Host { get; } = "127.0.0.1";

        /// <summary>
        /// Default encoding of messages
        /// </summary>
        public static Encoding MessageEncoding { get; } = Encoding.Unicode;

        public static Message? ReceiveMessage(NetworkStream? stream)
        {

            if (stream == null)
            {
                throw new NullReferenceException("Stream cannot be null");
            }

            BinaryReader reader = new BinaryReader(stream);

            //int length = reader.ReadInt32();

            //byte[] data = new byte[length];
            //reader.Read(data, 0, length);

            //string msg = MessageEncoding.GetString(data, 0, length);

            //return JsonSerializer.Deserialize<Message>(msg);
            return null;
        }

        public static async Task<Message?> ReceiveMessageAsync(NetworkStream? stream)
        {

            if (stream == null)
            {
                throw new NullReferenceException("Stream cannot be null");
            }

            //BinaryReader reader = new BinaryReader(stream);

            //int length = reader.ReadInt32();

            //byte[] data = new byte[length];
            //reader.Read(data, 0, length);

            //string msg = MessageEncoding.GetString(data, 0, length);

            return await JsonSerializer.DeserializeAsync<Message>(stream);
        }

        public static void SendMessage(NetworkStream? stream, Message message)
        {
            if (stream == null)
            {
                return;
            }

            string msg = JsonSerializer.Serialize<Message>(message);

            byte[] data = MessageEncoding.GetBytes(msg);

            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(data.Length);
            writer.Write(data);
        }

        public static async Task SendMessageAsync(NetworkStream? stream, Message message)
        {
            if (stream == null)
            {
                return;
            }

            await JsonSerializer.SerializeAsync<Message>(stream, message);

            //byte[] data = MessageEncoding.GetBytes(msg);

            //BinaryWriter writer = new BinaryWriter(stream);
            //writer.Write(data.Length);
            //writer.Write(data);
        }

        public static Message? f()
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            TcpClient tcpClient = listener.AcceptTcpClient();

            return ReceiveMessage(tcpClient.GetStream());
        }

        public static void f2()
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(Host, Port);
            SendMessage(tcpClient.GetStream(), new Message() { Code = 2, Content = "Some text" });
        }
    }
}

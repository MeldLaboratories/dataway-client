using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dataway_Worker
{
    internal class DWSocket
    {
        private Socket socket;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public delegate void DataRecievedEvent(object sender, byte[] buffer, int bytes);

        public delegate void ErrorEvent(object sender, Exception e);

        public delegate void SocketErrorEvent(object sender, SocketException e);

        public event DataRecievedEvent OnDataRecieved;

        public event SocketErrorEvent OnSocketError;

        public event ErrorEvent OnError;

        public DWSocket(IPAddress addr, int port)
        {
            IPEndPoint endpoint = new IPEndPoint(addr, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Attempting connection to: {0}", endpoint.ToString());
            socket.Connect(endpoint); //TODO:Handle connect error
            Console.WriteLine("Connection successful.");
            this.startDataListener();
        }

        public void SendFile(string path)
        {
            socket.SendFile(path);//TODO: error handling for each send socket close
        }

        public void SendJson(object json)
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(json));
            socket.Send(bytes, bytes.Length, 0);
        }

#pragma warning disable CS1998

        private void startDataListener()
        {
            Task.Factory.StartNew(async () =>
            {
                int bytes = 0;

                while (cts.IsCancellationRequested == false)
                {
                    Byte[] buffer = new Byte[256]; //TODO: larger files

                    bytes = socket.Receive(buffer, buffer.Length, 0);
                    string data = Encoding.ASCII.GetString(buffer, 0, bytes);

                    OnDataRecieved?.Invoke(this, buffer, bytes);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

#pragma warning restore CS1998
    }
}
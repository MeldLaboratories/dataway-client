using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dataway_Worker
{
    internal class TSocket
    {
        private Socket socket;

        private CancellationTokenSource cts = new CancellationTokenSource();


        public delegate void DataRecievedEvent(object sender, byte[] buffer, int bytes);

        public event DataRecievedEvent OnDataRecieved;


        public IPEndPoint EndPoint;
        public bool Connected = false;


        public enum CONN_RESULT 
        {
            CONNECTION_SUCCESSFUL = 0,
            CONNECTION_REFUSED = 1,
            CONNECTION_CLOSED = 2 //TODO: make other result types
        }


        public TSocket(IPAddress addr, int port)
        {
            EndPoint = new IPEndPoint(addr, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        public CONN_RESULT Connect()
        {
            CONN_RESULT result = CONN_RESULT.CONNECTION_SUCCESSFUL;

            Task.Run(async () =>
            {
                try
                {
                    socket.Connect(EndPoint);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        //OnError?.Invoke(this, (int)CONN_RESULT.CONNECTION_REFUSED);
                        result = CONN_RESULT.CONNECTION_REFUSED;
                    }
                    else
                    {
                        throw new Exception("Unknown exception during socket connection.");
                    }
                }
            }).Wait();


            //CONNECTION SUCCESSFUL
            if (result == CONN_RESULT.CONNECTION_SUCCESSFUL)
            {
                this.startDataListener();
                this.Connected = true;
            }

            return result;
        }

        public void SendFile(string path)
        {
            socket.SendFile(path);//TODO: error handling for each send socket close
        }

        public void SendJson(object json)
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(json));//TODO: error handling for each send socket close
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
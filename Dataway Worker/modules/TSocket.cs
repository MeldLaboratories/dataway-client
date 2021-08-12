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

        public enum SEND_RESULT
        {
            SEND_SUCCESSFUL = 0,
            SEND_SOCKETNOTCONNECTED = 1
        }

        public TSocket(IPAddress addr, int port)
        {
            EndPoint = new IPEndPoint(addr, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public CONN_RESULT Connect()
        {
            CONN_RESULT result = CONN_RESULT.CONNECTION_SUCCESSFUL;

            #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
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
            #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            //CONNECTION SUCCESSFUL
            if (result == CONN_RESULT.CONNECTION_SUCCESSFUL)
            {
                this.startDataListener();
                this.Connected = true;
            }

            return result;
        }


        public void Disconnect()
        {
            this.socket.Disconnect(true);
        }

        public SEND_RESULT SendFile(string path)
        {
            try
            {
                socket.SendFile(path);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.NotConnected)
                {
                    return SEND_RESULT.SEND_SOCKETNOTCONNECTED;
                }
                else DWHelper.ShowErrorBox(e.Message);
            }
            catch (Exception e)
            {
                DWHelper.ShowErrorBox(e.Message);
            }

            return SEND_RESULT.SEND_SUCCESSFUL;
        }

        public SEND_RESULT SendJson(object json)
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(json));
            try
            {
                socket.Send(bytes, bytes.Length, 0);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.NotConnected)
                {
                    return SEND_RESULT.SEND_SOCKETNOTCONNECTED;
                }
                else DWHelper.ShowErrorBox(e.Message);
            }
            catch (Exception e)
            {
                DWHelper.ShowErrorBox(e.Message);
            }

            return SEND_RESULT.SEND_SUCCESSFUL;
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

                    OnDataRecieved?.Invoke(this, buffer, bytes);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

#pragma warning restore CS1998
    }
}
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Dataway_Worker
{
    internal class Client
    {
        //
        // Auto Reset Events & their Variables
        //

        private AutoResetEvent connectEvent = new AutoResetEvent(false);
        private AutoResetEvent loginEvent = new AutoResetEvent(false);
        private AutoResetEvent registerEvent = new AutoResetEvent(false);
        private AutoResetEvent logoutEvent = new AutoResetEvent(false);
        private AutoResetEvent sendFileEvent = new AutoResetEvent(false);
        private AutoResetEvent transmitRequestEvent = new AutoResetEvent(false);

        private Dataway_Worker.Formats.Communication.Local.ConnectEventData connectEventData = new Dataway_Worker.Formats.Communication.Local.ConnectEventData();
        private Dataway_Worker.Formats.Communication.Local.TransmitRequestEventData loginEventData = new Dataway_Worker.Formats.Communication.Local.TransmitRequestEventData();
        private Dataway_Worker.Formats.Communication.Local.RegisterEventData registerEventData = new Dataway_Worker.Formats.Communication.Local.RegisterEventData();
        private Dataway_Worker.Formats.Communication.Local.LogoutEventData logoutEventData = new Dataway_Worker.Formats.Communication.Local.LogoutEventData();
        private Dataway_Worker.Formats.Communication.Local.SendFileEventData sendFileEventData = new Dataway_Worker.Formats.Communication.Local.SendFileEventData();
        private Dataway_Worker.Formats.Communication.Local.TransmitRequestEventData transmitRequestEventData = new Dataway_Worker.Formats.Communication.Local.TransmitRequestEventData();

        //
        // Events
        //

        public delegate void TransmitRequestRecievedEvent(object invoker, string sender, string message, string filename, int filesizeMB);
        public delegate void ErrorEvent(object invoker, Exception e);

        public event TransmitRequestRecievedEvent OnTransmitRequest;
        public event ErrorEvent OnError;

        //
        // Variables
        //

        private Dataway_Worker.Formats.Communication.Local.NextFileRecieveData nextFileRecieveData = new Dataway_Worker.Formats.Communication.Local.NextFileRecieveData();

        private TSocket socket;

        public Client()
        {
            this.nextFileRecieveData.filetype = "json";
        }

        #region Public Methods

        //
        // Public Methods
        //

        public Result Connect(IPAddress addr, int port)
        {
            socket = new TSocket(addr, port);
            TSocket.CONN_RESULT res = socket.Connect();

            if (res == TSocket.CONN_RESULT.CONNECTION_SUCCESSFUL)
            {
                socket.OnDataRecieved += this.DataParser;
                return new Result(Result.CODE.SUCCESS);
            }
            else if (res == TSocket.CONN_RESULT.CONNECTION_REFUSED)
            {
                return new Result(Result.CODE.CONNECTION_REFUSED);
            }
            else
            {
                throw new Exception("Unknown exception during socket connection.");
            }
        }

        public Result Login(string username, string password)
        {
            var json = new Dataway_Worker.Formats.Communication.Send.Login();
            json.username = username;
            json.password = password;
            this.socket.SendJson(json);

            loginEvent.WaitOne();
            return new Result((Result.CODE)loginEventData.resultCode);
        }

        public Result Register(string username, string password)
        {
            var json = new Dataway_Worker.Formats.Communication.Send.Register();
            json.username = username;
            json.password = password;
            this.socket.SendJson(json); //TODO: TIMEOUT

            registerEvent.WaitOne();
            return new Result(Result.CODE.SUCCESS);
        }

        public Result Logout(string username, string password)
        {
            var json = new Dataway_Worker.Formats.Communication.Send.Register();
            json.username = username;
            json.password = password;
            this.socket.SendJson(json);

            logoutEvent.WaitOne();
            return new Result(Result.CODE.SUCCESS);
        }

        public Result SendFile(string path, string filename, string reciever)
        {
            //Send request
            var json = new Dataway_Worker.Formats.Communication.Send.TransmitRequest();
            json.filename = filename;
            json.reciever = reciever;

            this.socket.SendJson(json);

            //Wait for response
            sendFileEvent.WaitOne();

            //ACt accordingly to response
            this.socket.SendFile(path);
            //TODO: pause and check with checksum
            return new Result(Result.CODE.SUCCESS);
        }

        public Result AcceptCurrentTransmitRequest()
        {
            Console.WriteLine("accepted");
            transmitRequestEventData.resultCode = (int)Result.CODE.SUCCESS;
            transmitRequestEvent.Set(); //TODO: what if event not waiting
            return new Result(Result.CODE.SUCCESS);
        }

        public Result DeclineCurrentTransmitRequest()
        {
            transmitRequestEventData.resultCode = (int)Result.CODE.DECLINED_TRANSMIT_REQUEST;
            transmitRequestEvent.Set(); //TODO: what if event not waiting
            return new Result(Result.CODE.SUCCESS);
        }

        #endregion Public Methods

        #region Server responses

        //
        // Server responses
        //

        private void ConnectResult(int code)
        {
            connectEventData.resultCode = code;
            connectEvent.Set();
        }

        private void LoginResult(int code)
        {
            loginEventData.resultCode = code;
            loginEvent.Set();
        }

        private void RegisterResult(int code)
        {
            registerEventData.resultCode = code;
            registerEvent.Set();
        }

        private void LogoutResult(int code)
        {
            logoutEventData.resultCode = code;
            logoutEvent.Set();
        }

        private void TransmitRequestResult(int code)
        {
            sendFileEventData.resultCode = code;
            sendFileEvent.Set();
        }

        #endregion Server responses

        private void TransmitRequest(Dataway_Worker.Formats.Communication.Recieve.TransmitRequest transmitRequest)
        {
            //NOTIFY USER AND GET RESPONSE
            OnTransmitRequest?.Invoke(this, transmitRequest.sender, transmitRequest.message, transmitRequest.filename, transmitRequest.filesizeMB);
            transmitRequestEvent.WaitOne();

            if (transmitRequestEventData.resultCode == (int)Result.CODE.SUCCESS)
            {
                //CASE ACCEPT
                this.nextFileRecieveData.filename = transmitRequest.filename;
                this.nextFileRecieveData.filetype = "other";
                this.nextFileRecieveData.sender = transmitRequest.sender;
            }

            var json = new Dataway_Worker.Formats.Communication.Send.TransmitRequestResult();
            json.reciever = transmitRequest.sender;
            json.result = transmitRequestEventData.resultCode;
            this.socket.SendJson(json);
        }

        public void ErrorEventHandler(object sender, Exception e)
        {
        }

        private void DataParser(object sender, byte[] buffer, int bytes)
        {
            //Incoming File
            if (this.nextFileRecieveData.filetype != "json")
            {
                //Open Save-File-Dialog
                DWHelper.SaveBytesViaDialog(buffer, this.nextFileRecieveData.filename);

                this.nextFileRecieveData.filetype = "json";
                this.nextFileRecieveData.filename = "_";
                return;
            }

            //Incoming Json Object
            else
            {
                string data = Encoding.ASCII.GetString(buffer, 0, bytes);
                var baseType = JsonConvert.DeserializeObject<Dataway_Worker.Formats.Communication.Recieve.BaseType>(data);

                Console.WriteLine(data);

                if (baseType.type == "result")
                {
                    var result = JsonConvert.DeserializeObject<Dataway_Worker.Formats.Communication.Recieve.Result>(data);

                    switch (result.origin)
                    {
                        case "connect":
                            this.ConnectResult(result.code);
                            break;

                        case "login":
                            this.LoginResult(result.code);
                            break;

                        case "register":
                            this.RegisterResult(result.code);
                            break;

                        case "logout":
                            this.LogoutResult(result.code);
                            break;

                        case "transmitRequest":
                            this.TransmitRequestResult(result.code);
                            break;

                        default:
                            Console.WriteLine("Warning: Server responded with unknown result type. This is most likely caused by a missed update.");
                            break;
                    }
                }
                else
                {
                    switch (baseType.type)
                    {
                        case "transmitRequest":
                            var transmitRequest = JsonConvert.DeserializeObject<Dataway_Worker.Formats.Communication.Recieve.TransmitRequest>(data);
                            this.TransmitRequest(transmitRequest);
                            break;

                        default:
                            Console.WriteLine("Warning: Server responded with unknown message type. This is most likely caused by a missed update.");
                            break;
                    }
                }
            }
        }
    }
}
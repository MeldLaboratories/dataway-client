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

        private AutoResetEvent loginEvent = new AutoResetEvent(false);
        private AutoResetEvent registerEvent = new AutoResetEvent(false);
        private AutoResetEvent logoutEvent = new AutoResetEvent(false);
        private AutoResetEvent sendFileEvent = new AutoResetEvent(false);

        private Dataway_Worker.Formats.Communication.Local.LoginEventData loginEventData = new Dataway_Worker.Formats.Communication.Local.LoginEventData();
        private Dataway_Worker.Formats.Communication.Local.RegisterEventData registerEventData = new Dataway_Worker.Formats.Communication.Local.RegisterEventData();
        private Dataway_Worker.Formats.Communication.Local.LogoutEventData logoutEventData = new Dataway_Worker.Formats.Communication.Local.LogoutEventData();
        private Dataway_Worker.Formats.Communication.Local.SendFileEventData sendFileEventData = new Dataway_Worker.Formats.Communication.Local.SendFileEventData();

        //
        // Variables
        //

        private Dataway_Worker.Formats.Communication.Local.NextFileRecieveData nextFileRecieveData = new Dataway_Worker.Formats.Communication.Local.NextFileRecieveData();

        private DWSocket socket;

        public Client(IPAddress addr, int port)
        {
            this.nextFileRecieveData.filetype = "json";

            socket = new DWSocket(addr, port);
            socket.OnDataRecieved += this.DataParser;

            /*
            while (true)
            {
                Console.Write("Filename: ");
                var fn = Console.ReadLine();
                Console.Write("Reciever: ");
                var reciever = Console.ReadLine();

                //this.socket.SendFile(@"C:\Users\tensoid\Desktop\testFile.txt", "KEKFILE.txt", reciever);
                var json = new Dataway_Worker.Formats.Communication.Send.TransmitRequest();
                json.filetype = "txt";
                json.filename = fn;
                json.reciever = reciever;

                this.socket.SendJson(json);

                this.nextFileSendData.path = Environment.CurrentDirectory + "\\test.txt";

                Utils.WriteLineColor("Sent TransmitRequest", ConsoleColor.Yellow);
            }
            */
        }

        #region Public Methods

        //
        // Public Methods
        //

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
            this.socket.SendJson(json);

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
            var json = new Dataway_Worker.Formats.Communication.Send.TransmitRequest();
            json.filename = filename;
            json.reciever = reciever;

            this.socket.SendJson(json);

            this.sendFileEventData.path = Environment.CurrentDirectory + "\\test.txt";

            sendFileEvent.WaitOne();

            this.socket.SendFile(this.sendFileEventData.path);
            //pause and check with checksum
            return new Result(Result.CODE.SUCCESS);
        }

        #endregion Public Methods

        #region Server responses

        //
        // Server responses
        //

        private void LoginResult(int code)
        {
            this.loginEventData.resultCode = code;
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
            this.socket.SendFile(this.sendFileEventData.path);
        }

        #endregion Server responses

        private void TransmitRequest(Dataway_Worker.Formats.Communication.Recieve.TransmitRequest transmitRequest)
        {
            //TODO: accept or decline?

            //CASE ACCEPT
            this.nextFileRecieveData.filename = transmitRequest.filename;
            this.nextFileRecieveData.filetype = "other";
            this.nextFileRecieveData.sender = transmitRequest.sender;

            var json = new Dataway_Worker.Formats.Communication.Send.TransmitRequestResult();
            json.reciever = transmitRequest.sender;
            json.result = (int)Result.CODE.SUCCESS;
            this.socket.SendJson(json);
        }

        private void Error(Dataway_Worker.Formats.Communication.Recieve.Error error)
        {
            throw new Exception("Unknown Error: " + error.code);
        }

        private void DataParser(object sender, byte[] buffer, int bytes)
        {
            //Incoming File
            if (this.nextFileRecieveData.filetype != "json")
            {
                Console.WriteLine("Writing File {0} to {1}", this.nextFileRecieveData.filename, Environment.CurrentDirectory);

                File.WriteAllBytes(Environment.CurrentDirectory + "\\" + this.nextFileRecieveData.filename, buffer);
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

                        case "Error":
                            var error = JsonConvert.DeserializeObject<Dataway_Worker.Formats.Communication.Recieve.Error>(data);
                            this.Error(error);
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
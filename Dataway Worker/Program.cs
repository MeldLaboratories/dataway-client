using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;
using System;
using System.Diagnostics;
using System.Net;

namespace Dataway_Worker
{
    internal class Program
    {

        public static Client client = new Client();

        private static void Main(string[] args)
        {
            Console.WriteLine("> Dataway Worker <");

            Console.Write("Pipename: ");
            string pipeName = Console.ReadLine(); // DEBUG

            // start pipe server
            var server = new SimpleNamedPipeServer(pipeName);
            //var server = new SimpleNamedPipeServer("Dataway");
            server.Start();

            // start dataway client
            var res = client.Connect(new IPAddress(new Byte[] { 127, 0, 0, 1 }), 3003);
            Console.WriteLine("Connection to server: " + res.message);
            // TODO: check if result is a success
            client.OnTransmitRequest += HandleTransmitRequest;

            // TODO: add error handling
            // TODO: add ip switch

            server.ClientConnected += delegate (SimpleNamedPipeServer _)
            {
                Console.WriteLine("User connected");

                // handle data
                while (server.IsConnected)
                {
                    var msg = server.WaitForResponse(1000);
                    if (msg == "" || msg == null) continue;

                    var rawData = JsonConvert.DeserializeObject<Formats.Base>(msg);

                    Console.WriteLine(msg);

                    // handle send objects
                    if (rawData.Action.ToUpper() == "SEND")
                    {
                        switch (rawData.Type.ToUpper())
                        {
                            case "REQUEST":
                                try
                                {
                                    Actions.Send.Request(server, client, JsonConvert.DeserializeObject<Formats.Send.Command>(msg));
                                }
                                catch (Exception err)
                                {
                                    server.PushMessage(JsonConvert.SerializeObject(CreateError(err)));
                                    continue;
                                }

                                // return success
                                server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
                                break;

                            default:
                                break;
                        }
                    }

                    // handle login objects
                    if (rawData.Action.ToUpper() == "LOGIN")
                    {
                        switch (rawData.Type.ToUpper())
                        {
                            case "REQUEST":
                                try
                                {
                                    Actions.Login.PerformLogin(server, client, JsonConvert.DeserializeObject<Formats.Login.Command>(msg));
                                }
                                catch (Exception err)
                                {
                                    server.PushMessage(JsonConvert.SerializeObject(CreateError(err)));
                                    continue;
                                }

                                // return success
                                server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
                                break;

                            default:
                                break;
                        }
                    }

                    // handle login objects
                    if (rawData.Action.ToUpper() == "REGISTER")
                    {
                        switch (rawData.Type.ToUpper())
                        {
                            case "REQUEST":
                                try
                                {
                                    Actions.Register.PerformRegister(server, client, JsonConvert.DeserializeObject<Formats.Register.Command>(msg));
                                }
                                catch (Exception err)
                                {
                                    server.PushMessage(JsonConvert.SerializeObject(CreateError(err)));
                                    continue;
                                }

                                // return success
                                server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
                                break;

                            default:
                                break;
                        }
                    }

                    // handle generic data
                    if (rawData.Action.ToUpper() == "GENERIC")
                    {
                        switch (rawData.Type.ToUpper())
                        {
                            case "MESSAGE":
                                break;

                            case "ERROR":
                                break;

                            case "COMPLETE":
                                break;

                            default:
                                break;
                        }
                    }
                }
            };

            Process.GetCurrentProcess().WaitForExit();
        }

        private static Formats.Generic.Error CreateError(Exception err)
        {
            var response = new Formats.Generic.Error();
            response.Code = err.GetHashCode();
            response.Text = err.Message;
            return response;
        }


        private static void HandleTransmitRequest(object invoker, string sender, string message, string filename, int filesizeMB)
        {
            Console.WriteLine("Incoming transmit request from {0} with file {1}({2}MB) with message {3}", sender, filename, filesizeMB, message);
            Console.Write("Accept? y/n  ");
            string res = Console.ReadLine();

            if(res == "y" || res == "Y")
            {
                client.AcceptCurrentTransmitRequest(); //TODO: add multiple requests at same time or block other incoming requests
            }

            else client.DeclineCurrentTransmitRequest();
        }
    }
}
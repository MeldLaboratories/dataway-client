using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Dataway_Worker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // set a custom pipe server name by commandline arguments, else fallback to "Dataway"
            var pipename = "Dataway";
            if (GetValueOfSwitch("--pipe") != "") pipename = GetValueOfSwitch("--pipe");
            if (GetValueOfSwitch("-p") != "") pipename = GetValueOfSwitch("--pipe");
            if (pipename != "Dataway") Console.WriteLine("Custom Pipe-Server Name: {0}", pipename);

            // start pipe server
            var server = new SimpleNamedPipeServer(pipename);
            server.Start();

            // start dataway client
            var client = new Client();
            var res = client.Connect(new IPAddress(new Byte[] { 127, 0, 0, 1 }), 3003);
            Console.WriteLine("Connection to server: " + res.message);
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

            // Wait for current process to end. Kappa
            Process.GetCurrentProcess().WaitForExit();
        }

        /// <summary>
        /// Creates a new error instance
        /// </summary>
        /// <param name="err">Exception</param>
        /// <returns></returns>
        private static Formats.Generic.Error CreateError(Exception err)
        {
            var response = new Formats.Generic.Error();
            response.Code = err.GetHashCode();
            response.Text = err.Message;
            return response;
        }

        /// <summary>
        /// Simply gets the following value of a specified value in an array.
        /// </summary>
        /// <param name="name">Identifier value</param>
        /// <returns>Returns "" if invalid</returns>
        private static string GetValueOfSwitch(string name)
        {
            var result = "";

            // get commandline args as list
            var args = new List<string>();
            args.AddRange(Environment.GetCommandLineArgs());

            // get index of switch
            var i = args.IndexOf(name);

            // return if not found
            if (i == -1) return result;

            // check if index is valid
            if (args.Count == i + 1) return result;

            // get value of switch
            result = args[i + 1];
            return result;
        }
    }
}
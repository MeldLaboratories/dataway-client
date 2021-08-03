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
        private static readonly Client client = new Client();
        private static readonly TrayIcon icon = new TrayIcon();

        private static void Main(string[] args)
        {
            // set a custom pipe server name by commandline arguments, else fallback to "Dataway"
            var pipename = "Dataway";
            if (GetValueOfSwitch("--pipe") != "") pipename = GetValueOfSwitch("--pipe");
            if (GetValueOfSwitch("-p") != "") pipename = GetValueOfSwitch("-p");
            if (pipename != "Dataway") Console.WriteLine("Custom Pipe-Server Name: {0}", pipename);

            // start pipe server
            var server = new SimpleNamedPipeServer(pipename);
            server.Start();

            // start dataway client
            IPAddress ip = Dns.GetHostAddresses("kevin-ortmann.com")[0];
            var res = client.Connect(ip, 2000); //TODO: IF REFUSED DO SOMETHING
            if (res.code != 0) Toaster.ShowErrorToast("Error trying to connect to the Dataway server", res.message);

            // attempt auto login via config
            var properties = Properties.Settings.Default;
            if (properties.Password != "" || properties.Username != "")// CHANGE TO && IN RELEASE DEBUG
            {
                Console.WriteLine("Attempting auto login with credentials: " + properties.Username + "+" + properties.Password); //DEBUG
                var command = new Formats.Login.Command()
                {
                    Username = properties.Username,
                    Password = properties.Password
                };

                Actions.Login.PerformLogin(server, client, command);
            }

            Properties.Settings.Default.Username = "c";
            Properties.Settings.Default.Password = "";
            Properties.Settings.Default.Save();

            // transmit request listener
            client.OnTransmitRequest += HandleTransmitRequest;

            // toast listener
            Toaster.HandleToastResponses(client);

            // TODO: add error handling
            // TODO: add ip switch

            server.ClientConnected += delegate (SimpleNamedPipeServer _)
            {
                Console.WriteLine("Pipe connected");

                // handle data
                while (server.IsConnected)
                {
                    var msg = server.WaitForResponse(1000);
                    if (msg == "" || msg == null) continue;

                    var rawData = JsonConvert.DeserializeObject<Formats.Base>(msg);

                    //Console.WriteLine(msg);

                    // handle send objects
                    if (rawData.Action.ToUpper() == "SEND")
                    {
                        Actions.Send.Request(server, client, JsonConvert.DeserializeObject<Formats.Send.Command>(msg)); //TODO: context menu vs cmd command | error handling via gui or cmd promt msg????
                    }

                    // handle login objects
                    if (rawData.Action.ToUpper() == "LOGIN")
                    {
                        Actions.Login.PerformLogin(server, client, JsonConvert.DeserializeObject<Formats.Login.Command>(msg));
                    }

                    // handle register objects
                    if (rawData.Action.ToUpper() == "REGISTER")
                    {
                        Actions.Register.PerformRegister(server, client, JsonConvert.DeserializeObject<Formats.Register.Command>(msg));
                    }
                }
            };

            // Wait for current process to end. Kappa
            Process.GetCurrentProcess().WaitForExit();
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

        /// <summary>
        /// Handles incoming transmit requests
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="filename"></param>
        /// <param name="filesizeMB"></param>
        private static void HandleTransmitRequest(object invoker, string sender, string message, string filename, int filesizeMB)
        {
            Console.WriteLine("Incoming transmit request from {0} with file {1}({2}MB) with message {3}", sender, filename, filesizeMB, message);

            if (icon.Muted)
            {
                Console.WriteLine("Declined incomming transmit request from {0} ({1}).", sender, filename);
                client.DeclineCurrentTransmitRequest();
                return;
            }

            Toaster.ShowReceiveToast(sender, filename, filesizeMB);
        }
    }
}
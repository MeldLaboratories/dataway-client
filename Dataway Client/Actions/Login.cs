using Newtonsoft.Json;
using System;

namespace Dataway_Client.Actions
{
    internal class Login
    {
        /**
         * Gets executed on the user verb 'login'
         */

        public static int Run(Helper.Login opts)
        {
            // spawn pipe client
            var client = PipeSpawner.Spawn(opts.Pipename);

            // wait for user input
            Console.Write("Username: ");
            var user = Console.ReadLine();

            Console.Write("Password: ");
            var password = ReadLineSecure();
            Console.WriteLine();

            // communicate with worker
            Console.WriteLine("Connecting to worker process...");
            client.WaitForConnection();

            var command = new Formats.Login.Command();
            command.Password = password;
            command.Username = user;

            // send command
            client.PushMessage(JsonConvert.SerializeObject(command));

            Console.WriteLine("Authenticating with server...");

            // begin action loop
            while (true)
            {
                var resp = client.WaitForResponse();
                var baseData = JsonConvert.DeserializeObject<Formats.Base>(resp);

                // handle login actions
                if (baseData.Action.ToUpper() == "LOGIN")
                {
                    // TODO: add send actions
                }

                // handle Generic actions
                if (baseData.Action.ToUpper() == "GENERIC")
                {
                    switch (baseData.Type.ToUpper())
                    {
                        case "MESSAGE":
                            break;

                        case "ERROR":
                            var data = JsonConvert.DeserializeObject<Formats.Generic.Error>(resp);
                            Console.WriteLine("\nThe worker process reported an error.\nError: {0}\nCode: {1}", data.Text, data.Code);
                            return data.Code;

                        case "COMPLETE":
                            return 0;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Hides the input characters
        /// </summary>
        /// <returns></returns>
        private static string ReadLineSecure()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass.Substring(0, pass.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }
    }
}
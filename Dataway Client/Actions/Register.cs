using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;
using System;

namespace Dataway_Client.Actions
{
    internal class Register
    {
        /**
         * Gets executed on the user verb 'register'
         */

        public static int Run(Helper.Register opts)
        {
            // spawn pipe client
            var client = PipeSpawner.Spawn(opts.Pipename);

            string user, pw = "?", pwr = "!";

            do
            {
                // wait for user input
                Console.Write("Username: ");
                user = Console.ReadLine();

                Console.Write("Password: ");
                pw = ReadLineSecure();

                Console.Write("Repeat Password: ");
                pwr = ReadLineSecure();
                Console.WriteLine();

                if (pw != pwr) Console.WriteLine("The passwords do not match!\n");
            } while (pw != pwr);

            // communicate with worker
            Console.WriteLine("Connecting to worker process...");
            client.WaitForConnection();

            var command = new Formats.Register.Command();
            command.Password = pw;
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
                if (baseData.Action.ToUpper() == "REGISTER")
                {
                    // TODO: add register actions
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
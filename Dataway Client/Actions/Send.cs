using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;
using System;

namespace Dataway_Client.Actions
{
    internal class Send
    {
        /**
         * Gets executed on the user verb 'send'
         */

        public static int Run(Helper.Send opts, SimpleNamedPipeClient client)
        {
            Console.WriteLine("Connecting to worker process...");
            client.WaitForConnection();

            Console.WriteLine("Sending command...");
            var command = new Formats.Send.Command();
            command.File = opts.File;
            command.User = opts.User;
            command.Message = opts.Message;

            // send command as json
            client.PushMessage(JsonConvert.SerializeObject(command));

            // begin action loop
            while (true)
            {
                var resp = client.WaitForResponse();
                var baseData = JsonConvert.DeserializeObject<Formats.Base>(resp);

                // handle Send actions
                if (baseData.Action.ToUpper() == "SEND")
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
    }
}
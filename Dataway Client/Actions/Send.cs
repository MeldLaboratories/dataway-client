using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client.Actions
{
    class Send
    {
        /**
         * Gets executed on the user verb 'send'
         */
        public static int Run(Helper.Send opts)
        {
            Console.WriteLine("Connecting to worker process...");
            var client = new Client();

            // wait for connection
            client.Wait();
            System.Threading.Thread.Sleep(1000);

            Console.WriteLine("Preparing actions...");
            var data = new Formats.Data();
            data.Action = "upload";
            data.File = opts.File;
            data.Username = opts.User;
            data.Message = opts.Message;

            Console.WriteLine("Requesting file transfer...");
            client.Send(data);

            return 0;
        }
    }
}

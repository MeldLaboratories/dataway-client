using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client.Actions
{
    class Send
    {
        public static int Run(Helper.Send opts)
        {
            Console.WriteLine("File: " + opts.File);
            Console.WriteLine("User: " + opts.User);
            Console.WriteLine("Msg: " + opts.Message);

            return 0;
        }
    }
}

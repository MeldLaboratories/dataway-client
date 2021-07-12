using PLib.SimpleNamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client.Actions
{
    class Register
    {
        /**
         * Gets executed on the user verb 'register'
         */
        public static int Run(Helper.Register opts, SimpleNamedPipeClient client)
        {
            Console.WriteLine("Your register message here.");

            return 0;
        }
    }
}

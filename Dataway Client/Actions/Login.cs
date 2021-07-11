using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client.Actions
{
    class Login
    {
        /**
         * Gets executed on the user verb 'login'
         */
        public static int Run(Helper.Login opts)
        {
            Console.WriteLine("Your login message here.");

            return 0;
        }
    }
}

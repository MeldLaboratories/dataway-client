using System;

namespace Dataway_Client.Actions
{
    internal class Debug
    {
        /**
         * Gets executed on the user verb 'debug'
         */

        public static int Run(Helper.Debug opts)
        {
            Console.WriteLine("Selected Pipe-Server: {0}", opts.Pipename);

            return 0;
        }
    }
}
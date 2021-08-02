using System;

namespace Dataway_Client.Actions
{
    internal class AddQuick
    {
        /**
         * Gets executed on the user verb 'addquick'
         */

        public static int Run(Helper.AddQuick opts)
        {
            DWHelper.AddQuickSendRegistryKey(opts.Username, @"D:\Stash\Code\Dataway\dataway-client\Dataway Client\bin\Debug\dw.exe");
            
            return 0;
        }
    }
}

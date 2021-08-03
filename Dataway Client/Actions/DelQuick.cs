namespace Dataway_Client.Actions
{
    internal class DelQuick
    {
        /**
         * Gets executed on the user verb 'addquick'
         */

        public static int Run(Helper.DelQuick opts)
        {
            DWHelper.RemoveQuickSendRegistryKey(opts.Username);

            return 0;
        }
    }
}
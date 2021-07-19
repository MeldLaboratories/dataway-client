using PLib.SimpleNamedPipeWrapper;
using System;

namespace Dataway_Worker.Actions
{
    internal class Login
    {
        public static void PerformLogin(SimpleNamedPipeServer _, Client client, Formats.Login.Command command)
        {
            // send login task
            var res = client.Login(command.Username, command.Password);

            if (res.code != 0) throw new Exception(res.message);
            return;
        }
    }
}
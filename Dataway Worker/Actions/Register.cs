using PLib.SimpleNamedPipeWrapper;
using System;

namespace Dataway_Worker.Actions
{
    internal class Register
    {
        public static void PerformRegister(SimpleNamedPipeServer server, Client client, Formats.Register.Command command)
        {
            // send register task
            var res = client.Register(command.Username, command.Password);

            if (res.code != 0) throw new Exception(res.message);
            return;
        }
    }
}
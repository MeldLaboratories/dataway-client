using PLib.SimpleNamedPipeWrapper;
using System;

namespace Dataway_Worker.Actions
{
    internal class Send
    {
        public static void Request(SimpleNamedPipeServer server, Client client, Formats.Send.Command command)
        {
            var parts = command.File.Split('\\');
            var filename = parts[parts.Length - 1];

            // send send task
            var res = client.SendFile(command.File, filename, command.User);
            // TODO: add message

            if (res.code != 0) throw new Exception(res.message);
            return;
        }
    }
}
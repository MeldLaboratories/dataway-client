using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;

namespace Dataway_Worker.Actions
{
    internal class Send
    {
        public static void Request(SimpleNamedPipeServer server, Client client, Formats.Send.Command command)
        {
            var parts = command.File.Split('\\');
            var filename = parts[parts.Length - 1];

            Result res = client.SendFile(command.File, filename, command.User);

            if (res.code == (int)Result.CODE.SUCCESS)
            {
                //TODO: toast or console
                DWHelper.ShowErrorBox(res.message);
                server.PushMessage(JsonConvert.SerializeObject(Error.CreateError(res)));
            }

            // return success
            server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
        }
    }
}
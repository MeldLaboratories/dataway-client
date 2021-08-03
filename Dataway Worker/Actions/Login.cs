using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;

namespace Dataway_Worker.Actions
{
    internal class Login
    {
        public static void PerformLogin(SimpleNamedPipeServer server, Client client, Formats.Login.Command command)
        {
            var res = client.Login(command.Username, command.Password);

            if (res.code == (int)Result.CODE.SUCCESS)
            {
                //TODO: toast or console
                server.PushMessage(JsonConvert.SerializeObject(Error.CreateError(res))); //TODO: toast or console
                DWHelper.ShowErrorBox(res.message);
            }

            // return success
            server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete())); //TODO: toast or console
        }
    }
}
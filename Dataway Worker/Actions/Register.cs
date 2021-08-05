using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;

namespace Dataway_Worker.Actions
{
    internal class Register
    {
        public static void PerformRegister(SimpleNamedPipeServer server, Client client, Formats.Register.Command command)
        {
            Result res = client.Register(command.Username, command.Password);

            if (res.code != (int)Result.CODE.SUCCESS)
            {
                //TODO: toast or console
                server.PushMessage(JsonConvert.SerializeObject(Error.CreateError(res))); //TODO: toast or console
                DWHelper.ShowErrorBox(res.message);
            }

            // return success
            server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
        }
    }
}
using Newtonsoft.Json;
using Pipes.SimpleNamedPipeWrapper;
using System.IO;

namespace Dataway_Worker.Actions
{
    internal class Send
    {
        public static void Request(SimpleNamedPipeServer server, Client client, Formats.Send.Command command)
        {
            var parts = command.File.Split('\\');
            var filename = parts[parts.Length - 1];

            FileInfo fileInfo = new FileInfo(command.File);
            int filesize = (int)fileInfo.Length;

            Result res = client.SendFile(command.File, filename, command.User, filesize);

            if (res.code != (int)Result.CODE.SUCCESS)
            {
                if(res.code == (int)Result.CODE.DECLINED_TRANSMIT_REQUEST){
                    Toaster.ShowErrorToast("Transmit request declined", "");
                    server.PushMessage(JsonConvert.SerializeObject(Error.CreateError(res)));
                }
                else
                {
                    DWHelper.ShowErrorBox(res.message);
                    server.PushMessage(JsonConvert.SerializeObject(Error.CreateError(res)));
                }
                //TODO: toast or console
            }

            // return success //TODO: maybe make a toast
            server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
        }
    }
}
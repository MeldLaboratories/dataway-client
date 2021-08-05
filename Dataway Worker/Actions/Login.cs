using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;
using System.Net;
using System;
namespace Dataway_Worker.Actions
{
    internal class Login
    {
        public static void PerformLogin(SimpleNamedPipeServer server, Client client, Formats.Login.Command command)
        {
            // connect dataway client
            IPAddress ip = Dns.GetHostAddresses(Properties.Settings.Default.Server)[0];
            var res = client.Connect(ip, 2000); //TODO: IF REFUSED DO SOMETHING
            if (res.code != 0) {
                Toaster.ShowErrorToast("Error trying to connect to the Dataway server", res.message); //TODO: toast or console
                return;
            }
            
            // attempt login
            res = client.Login(command.Username, command.Password);
            Console.WriteLine(res.message);

            if (res.code != (int)Result.CODE.SUCCESS)
            {
                //TODO: toast or console
                Console.WriteLine("errpr");
                server.PushMessage(JsonConvert.SerializeObject(Error.CreateError(res))); //TODO: toast or console
                DWHelper.ShowErrorBox(res.message);
            }

            // return success
            server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete())); //TODO: toast or console
        }
    }
}
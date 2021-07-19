using CommandLine;
using PLib.SimpleNamedPipeWrapper;

namespace Dataway_Client
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            // start pipe client
            var client = new SimpleNamedPipeClient("Dataway");
            client.Start();

            // Parse commandline arguments
            return Parser.Default.ParseArguments<Helper.Send, Helper.Register, Helper.Login>(args)
                .MapResult(
                    (Helper.Send snd) => Actions.Send.Run(snd, client),
                    (Helper.Register rgs) => Actions.Register.Run(rgs, client),
                    (Helper.Login lgn) => Actions.Login.Run(lgn, client),

                    errs => 1
                );
        }
    }
}
using CommandLine;
using PLib.SimpleNamedPipeWrapper;
using System;

namespace Dataway_Client
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var client = new SimpleNamedPipeClient("Dataway");
            client.Start();

            // Parse commandline arguments
            return Parser.Default.ParseArguments<Helper.Send, Helper.Register, Helper.Login, Helper.Debug>(args)
                .MapResult(
                    (Helper.Send snd) => Actions.Send.Run(snd),
                    (Helper.Register rgs) => Actions.Register.Run(rgs),
                    (Helper.Login lgn) => Actions.Login.Run(lgn),
                    (Helper.Debug dbg) => Actions.Debug.Run(dbg),

                    errs => 1
                );
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLib.SimpleNamedPipeWrapper;
using CommandLine;

namespace Dataway_Client
{
    class Program
    {
        static int Main(string[] args)
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

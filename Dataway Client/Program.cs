using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Dataway_Client
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Helper.Send, Helper.Register, Helper.Login>(args)
                .MapResult(
                    (Helper.Send snd) => Actions.Send.Run(snd),
                    (Helper.Register rgs) => Actions.Register.Run(rgs),
                    (Helper.Login lgn) => Actions.Login.Run(lgn),

                    errs => 1
                );
        }
    }
}

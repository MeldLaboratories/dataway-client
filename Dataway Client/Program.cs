using CommandLine;

namespace Dataway_Client
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            // Parse commandline arguments
            return Parser.Default.ParseArguments<Helper.Send, Helper.Register, Helper.Login, Helper.Debug, Helper.AddQuick, Helper.DelQuick>(args)
                .MapResult(
                    (Helper.Send snd) => Actions.Send.Run(snd),
                    (Helper.Register rgs) => Actions.Register.Run(rgs),
                    (Helper.Login lgn) => Actions.Login.Run(lgn),
                    (Helper.Debug dbg) => Actions.Debug.Run(dbg),
                    (Helper.AddQuick adq) => Actions.AddQuick.Run(adq),
                    (Helper.DelQuick dlq) => Actions.DelQuick.Run(dlq),

                    errs => 1
                );
        }
    }
}
using CommandLine;

namespace Dataway_Client.Helper
{
    internal class Generic
    {
        [Option('p', "pipe", Required = false, Hidden = true, Default = "Dataway", HelpText = "Intended for debug purposes only. Specifies a user-defined process pipe ID.")]
        public string Pipename { get; set; }
    }
}
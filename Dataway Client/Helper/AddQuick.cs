using CommandLine;

namespace Dataway_Client.Helper
{
    [Verb("addquick", HelpText = "Adds a user to the quick-send context menu.")]
    internal class AddQuick
    {
        [Value(0, MetaName = "Username", Required = true)]
        public string Username { get; set; }
    }
}
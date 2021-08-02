using CommandLine;

namespace Dataway_Client.Helper
{
    [Verb("delquick", HelpText = "Deletes a user from the quick-send context menu.")]
    internal class DelQuick
    {
        [Value(0, MetaName = "Username", Required = true)]
        public string Username { get; set; }
    }
}
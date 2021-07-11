using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Dataway_Client.Helper
{
    [Verb("send", HelpText = "Send a file to a user.")]
    class Send
    {
        [Option('f', "file", Required = true, HelpText = "The file to be sent.")]
        public string File { get; set; }

        [Option('u', "user", Required = true, HelpText = "Target user.")]
        public string User { get; set; }

        [Option('m', "message", HelpText = "Send a message to the target user.")]
        public string Message { get; set; }
    }
}

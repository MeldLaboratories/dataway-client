using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client.Helper
{
    class Generic
    {
        [Option('p', "pipe", Required = false, Hidden = true, Default = "Dataway", HelpText = "Intended for debug purposes only. Specifies a user-defined process pipe ID.")]
        public string Pipename { get; set; }
    }
}

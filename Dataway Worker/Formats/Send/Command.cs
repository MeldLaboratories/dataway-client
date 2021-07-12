using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Worker.Formats.Send
{
    class Command
    {
        public string Type = "Request";
        public string Action = "Send";

        public string File = "";
        public string User = "";
        public string Message = "";
    }
}

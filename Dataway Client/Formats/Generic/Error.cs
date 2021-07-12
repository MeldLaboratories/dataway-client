using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client.Formats.Generic
{
    class Error
    {
        public string Type = "Error";
        public string Action = "Generic";

        public int Code = 0;
        public string Text = "";
    }
}

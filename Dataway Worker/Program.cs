using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new Worker();
            w.Start();
            Process.GetCurrentProcess().WaitForExit();
        }
    }
}

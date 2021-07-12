using Newtonsoft.Json;
using PLib.SimpleNamedPipeWrapper;
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
            Console.WriteLine("> Dataway Worker <");

            // start pipe server
            var server = new SimpleNamedPipeServer("Dataway");
            server.Start();

            server.ClientConnected += delegate (SimpleNamedPipeServer _)
            {
                Console.WriteLine("User connected");

                while(server.IsConnected)
                {
                    var msg = server.WaitForResponse(1000);
                    if (msg == "" || msg == null) continue;

                    var rawData = JsonConvert.DeserializeObject<Formats.Base>(msg);

                    Console.WriteLine(msg);

                    // handle send objects
                    if (rawData.Action.ToUpper() == "SEND")
                    {
                        switch (rawData.Type.ToUpper())
                        {
                            case "REQUEST":
                                // Todo: upload code here

                                server.PushMessage(JsonConvert.SerializeObject(new Formats.Generic.Complete()));
                                break;

                            default:
                                break;
                        }
                    }

                    // handle generic data
                    if (rawData.Action.ToUpper() == "GENERIC")
                    {
                        switch (rawData.Type.ToUpper())
                        {
                            case "MESSAGE":
                                break;

                            case "ERROR":
                                break;

                            case "COMPLETE":
                                break;

                            default:
                                break;
                        }
                    }
                }
            };


            Process.GetCurrentProcess().WaitForExit();
        }
    }
}

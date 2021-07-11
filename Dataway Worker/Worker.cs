using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Worker
{
    class Worker
    {
        // provides a name pipe server
        private NamedPipeServer<Formats.Data> server;

        public void Start()
        {
            // prepare pipe security
            var security = new PipeSecurity();
            var accessRule = new PipeAccessRule("Benutzer", PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);

            // update pipe security
            security.AddAccessRule(accessRule);

            // spawn new pipe server
            this.server = new NamedPipeServer<Formats.Data>("Dataway", security);
            this.server.ClientConnected += delegate (NamedPipeConnection<Formats.Data, Formats.Data> conn) { Console.WriteLine("Client connected"); };
            this.server.ClientDisconnected += delegate (NamedPipeConnection<Formats.Data, Formats.Data> conn) { Console.WriteLine("Client disconnected"); };

            // register handlers
            this.server.ClientMessage += Server_ClientMessage;

            // start pipe
            this.server.Start();
        }

        private void Server_ClientMessage(NamedPipeConnection<Formats.Data, Formats.Data> connection, Formats.Data data)
        {
            // upload
            if (data.Action.ToUpper() == "UPLOAD")
            {
                Console.WriteLine("ACTION: {0}\nMESSAGE: {1}\nFILE: {2}\nUSERNAME: {3}", data.Action, data.Message, data.File, data.Username);
            }
            else
            {
                Console.WriteLine(data.Action); // TODO: FIX transfer bla bla bla
            }
        }
    }
}

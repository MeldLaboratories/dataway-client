using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataway_Client
{
    class Client
    {
        private NamedPipeClient<Formats.Data> c;

        public delegate void OnDataHandler(object sender, Formats.Data data);
        public event OnDataHandler OnData;
        public delegate void OnErrorHandler(object sender, Exception err);
        public event OnErrorHandler OnError;

        public Client()
        {
            // prepare pipe
            this.c = new NamedPipeClient<Formats.Data>("Dataway");

            // register events
            this.c.ServerMessage += delegate (NamedPipeConnection<Formats.Data, Formats.Data> conn, Formats.Data data)
            {
                OnData?.Invoke(this, data);
            };
            this.c.Error += C_Error;

            this.c.Start();

            this.c.PushMessage(new Formats.Data());
        }

        public void Send(Formats.Data data)
        {
            this.c.PushMessage(data);
        }

        public void Wait()
        {
            this.c.WaitForConnection();
        }

        private void C_Error(Exception exception)
        {
            OnError?.Invoke(this, exception);
        }
    }
}

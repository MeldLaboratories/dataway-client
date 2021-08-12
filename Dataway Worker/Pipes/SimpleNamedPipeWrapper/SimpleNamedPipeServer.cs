using Pipes.NamedPipeWrapper;
using Pipes.NamedPipeWrapper.MessageObjects;
using System;
using System.IO.Pipes;
using System.Threading;

namespace Pipes.SimpleNamedPipeWrapper
{
    public class SimpleNamedPipeServer
    {
        private NamedPipeServer<BasicMessageObject> _server;

        public delegate void ConnectionEventHandler(SimpleNamedPipeServer sender);

        public delegate void ConnectionMessageEventHandler(SimpleNamedPipeServer sender, string message);

        public delegate void PipeExceptionEventHandler(SimpleNamedPipeServer sender, Exception error);

        private AutoResetEvent _messageRecieved;
        private string _lastData = "";
        private bool _connected = false;

        private void PrepareEvents()
        {
            this._messageRecieved = new AutoResetEvent(false);
            this._server.ClientConnected += delegate (NamedPipeConnection<BasicMessageObject, BasicMessageObject> _) { _connected = true; this.ClientConnected?.Invoke(this); };
            this._server.ClientDisconnected += delegate (NamedPipeConnection<BasicMessageObject, BasicMessageObject> _) { _connected = false; this.ClientDisconnected?.Invoke(this); };
            this._server.ClientMessage += delegate (NamedPipeConnection<BasicMessageObject, BasicMessageObject> _, BasicMessageObject data)
            {
                this.ClientMessage?.Invoke(this, data.Message);
                this._lastData = data.Message;
            };
            this._server.Error += delegate (Exception error) { this.Error?.Invoke(this, error); };
        }

        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        public event ConnectionEventHandler ClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        public event ConnectionEventHandler ClientDisconnected;

        /// <summary>
        /// Invoked whenever a client sends a message to the server.
        /// </summary>
        public event ConnectionMessageEventHandler ClientMessage;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation.
        /// </summary>
        public event PipeExceptionEventHandler Error;

        /// <summary>
        /// Constructs a new <c>SimpleNamedPipeServer</c> object that listens for client connections on the given <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to listen on</param>
        public SimpleNamedPipeServer(string pipeName)
        {
            this._server = new NamedPipeServer<BasicMessageObject>(pipeName);
            this.PrepareEvents();
        }

        public bool IsConnected { get { return this._connected; } }

        /// <summary>
        /// Constructs a new <c>SimpleNamedPipeServer</c> object that listens for client connections on the given <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to listen on</param>
        public SimpleNamedPipeServer(string pipeName, PipeSecurity security)
        {
            this._server = new NamedPipeServer<BasicMessageObject>(pipeName, security);
            this.PrepareEvents();
        }

        /// <summary>
        /// Begins listening for client connections in a separate background thread.
        /// This method returns immediately.
        /// </summary>
        public void Start()
        {
            this._server.Start();
        }

        public string WaitForResponse()
        {
            this._messageRecieved.WaitOne();
            var data = this._lastData;
            this._lastData = "";
            return data;
        }

        public string WaitForResponse(int millisecondsTimeout)
        {
            this._messageRecieved.WaitOne(millisecondsTimeout);
            var data = this._lastData;
            this._lastData = "";
            return data;
        }

        public string WaitForResponse(TimeSpan timeout)
        {
            this._messageRecieved.WaitOne(timeout);
            var data = this._lastData;
            this._lastData = "";
            return data;
        }

        /// <summary>
        /// Sends a message to all connected clients asynchronously.
        /// This method returns immediately, possibly before the message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(string message)
        {
            this._server.PushMessage(new BasicMessageObject() { Message = message });
        }

        /// <summary>
        /// Push message to the given client.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="clientName"></param>
        public void PushMessage(string message, string clientName)
        {
            this._server.PushMessage(new BasicMessageObject() { Message = message }, clientName);
        }

        /// <summary>
        /// Closes all open client connections and stops listening for new ones.
        /// </summary>
        public void Stop()
        {
            this._server.Stop();
        }
    }
}
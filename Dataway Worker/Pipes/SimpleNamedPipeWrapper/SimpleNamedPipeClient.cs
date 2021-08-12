using Pipes.NamedPipeWrapper;
using Pipes.NamedPipeWrapper.MessageObjects;
using System;
using System.IO.Pipes;
using System.Threading;

namespace Pipes.SimpleNamedPipeWrapper
{
    public class SimpleNamedPipeClient
    {
        private NamedPipeClient<BasicMessageObject> _client;

        public delegate void ConnectionMessageEventHandler(SimpleNamedPipeClient sender, string message);

        public delegate void ConnectionEventHandler(SimpleNamedPipeClient sender);

        public delegate void PipeExceptionEventHandler(SimpleNamedPipeClient sender, Exception error);

        private AutoResetEvent _messageRecieved;
        private string _lastData = "";

        /// <summary>
        /// Invoked whenever a message is received from the server.
        /// </summary>
        public event ConnectionMessageEventHandler ServerMessage;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        public event ConnectionEventHandler Disconnected;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        public event PipeExceptionEventHandler Error;

        /// <summary>
        /// Creates a new <see cref="NamedPipeClientStream"></see> to allow interprocess communications.
        /// <see cref="SimpleNamedPipeClient"/> is a simplified version of <see cref="NamedPipeClient{TRead, TWrite}"/> wich only allows string transfers.
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        /// <param name="serverName">Server name default is local.</param>
        public SimpleNamedPipeClient(string pipeName, string serverName = ".")
        {
            this._client = new NamedPipeClient<BasicMessageObject>(pipeName, serverName);
            this._messageRecieved = new AutoResetEvent(false);

            // register event handlers
            this._client.Disconnected += delegate (NamedPipeConnection<BasicMessageObject, BasicMessageObject> _)
            {
                this.Disconnected?.Invoke(this);
            };
            this._client.Error += delegate (Exception error)
            {
                this.Error?.Invoke(this, error);
            };
            this._client.ServerMessage += delegate (NamedPipeConnection<BasicMessageObject, BasicMessageObject> _, BasicMessageObject data)
            {
                this.ServerMessage?.Invoke(this, data.Message);
                this._lastData = data.Message;
                this._messageRecieved.Set();
            };
        }

        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// This method returns immediately, possibly before the connection has been established.
        /// </summary>
        public void Start()
        {
            this._client.Start();
        }

        /// <summary>
        ///     Sends a message to the server over a named pipe.
        /// </summary>
        /// <param name="message">Message to send to the server.</param>
        public void PushMessage(string message)
        {
            this._client.PushMessage(new BasicMessageObject() { Message = message });
        }

        /// <summary>
        /// Closes the named pipe.
        /// </summary>
        public void Stop()
        {
            this._client.Stop();
        }

        #region Wait for connection/disconnection

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

        public void WaitForConnection()
        {
            this._client.WaitForConnection();
        }

        public void WaitForConnection(int millisecondsTimeout)
        {
            this._client.WaitForConnection(millisecondsTimeout);
        }

        public void WaitForConnection(TimeSpan timeout)
        {
            this._client.WaitForConnection(timeout);
        }

        public void WaitForDisconnection()
        {
            this._client.WaitForDisconnection();
        }

        public void WaitForDisconnection(int millisecondsTimeout)
        {
            this._client.WaitForDisconnection(millisecondsTimeout);
        }

        public void WaitForDisconnection(TimeSpan timeout)
        {
            this._client.WaitForDisconnection(timeout);
        }

        #endregion Wait for connection/disconnection
    }
}
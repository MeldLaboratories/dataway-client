using PLib.SimpleNamedPipeWrapper;

namespace Dataway_Client
{
    internal class PipeSpawner
    {
        /// <summary>
        /// Creates a new SimpleNamedPipeClient instance and starts it.
        /// </summary>
        /// <param name="name">pipe id</param>
        /// <param name="path">pipe path</param>
        /// <returns>SimpleNamedPipeClient</returns>
        public static SimpleNamedPipeClient Spawn(string name, string path = ".")
        {
            var pipe = new SimpleNamedPipeClient(name, path);
            pipe.Start();

            return pipe;
        }
    }
}
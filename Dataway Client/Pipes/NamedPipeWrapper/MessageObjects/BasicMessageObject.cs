using System;

namespace Pipes.NamedPipeWrapper.MessageObjects
{
    [Serializable]
    public class BasicMessageObject
    {
        public int Id = 0;
        public string Message = "";
    }
}
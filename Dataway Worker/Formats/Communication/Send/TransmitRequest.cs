namespace Dataway_Worker.Formats.Communication.Send
{
    internal class TransmitRequest
    {
        public string type = "transmitRequest";
        public string filename { get; set; }
        public string reciever { get; set; }
    }
}
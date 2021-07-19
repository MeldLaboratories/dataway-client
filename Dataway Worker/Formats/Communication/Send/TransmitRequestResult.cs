namespace Dataway_Worker.Formats.Communication.Send
{
    internal class TransmitRequestResult
    {
        public string type = "transmitRequestResult";
        public int result { get; set; }
        public string reciever { get; set; }
    }
}
namespace Dataway_Worker.Formats.Communication.Recieve
{
    internal class TransmitRequest
    {
        public string type = "transmitRequest";
        public string filename { get; set; }
        public string sender { get; set; }
    }
}
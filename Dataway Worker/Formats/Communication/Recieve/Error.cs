namespace Dataway_Worker.Formats.Communication.Recieve
{
    internal class Error
    {
        public string type = "error";
        public string code { get; set; }
        public string initialRequestType { get; set; }
    }
}
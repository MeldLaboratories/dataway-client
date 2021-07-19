namespace Dataway_Worker.Formats.Send
{
    internal class Command
    {
        public string Type = "Request";
        public string Action = "Send";

        public string File = "";
        public string User = "";
        public string Message = "";
    }
}
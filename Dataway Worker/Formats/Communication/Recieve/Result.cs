namespace Dataway_Worker.Formats.Communication.Recieve
{
    internal class Result
    {
        public string type = "result";
        public string origin { get; set; }
        public int code { get; set; }
    }
}
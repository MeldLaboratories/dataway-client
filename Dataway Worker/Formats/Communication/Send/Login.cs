namespace Dataway_Worker.Formats.Communication.Send
{
    internal class Login
    {
        public string type = "login";
        public string username { get; set; }
        public string password { get; set; }
    }
}
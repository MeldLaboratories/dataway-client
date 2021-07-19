namespace Dataway_Worker.Formats.Communication.Send
{
    internal class Logout
    {
        public string type = "logout";
        public string username { get; set; }
        public string password { get; set; }
    }
}
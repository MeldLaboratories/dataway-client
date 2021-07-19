namespace Dataway_Worker.Formats.Communication.Send
{
    internal class Register
    {
        public string type = "register";
        public string username { get; set; }
        public string password { get; set; }
    }
}
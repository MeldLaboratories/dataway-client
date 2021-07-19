namespace Dataway_Worker.Formats.Register
{
    internal class Command
    {
        public string Type = "Request";
        public string Action = "Register";

        public string Username = "";
        public string Password = "";
    }
}
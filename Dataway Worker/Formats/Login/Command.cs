namespace Dataway_Worker.Formats.Login
{
    internal class Command
    {
        public string Type = "Request";
        public string Action = "Login";

        public string Username = "";
        public string Password = "";
    }
}
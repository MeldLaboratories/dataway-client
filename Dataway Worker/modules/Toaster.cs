using Microsoft.Toolkit.Uwp.Notifications;
using PLib.SimpleNamedPipeWrapper;
using System;
using Windows.UI.Notifications;

namespace Dataway_Worker
{
    internal class Toaster
    {
        /// <summary>
        /// Shows a File-Receive Toast-Notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="filename"></param>
        /// <param name="filesizeMB"></param>
        public static void ShowReceiveToast(string sender, string filename, int filesizeMB)
        {
            new ToastContentBuilder()
            .AddArgument("", "dw-rec-invalid")
            .AddText(sender + " wants to send you '" + filename + "' (" + filesizeMB + "MB)")
            .AddButton("Accept", ToastActivationType.Background, "dw-rec-success")
            .AddButton("Decline", ToastActivationType.Background, "dw-rec-fail")
            .SetToastScenario(ToastScenario.Reminder)
            .AddAudio(new Uri(@"C:\Windows\Media\Dataway.wav"))
            .Show();
        }

        /// <summary>
        /// Shows a File-Send Toast-Notification
        /// </summary>
        /// <param name="filename"></param>
        public static void ShowSendToast(string filename)
        {
            new ToastContentBuilder()
            .AddArgument("", "dw-send-invalid")
            .AddText("Send the file: " + filename)
            .AddInputTextBox("receiver", "Receiver")
            .AddInputTextBox("message", "Message (optional)")
            .AddButton("Send", ToastActivationType.Background, "dw-send-success")
            .AddButton("Cancel", ToastActivationType.Background, "dw-send-fail")
            .SetToastScenario(ToastScenario.Reminder)
            .Show();
        }

        public static void ShowErrorToast(string title, string message)
        {
            new ToastContentBuilder()
            .AddAppLogoOverride(new Uri(Environment.CurrentDirectory + @"\assets\toastErrorImage.png"))
            .AddText(title)
            .AddText(message)
            .SetToastScenario(ToastScenario.Default)
            .Show();
        }


        public static void ShowLoginRegisterToast()
        {
            new ToastContentBuilder()
            .AddArgument("", "dw-auth-invalid")
            .AddText("Login with your Dataway account")
            .AddText("Or use register to create a new one")
            .AddInputTextBox("username", "Username")
            .AddInputTextBox("password", "Password")
            .AddButton("Login", ToastActivationType.Background, "dw-auth-login")
            .AddButton("Register", ToastActivationType.Background, "dw-auth-register")
            .SetToastScenario(ToastScenario.Reminder)
            .Show();
        }



        /// <summary>
        /// Handles all the Dataway Toast responses
        /// </summary>
        public static void HandleToastResponses(Client client, SimpleNamedPipeServer server)
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                //Parse result
                string rawResult = toastArgs.Argument.Replace("=", "");
                if (!rawResult.StartsWith("dw")) return;
                string type = rawResult.Split('-')[1];
                string result = rawResult.Split('-')[2];

                //Debug
                Console.WriteLine("Type:" + type);
                Console.WriteLine("Answer:" + result);

                if (type == "rec")
                {
                    if (result == "invalid") return; //TODO: do something
                    else if (result == "success") client.AcceptCurrentTransmitRequest();
                    else if (result == "fail") client.DeclineCurrentTransmitRequest();
                }
                else if (type == "send")
                {
                    if (result == "invalid") return; //TODO: do something

                    string receiver = (string)toastArgs.UserInput["receiver"];
                    string message = (string)toastArgs.UserInput["message"];
                    Console.WriteLine("Receiver: " + receiver);
                    Console.WriteLine("Message: " + message);
                }
                else if (type == "auth")
                {
                    if (result == "invalid") return; //TODO: do something

                    if(result == "login")
                    {
                        var command = new Formats.Login.Command()
                        {
                            Username = (string)toastArgs.UserInput["username"],
                            Password = (string)toastArgs.UserInput["password"]
                        };

                        Actions.Login.PerformLogin(server, client, command);
                    }
                    else if(result == "register")
                    {
                        var command = new Formats.Register.Command()
                        {
                            Username = (string)toastArgs.UserInput["username"],
                            Password = (string)toastArgs.UserInput["password"]
                        };

                        Actions.Register.PerformRegister(server, client, command);
                    }

                   
                }
            };
        }
    }
}
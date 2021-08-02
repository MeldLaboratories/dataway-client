using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Dataway_Worker.modules
{
    class Toaster
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
            .AddArgument("", "rec-invalid")
            .AddText(sender + " wants to send you '" + filename + "' (" + filesizeMB + "MB)")
            .AddButton("Accept", ToastActivationType.Background, "rec-success")
            .AddButton("Decline", ToastActivationType.Background, "rec-fail")
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
            .AddArgument("", "send-invalid")
            .AddText("Send the file: " + filename)
            .AddInputTextBox("receiver", "Receiver")
            .AddInputTextBox("message", "Message (optional)")
            .AddButton("Send", ToastActivationType.Background, "send-success")
            .AddButton("Cancel", ToastActivationType.Background, "send-fail")
            .SetToastScenario(ToastScenario.Reminder)
            .Show();
        }

        /// <summary>
        /// Handles all the Dataway Toast responses
        /// </summary>
        public static void HandleToastResponses()
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                //Parse result
                string rawResult = toastArgs.Argument.Replace("=", "");
                string type = rawResult.Split('-')[0];
                string result = rawResult.Split('-')[1];

                Console.WriteLine("Type:" + type);
                Console.WriteLine("Answer:" + result);

                if (type == "rec")
                {
                    if (result == "invalid") return; //TODO: do something
                }
                else if (type == "send")
                {
                    if (result == "invalid") return; //TODO: do something

                    string receiver = (string)toastArgs.UserInput["receiver"];
                    string message = (string)toastArgs.UserInput["message"];
                    Console.WriteLine("Receiver: " + receiver);
                    Console.WriteLine("Message: " + message);
                }
            };
        }
    }
}

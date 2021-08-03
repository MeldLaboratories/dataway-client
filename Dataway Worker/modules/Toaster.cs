using System;
using Microsoft.Toolkit.Uwp.Notifications;

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

        /// <summary>
        /// Handles all the Dataway Toast responses
        /// </summary>
        public static void HandleToastResponses(Client client)
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                //Parse result
                string rawResult = toastArgs.Argument.Replace("=", "");
                if (!rawResult.StartsWith("dw")) return;
                string type = rawResult.Split('-')[1];
                string result = rawResult.Split('-')[2];

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
            };
        }
    }
}
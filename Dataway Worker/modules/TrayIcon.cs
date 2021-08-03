using System;
using System.Threading;
using System.Windows.Forms;

namespace Dataway_Worker
{
    internal class TrayIcon
    {
        public bool Muted = false;

        public TrayIcon()
        {
            Thread notifyIcon = new Thread(delegate ()
            {
                NotifyIcon Icon = new NotifyIcon();
                Icon = new NotifyIcon()
                {
                    Icon = Properties.Resources.app,
                    Visible = true,
                    Text = "Dataway"
                };

                // Exit Button
                var exit = new MenuItem("Exit");
                exit.Click += this.MenuExitHandler;

                // Add entries to menu
                var menu = new ContextMenu();
                menu.MenuItems.AddRange(new MenuItem[] { exit });
                Icon.ContextMenu = menu;

                Application.Run();
            });

            notifyIcon.SetApartmentState(ApartmentState.STA);
            notifyIcon.IsBackground = false;
            notifyIcon.Start();
        }

        private void MenuExitHandler(object sender, EventArgs args)
        {
            Environment.Exit(0);
        }
    }
}
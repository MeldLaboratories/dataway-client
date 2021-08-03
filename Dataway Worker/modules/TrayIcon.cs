using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Dataway_Worker
{
    class TrayIcon
    {
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
                exit.Click += (object sender, EventArgs args) => { Environment.Exit(0); };

                // context
                var menu = new ContextMenu();
                menu.MenuItems.AddRange(new MenuItem[] { exit });
                Icon.ContextMenu = menu;

                Application.Run();
            });

            notifyIcon.SetApartmentState(ApartmentState.STA);
            notifyIcon.IsBackground = false;
            notifyIcon.Start();
        }
    }
}

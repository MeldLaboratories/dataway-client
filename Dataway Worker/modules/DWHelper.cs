using Microsoft.Win32;
using System.Threading;
using System.Windows.Forms;

namespace Dataway_Worker.modules
{
    class DWHelper
    {
        /// <summary>
        /// Creates all the necesary registry keys for the Dataway context menu.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="executablePath"></param>
        public static void CreateBaseRegistryKeys(string folderName, string executablePath)
        {
            //Create Dataway Folder-Key and set properties
            RegistryKey shell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true);

            shell.CreateSubKey(folderName);

            RegistryKey dataway = shell.OpenSubKey(folderName, true);
            dataway.SetValue("ExtendedSubCommandsKey", "*\\shell\\" + folderName);
            dataway.SetValue("icon", executablePath + ", 0");

            //Create base command (send to...)
            RegistryKey datawayShell = dataway.CreateSubKey("shell", true);
            RegistryKey baseCMD = datawayShell.CreateSubKey("Send to...", true);
            baseCMD.CreateSubKey("command").SetValue("", executablePath + " context %1");
        }


        /// <summary>
        /// Deletes all Dataway related registry keys.
        /// </summary>
        /// <param name="folderName"></param>
        public static void RemoveAllRegistryKeys(string folderName)
        {
            RegistryKey shell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true);

            shell.DeleteSubKeyTree(folderName);
        }



        /// <summary>
        /// Adds a quick send entry to the context menu
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="user"></param>
        /// <param name="executablePath"></param>
        public static void AddQuickSendRegistryKey(string folderName, string user, string executablePath)
        {
            RegistryKey datawayShell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true)
                .OpenSubKey(folderName, true)
                .OpenSubKey("shell", true);

            RegistryKey cmdKey = datawayShell.CreateSubKey("Send to " + user, true);
            cmdKey.CreateSubKey("command", true).SetValue("", executablePath + " context -q %1 " + user);
        }



        /// <summary>
        /// Adds a quick send entry from the context menu
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="user"></param>
        /// <param name="executablePath"></param>
        public static void RemoveQuickSendRegistryKey(string folderName, string user)
        {
            RegistryKey datawayShell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true)
                .OpenSubKey(folderName, true)
                .OpenSubKey("shell", true);

            datawayShell.DeleteSubKeyTree("Send to " + user);
        }
        /// <summary>
        /// Saves a byte array via the Save-File-Dialog
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        public static void SaveBytesViaDialog(byte[] bytes, string fileName)
        {
            Thread thread = new Thread(() => {

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                string filetype = fileName.Split('.')[fileName.Split('.').Length - 1];

                saveFileDialog.Filter = filetype + " (*." + filetype + ")|*." + filetype;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = fileName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, bytes); //Maybe force filetype
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}


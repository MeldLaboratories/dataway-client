using Microsoft.Win32;
using System;
using System.Security;
using System.Collections.Generic;

namespace Dataway_Client
{
    internal class DWHelper
    {
        /// <summary>
        /// Creates all the necesary registry keys for the Dataway context menu.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="executablePath"></param>
        public static void CreateBaseRegistryKeys(string executablePath)
        {
            string folderName = Properties.Settings.Default.RegistryKeyName;

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
        public static void RemoveAllRegistryKeys()
        {
            string folderName = Properties.Settings.Default.RegistryKeyName;

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
        public static void AddQuickSendRegistryKey(string user, string executablePath)
        {
            string folderName = Properties.Settings.Default.RegistryKeyName;

            RegistryKey datawayShell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true)
                .OpenSubKey(folderName, true)
                .OpenSubKey("shell", true);

            RegistryKey cmdKey = datawayShell.CreateSubKey("Send to " + user, true);
            cmdKey.CreateSubKey("command", true).SetValue("", executablePath + " send -f %1 -u " + user);
        }



        /// <summary>
        /// Adds a quick send entry from the context menu
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="user"></param>
        /// <param name="executablePath"></param>
        public static void RemoveQuickSendRegistryKey(string user)
        {
            string folderName = Properties.Settings.Default.RegistryKeyName;

            try 
            {
                RegistryKey datawayShell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true)
                .OpenSubKey(folderName, true)
                .OpenSubKey("shell", true);

                datawayShell.DeleteSubKeyTree("Send to " + user);
            }

            catch (SecurityException)
            {
                Console.WriteLine("To add or remove a quick send entry, elevated privileges are required.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("There is no quick send entry of the user: " + user);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Dataway registry entry not found.");
            }
        }

        public static string[] ListAllQuickSendRegistryKeys()
        {
            string folderName = Properties.Settings.Default.RegistryKeyName;

            RegistryKey datawayShell = Registry.ClassesRoot
                .OpenSubKey("*", true)
                .OpenSubKey("shell", true)
                .OpenSubKey(folderName, true)
                .OpenSubKey("shell", true);

            List<string> quicks = new List<string>();
            quicks.AddRange(datawayShell.GetSubKeyNames());
            quicks.Remove("Send to...");
            return quicks.ToArray();
        }
    }
}

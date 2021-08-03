using Microsoft.Win32;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Dataway_Worker
{
    internal class DWHelper
    {
        /// <summary>
        /// Saves a byte array via the Save-File-Dialog
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        public static void SaveBytesViaDialog(byte[] bytes, string fileName)
        {
            Thread thread = new Thread(() =>
            {
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
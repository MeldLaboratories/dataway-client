using System.IO;
using System.Threading;
using System.Windows.Forms;

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

        public static void ShowErrorBox(string message)
        {
            var result = MessageBox.Show(message, "Unexpected Error!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Retry) { } //TODO restart code
        }
    }
}
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using OpenBots.Service.Client.Manager;
using System;
using System.IO;
using System.Linq;

namespace OpenBots.Service.Client.Executor
{
    public static class ProcessManager
    {
        public static string DownloadAndExtractProcess(string processId)
        {
            // Check if (Root) Processes Directory Exists
            var processesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Processes");
            if (!Directory.Exists(processesDirectory))
                Directory.CreateDirectory(processesDirectory);

            // Process Directory
            var processDirectoryPath = Path.Combine(processesDirectory, processId);

            // Download Process If it's not found
            if (!Directory.Exists(processDirectoryPath))
            {
                // Download Process by Id
                var apiResponse = ProcessesAPIManager.ExportProcess(AuthAPIManager.Instance, processId);

                // Create Process Directory named as Process Id
                Directory.CreateDirectory(processDirectoryPath);

                // Write Downloaded (.zip) file in the Process Directory
                var processZipFilePath = Path.Combine(processDirectoryPath, processId + ".zip");
                File.WriteAllBytes(processZipFilePath, apiResponse.Data.ToArray());

                // Extract Files/Folders from (.zip) file
                DecompressFile(processZipFilePath, processDirectoryPath);

                // Delete .zip File
                File.Delete(processZipFilePath);
            }

            string configFilePath = Directory.GetFiles(processDirectoryPath, "project.config", SearchOption.AllDirectories).First();
            string mainFileName = JObject.Parse(File.ReadAllText(configFilePath))["Main"].ToString();

            // Return "Main" Script File Path of the Process
            return Directory.GetFiles(processDirectoryPath, mainFileName, SearchOption.AllDirectories).First();
        }

        private static void DecompressFile(string processZipFilePath, string targetDirectory)
        {
            // Extract Files/Folders from downloaded (.zip) file
            FileStream fs = File.OpenRead(processZipFilePath);
            ZipFile file = new ZipFile(fs);

            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories
                    continue;
                }

                string entryFileName = zipEntry.Name;

                // 4K is optimum
                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                string fullZipToPath = Path.Combine(targetDirectory, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);

                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
            }

            if (file != null)
            {
                file.IsStreamOwner = true;
                file.Close();
            }
        }
    }
}

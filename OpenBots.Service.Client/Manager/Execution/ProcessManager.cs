using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager.API;
using System;
using System.IO;
using System.Linq;

namespace OpenBots.Service.Client.Manager.Execution
{
    public static class ProcessManager
    {
        public static string DownloadAndExtractProcess(Process process)
        {
            // Check if (Root) Processes Directory Exists (under User's AppData Folder), If Not create it
            var processesDirectory = Path.Combine(new EnvironmentSettings().GetEnvironmentVariable(), "Processes");
            if (!Directory.Exists(processesDirectory))
                Directory.CreateDirectory(processesDirectory);

            // Process Directory
            var processDirectoryPath = Path.Combine(processesDirectory, process.Id.ToString());

            // Create Process Directory named as Process Id If it doesn't exist
            if (!Directory.Exists(processDirectoryPath))
                Directory.CreateDirectory(processDirectoryPath);

            var processNugetFilePath = Path.Combine(processDirectoryPath, process.Name.ToString() + process.Version.ToString() + ".nuget");
            var processZipFilePath = Path.Combine(processDirectoryPath, process.Name.ToString() + process.Version.ToString() + ".zip");
            
            // Check if Process (.nuget) file exists if Not Download it
            if (!File.Exists(processNugetFilePath))
            {
                // Download Process by Id
                var apiResponse = ProcessesAPIManager.ExportProcess(AuthAPIManager.Instance, process.Id.ToString());

                // Write Downloaded(.nuget) file in the Process Directory
                File.WriteAllBytes(processNugetFilePath, apiResponse.Data.ToArray());
            }

            // Create .zip file if it doesn't exist
            if (!File.Exists(processZipFilePath))
                File.Copy(processNugetFilePath, processZipFilePath);

            var extractToDirectoryPath = Path.ChangeExtension(processZipFilePath, null);

            // Extract Files/Folders from (.zip) file
            DecompressFile(processZipFilePath, extractToDirectoryPath);

            // Delete .zip File
            File.Delete(processZipFilePath);

            string configFilePath = Directory.GetFiles(extractToDirectoryPath, "project.config", SearchOption.AllDirectories).First();
            string mainFileName = JObject.Parse(File.ReadAllText(configFilePath))["Main"].ToString();

            // Return "Main" Script File Path of the Process
            return Directory.GetFiles(extractToDirectoryPath, mainFileName, SearchOption.AllDirectories).First();
        }

        private static void DecompressFile(string processZipFilePath, string targetDirectory)
        {
            // Create Target Directory If it doesn't exist
            if(!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

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

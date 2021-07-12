using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using OpenBots.Agent.Core.Model;
using OpenBots.Server.SDK.HelperMethods;
using OpenBots.Server.SDK.Model;
using OpenBots.Service.Client.Manager.Common;
using System;
using System.IO;
using System.Linq;
using SystemIO = System.IO;

namespace OpenBots.Service.Client.Manager.Execution
{
    public static class AutomationManager
    {
        public static string DownloadAndExtractAutomation(AuthAPIManager authAPIManager, Automation automation, string jobId, string domainName, string userName, out string executionDirectoryPath, out string configFilePath)
        {
            configFilePath = "";
            executionDirectoryPath = "";

            // Check if (Root) Automations Directory Exists (under User's AppData Folder), If Not create it
            var automationsDirectory = Path.Combine(new EnvironmentSettings().GetEnvironmentVariablePath(domainName, userName), "Automations",
                automation.AutomationEngine);

            if (!Directory.Exists(automationsDirectory))
                Directory.CreateDirectory(automationsDirectory);

            // Automation Directory
            var processDirectoryPath = Path.Combine(automationsDirectory, automation.Id.ToString());
            

            // Create Automation Directory named as Automation Id If it doesn't exist
            if (!Directory.Exists(processDirectoryPath))
                Directory.CreateDirectory(processDirectoryPath);

            // Automation Nuget Package Path
            var processNugetFilePath = Path.Combine(processDirectoryPath, automation.Name.ToString() + ".nupkg");

            // Execution Directory Path
            executionDirectoryPath = Path.Combine(processDirectoryPath, string.IsNullOrEmpty(jobId) ? new Guid().ToString() : jobId);
            if (!Directory.Exists(executionDirectoryPath))
                Directory.CreateDirectory(executionDirectoryPath);

            var processZipFilePath = Path.Combine(executionDirectoryPath, automation.Name.ToString() + ".zip");

            // Check if Automation (.nupkg) file exists if Not Download it
            if (!File.Exists(processNugetFilePath))
            {
                // Download Automation by Id
                var apiResponse = AutomationMethods.ExportAutomation(authAPIManager.UserInfo, automation.Id.ToString());

                // Write Downloaded(.nupkg) file in the Automation Directory
                File.WriteAllBytes(processNugetFilePath, apiResponse.ToArray());
            }

            // Create .zip file if it doesn't exist
            if (!File.Exists(processZipFilePath))
                File.Copy(processNugetFilePath, processZipFilePath);

            var extractToDirectoryPath = Path.ChangeExtension(processZipFilePath, null);

            // Extract Files/Folders from (.zip) file
            DecompressFile(processZipFilePath, extractToDirectoryPath);

            // Delete .zip File
            File.Delete(processZipFilePath);

            configFilePath = Directory.GetFiles(extractToDirectoryPath, "project.obconfig", SearchOption.AllDirectories).First();
            string mainFileName = JObject.Parse(File.ReadAllText(configFilePath))["Main"].ToString();
            
            // Return "Main" Script File Path of the Automation
            return Directory.GetFiles(extractToDirectoryPath, mainFileName, SearchOption.AllDirectories).First();
        }

        private static void DecompressFile(string processZipFilePath, string targetDirectory)
        {
            // Create Target Directory If it doesn't exist
            if(!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            // Extract Files/Folders from downloaded (.zip) file
            SystemIO.FileStream fs = File.OpenRead(processZipFilePath);
            ZipFile file = new ZipFile(fs);

            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories but create them in case they're empty
                    Directory.CreateDirectory(Path.Combine(targetDirectory, Uri.UnescapeDataString(zipEntry.Name)));
                    continue;
                }

                string entryFileName = Uri.UnescapeDataString(zipEntry.Name);

                if (Path.GetExtension(entryFileName).Equals(".psmdcp"))
                    continue;

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
                using (SystemIO.FileStream streamWriter = File.Create(fullZipToPath))
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
            }

            if (file != null)
            {
                file.IsStreamOwner = true;
                file.Close();
            }
        }

        public static string GetMainScriptFilePath(string projectPackagePath, out string configFilePath)
        {
            var unpackagedProjectDir = UnpackageNugetProjectFile(projectPackagePath);

            configFilePath = Directory.GetFiles(unpackagedProjectDir, "project.obconfig", SearchOption.AllDirectories).First();
            var mainFileName = JObject.Parse(File.ReadAllText(configFilePath))["Main"].ToString();

            // Return "Main" Script File Path of the Automation
            return Directory.GetFiles(unpackagedProjectDir, mainFileName, SearchOption.AllDirectories).First();
        }

        private static string UnpackageNugetProjectFile(string processNugetFilePath)
        {
            var processZipFilePath = Path.ChangeExtension(processNugetFilePath, ".zip");

            // Create .zip file if it doesn't exist
            if (!File.Exists(processZipFilePath))
                File.Copy(processNugetFilePath, processZipFilePath);

            var extractToDirectoryPath = Path.ChangeExtension(processZipFilePath, null);

            // Extract Files/Folders from (.zip) file
            DecompressFile(processZipFilePath, extractToDirectoryPath);

            // Delete .zip File
            File.Delete(processZipFilePath);

            return extractToDirectoryPath;
        }
    }
}

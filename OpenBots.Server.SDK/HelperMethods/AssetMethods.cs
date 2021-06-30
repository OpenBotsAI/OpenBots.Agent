using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using System;
using IOFile = System.IO.File;
using SDKAsset = OpenBots.Server.SDK.Model.Asset;
using System.IO;
using System.Linq;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class AssetMethods
    {
        public static SDKAsset GetAsset(string token, string serverUrl, string organizationId, string assetName, string assetType, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                var asset = apiInstance.ApiVapiVersionAssetsGetAssetByNameAssetNameGetAsyncWithHttpInfo(assetName, apiVersion, organizationId, assetType).Result.Data;

                return asset;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.GetAssetByName: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void PutAsset(string token, string serverUrl, string organizationId, SDKAsset asset, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdPutAsyncWithHttpInfo(asset.Id.ToString(), apiVersion, organizationId, asset).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.UpdateAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DownloadFileAsset(string token, string serverUrl, string organizationId, SDKAsset asset, string directoryPath, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                var response = apiInstance.ExportAssetAsyncWithHttpInfo(asset.Id.ToString(), apiVersion, organizationId).Result;
                string value;
                var headers = response.Headers.TryGetValue("Content-Disposition", out value);
                string fileName;
                if (headers == true)
                {
                    string[] valueArray = value.Split('=');
                    string[] fileNameArray = valueArray[1].Split(';');
                    fileName = fileNameArray[0];
                }
                else
                {
                    var fileId = asset.FileId;
                    var fileApiInstance = new FilesApi(serverUrl);
                    fileApiInstance.Configuration.AccessToken = token;
                    var driveApiInstance = new DrivesApi(serverUrl);
                    driveApiInstance.Configuration.AccessToken = token;
                    string filter = "IsDefault eq true";
                    var driveResponse = driveApiInstance.ApiVapiVersionStorageDrivesGetAsyncWithHttpInfo(apiVersion, organizationId, filter).Result.Data.Items.FirstOrDefault();
                    var fileResponse = fileApiInstance.GetFileFolderAsyncWithHttpInfo(asset.FileId.ToString(), apiVersion, organizationId, driveResponse.Id.ToString()).Result.Data;
                    fileName = fileResponse.Name;
                }
                MemoryStream data = response.Data;
                byte[] file = data.ToArray();
                IOFile.WriteAllBytes(Path.Combine(directoryPath, fileName), file);
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.ExportAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void UpdateFileAsset(string token, string serverUrl, string organizationId, SDKAsset asset, string filePath, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                using (FileStream _file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    apiInstance.ApiVapiVersionAssetsIdUpdatePutAsyncWithHttpInfo(asset.Id.ToString(), apiVersion, organizationId, asset.Name, asset.Type, asset.FileId.Value, _file).Wait();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.UpdateFileAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void AppendAsset(string token, string serverUrl, string organizationId, Guid? assetId, string appendText, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdAppendPutAsyncWithHttpInfo(assetId.ToString(), apiVersion, organizationId, appendText).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.AppendAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void IncrementAsset(string token, string serverUrl, string organizationId, Guid? assetId, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdIncrementPutAsyncWithHttpInfo(assetId.ToString(), apiVersion, organizationId).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.IncrementAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DecrementAsset(string token, string serverUrl, string organizationId, Guid? assetId, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdDecrementPutAsyncWithHttpInfo(assetId.ToString(), apiVersion, organizationId).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.DecrementAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void AddAsset(string token, string serverUrl, string organizationId, Guid? assetId, int value, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdAddPutAsyncWithHttpInfo(assetId.ToString(), apiVersion, organizationId, value).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.AddAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void SubtractAsset(string token, string serverUrl, string organizationId, Guid? assetId, int value, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdSubtractPutAsyncWithHttpInfo(assetId.ToString(), apiVersion, organizationId, value).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.SubtractAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        private static AssetsApi GetApiInstance(string token, string serverUrl)
        {
            var apiInstance = new AssetsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;
            return apiInstance;
        }
    }
}

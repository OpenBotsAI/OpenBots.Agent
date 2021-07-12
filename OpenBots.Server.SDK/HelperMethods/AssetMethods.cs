using OpenBots.Server.SDK.Api;
using System;
using IOFile = System.IO.File;
using SDKAsset = OpenBots.Server.SDK.Model.Asset;
using System.IO;
using System.Linq;
using OpenBots.Server.SDK.Model;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class AssetMethods
    {
        public static SDKAsset GetAsset(UserInfo userInfo, string assetName, string assetType, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var asset = apiInstance.ApiVapiVersionAssetsGetAssetByNameAssetNameGetAsyncWithHttpInfo(assetName, userInfo.ApiVersion, userInfo.OrganizationId, assetType).Result.Data;
                return asset;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetAsset(userInfo, assetName, assetType, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.GetAssetByName: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void PutAsset(UserInfo userInfo, SDKAsset asset, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdPutAsyncWithHttpInfo(asset.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, asset).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    PutAsset(userInfo, asset, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.UpdateAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DownloadFileAsset(UserInfo userInfo, SDKAsset asset, string directoryPath, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var response = apiInstance.ExportAssetAsyncWithHttpInfo(asset.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId).Result;
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
                    var fileApiInstance = new FilesApi(userInfo.ServerUrl);
                    fileApiInstance.Configuration.AccessToken = userInfo.Token;
                    var driveApiInstance = new DrivesApi(userInfo.ServerUrl);
                    driveApiInstance.Configuration.AccessToken = userInfo.Token;
                    string filter = "IsDefault eq true";
                    var driveResponse = driveApiInstance.ApiVapiVersionStorageDrivesGetAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, filter).Result.Data.Items.FirstOrDefault();
                    var fileResponse = fileApiInstance.GetFileFolderAsyncWithHttpInfo(asset.FileId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, driveResponse.Id.ToString()).Result.Data;
                    fileName = fileResponse.Name;
                }
                System.IO.MemoryStream data = response.Data;
                byte[] file = data.ToArray();
                IOFile.WriteAllBytes(Path.Combine(directoryPath, fileName), file);
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    DownloadFileAsset(userInfo, asset, directoryPath, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.ExportAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void UpdateFileAsset(UserInfo userInfo, SDKAsset asset, string filePath, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                using (System.IO.FileStream _file = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    apiInstance.ApiVapiVersionAssetsIdUpdatePutAsyncWithHttpInfo(asset.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, asset.Name, asset.Type, asset.FileId.Value, _file).Wait();
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    UpdateFileAsset(userInfo, asset, filePath, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.UpdateFileAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void AppendAsset(UserInfo userInfo, Guid? assetId, string appendText, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdAppendPutAsyncWithHttpInfo(assetId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, appendText).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    AppendAsset(userInfo, assetId, appendText, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.AppendAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void IncrementAsset(UserInfo userInfo, Guid? assetId, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdIncrementPutAsyncWithHttpInfo(assetId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    IncrementAsset(userInfo, assetId, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.IncrementAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DecrementAsset(UserInfo userInfo, Guid? assetId, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdDecrementPutAsyncWithHttpInfo(assetId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    DecrementAsset(userInfo, assetId, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.DecrementAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void AddAsset(UserInfo userInfo, Guid? assetId, int value, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdAddPutAsyncWithHttpInfo(assetId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, value).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    AddAsset(userInfo, assetId, value, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AssetsApi.AddAsset: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void SubtractAsset(UserInfo userInfo, Guid? assetId, int value, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionAssetsIdSubtractPutAsyncWithHttpInfo(assetId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, value).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    SubtractAsset(userInfo, assetId, value, count);
                }
                else if (ex.Message != "One or more errors occurred.")
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

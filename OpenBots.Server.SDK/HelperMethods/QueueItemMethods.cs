using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using System;
using System.Collections.Generic;
using SystemFile = System.IO;
using System.Linq;
using OpenBots.Server.SDK.Model;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class QueueItemMethods
    {
        public static QueueItem GetQueueItemById(UserInfo userInfo, Guid? id, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var result = apiInstance.GetQueueItemAsyncWithHttpInfo(id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId).Result.Data;
                string queueItemString = JsonConvert.SerializeObject(result);
                var queueItem = JsonConvert.DeserializeObject<QueueItem>(queueItemString);
                return queueItem;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetQueueItemById(userInfo, id, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.GetQueueItem: " + ex.Message);
                else if (ex.InnerException.Message == "Error calling DequeueQueueItem with status BadRequest: Entity Does Not Exist. No item to dequeue from list of queue items")
                    return null;
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static QueueItem GetQueueItemByLockTransactionKey(UserInfo userInfo, string transactionKey, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                string filter = $"LockTransactionKey eq guid'{transactionKey}'";
                var result = apiInstance.ApiVapiVersionQueueItemsGetAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, filter).Result.Data.Items.FirstOrDefault();
                string queueItemString = JsonConvert.SerializeObject(result);
                var queueItem = JsonConvert.DeserializeObject<QueueItem>(queueItemString);
                return queueItem;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetQueueItemByLockTransactionKey(userInfo, transactionKey, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.GetQueueItems: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void EnqueueQueueItem(UserInfo userInfo, QueueItem queueItem, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var queueItemString = JsonConvert.SerializeObject(queueItem);
                var queueItemSDK = JsonConvert.DeserializeObject<QueueItem>(queueItemString);
                apiInstance.ApiVapiVersionQueueItemsEnqueuePostAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, queueItemSDK).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    EnqueueQueueItem(userInfo, queueItem, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.EnqueueQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void AttachFiles(UserInfo userInfo, Guid? queueItemId, List<string> attachments, int count = 0)
        {
            var apiInstance = GetAttachmentsApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                if (attachments != null && attachments.Count > 0)
                {
                    List<SystemFile.FileStream> attachmentsList = new List<SystemFile.FileStream>();
                    foreach (var attachment in attachments)
                    {
                        SystemFile.FileStream _file = new SystemFile.FileStream(attachment, SystemFile.FileMode.Open, SystemFile.FileAccess.Read);
                        attachmentsList.Add(_file);
                    }
                    apiInstance.ApiVapiVersionQueueItemsQueueItemIdQueueItemAttachmentsPostAsyncWithHttpInfo(queueItemId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, attachmentsList).Wait();

                    foreach (var file in attachmentsList)
                    {
                        file.Flush();
                        file.Dispose();
                        file.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    AttachFiles(userInfo, queueItemId, attachments, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemAttachmentsApi.AttachFiles: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static QueueItem DequeueQueueItem(UserInfo userInfo, Guid? queueId, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var result = apiInstance.ApiVapiVersionQueueItemsDequeueGetAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, userInfo.AgentId, queueId.ToString()).Result.Data;
                var queueItemString = JsonConvert.SerializeObject(result);
                var queueItem = JsonConvert.DeserializeObject<QueueItem>(queueItemString);
                return queueItem;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return DequeueQueueItem(userInfo, queueId, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.DequeueQueueItem: " + ex.Message);
                else if (ex.InnerException.Message == "Error calling DequeueQueueItem with status BadRequest: Entity Does Not Exist. No item to dequeue from list of queue items")
                    return null;
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void CommitQueueItem(UserInfo userInfo, Guid transactionKey, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionQueueItemsCommitPutAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, transactionKey.ToString()).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    CommitQueueItem(userInfo, transactionKey, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.CommitQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void RollbackQueueItem(UserInfo userInfo, Guid transactionKey, string code, string error, bool isFatal, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionQueueItemsRollbackPutAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, transactionKey.ToString(), code, error, isFatal).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    RollbackQueueItem(userInfo, transactionKey, code, error, isFatal, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.RollbackQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void ExtendQueueItem(UserInfo userInfo, Guid transactionKey, int count = 0)
        {
            var apiInstance = GetQueueItemApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionQueueItemsExtendPutAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, transactionKey.ToString()).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    ExtendQueueItem(userInfo, transactionKey, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.ExtendQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static List<QueueItemAttachment> GetAttachments(UserInfo userInfo, Guid? queueItemId, int count = 0)
        {
            var apiInstance = GetAttachmentsApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var attachments = apiInstance.ApiVapiVersionQueueItemsQueueItemIdQueueItemAttachmentsGetAsyncWithHttpInfo(queueItemId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId).Result.Data.Items;
                var listString = JsonConvert.SerializeObject(attachments);
                var attachmentsList = JsonConvert.DeserializeObject<List<QueueItemAttachment>>(listString);

                return attachmentsList;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetAttachments(userInfo, queueItemId, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemAttachmentsApi.GetQueueItemAttachments: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DownloadFile(UserInfo userInfo, QueueItemAttachment attachment, string directoryPath, int count = 0)
        {
            var apiInstance = GetAttachmentsApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var response = apiInstance.ExportQueueItemAttachmentAsyncWithHttpInfo(attachment.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, attachment.QueueItemId.ToString()).Result;
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
                    var fileId = attachment.FileId;
                    var fileApiInstance = new FilesApi(userInfo.ServerUrl);
                    fileApiInstance.Configuration.AccessToken = userInfo.Token;
                    var driveApiInstance = new DrivesApi(userInfo.ServerUrl);
                    driveApiInstance.Configuration.AccessToken = userInfo.Token;
                    string filter = "IsDefault eq true";
                    var driveResponse = driveApiInstance.ApiVapiVersionStorageDrivesGetAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, filter).Result.Data.Items.FirstOrDefault();
                    var fileResponse = fileApiInstance.GetFileFolderAsyncWithHttpInfo(attachment.FileId.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, driveResponse.Id.ToString()).Result.Data;
                    fileName = fileResponse.Name;
                }
                var data = response.Data;
                byte[] fileArray = data.ToArray();
                System.IO.File.WriteAllBytes(SystemFile.Path.Combine(directoryPath, fileName), fileArray);
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    DownloadFile(userInfo, attachment, directoryPath, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemAttachmentsApi.ExportQueueItemAttachment: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        private static QueueItemsApi GetQueueItemApiInstance(string token, string serverUrl)
        {
            var apiInstance = new QueueItemsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            return apiInstance;
        }

        private static QueueItemAttachmentsApi GetAttachmentsApiInstance(string token, string serverUrl)
        {
            var apiInstance = new QueueItemAttachmentsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            return apiInstance;
        }
    }
}

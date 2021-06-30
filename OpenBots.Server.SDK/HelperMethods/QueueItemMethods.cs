using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using System;
using System.Collections.Generic;
using SystemFile = System.IO;
using SDKQueueItem = OpenBots.Server.SDK.Model.QueueItem;
using System.Linq;
using OpenBots.Server.SDK.Model;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class QueueItemMethods
    {
        public static QueueItem GetQueueItemById(string token, string serverUrl, string organizationId, Guid? id, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                var result = apiInstance.GetQueueItemAsyncWithHttpInfo(id.ToString(), apiVersion, organizationId).Result.Data;
                string queueItemString = JsonConvert.SerializeObject(result);
                var queueItem = JsonConvert.DeserializeObject<SDKQueueItem>(queueItemString);
                return queueItem;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.GetQueueItem: " + ex.Message);
                else if (ex.InnerException.Message == "Error calling DequeueQueueItem with status BadRequest: Entity Does Not Exist. No item to dequeue from list of queue items")
                    return null;
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static SDKQueueItem GetQueueItemByLockTransactionKey(string token, string serverUrl, string organizationId, string transactionKey, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                string filter = $"LockTransactionKey eq guid'{transactionKey}'";
                var result = apiInstance.ApiVapiVersionQueueItemsGetAsyncWithHttpInfo(apiVersion, organizationId, filter).Result.Data.Items.FirstOrDefault();
                string queueItemString = JsonConvert.SerializeObject(result);
                var queueItem = JsonConvert.DeserializeObject<SDKQueueItem>(queueItemString);
                return queueItem;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.GetQueueItems: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void EnqueueQueueItem(string token, string serverUrl, string organizationId, SDKQueueItem queueItem, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                var queueItemString = JsonConvert.SerializeObject(queueItem);
                var queueItemSDK = JsonConvert.DeserializeObject<SDKQueueItem>(queueItemString);
                apiInstance.ApiVapiVersionQueueItemsEnqueuePostAsyncWithHttpInfo(apiVersion, organizationId, queueItemSDK).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.EnqueueQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void AttachFiles(string token, string serverUrl, string organizationId, Guid? queueItemId, List<string> attachments, string apiVersion)
        {
            var apiInstance = GetAttachmentsApiInstance(token, serverUrl);

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
                    apiInstance.ApiVapiVersionQueueItemsQueueItemIdQueueItemAttachmentsPostAsyncWithHttpInfo(queueItemId.ToString(), apiVersion, organizationId, attachmentsList).Wait();

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
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemAttachmentsApi.AttachFiles: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static SDKQueueItem DequeueQueueItem(string token, string serverUrl, string organizationId, string agentId, Guid? queueId, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                var result = apiInstance.ApiVapiVersionQueueItemsDequeueGetAsyncWithHttpInfo(apiVersion, organizationId, agentId, queueId.ToString()).Result.Data;
                //may have to map here since result returns a view model
                var queueItemString = JsonConvert.SerializeObject(result);
                var queueItem = JsonConvert.DeserializeObject<SDKQueueItem>(queueItemString);
                return queueItem;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.DequeueQueueItem: " + ex.Message);
                else if (ex.InnerException.Message == "Error calling DequeueQueueItem with status BadRequest: Entity Does Not Exist. No item to dequeue from list of queue items")
                    return null;
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void CommitQueueItem(string token, string serverUrl, string organizationId, Guid transactionKey, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionQueueItemsCommitPutAsyncWithHttpInfo(apiVersion, organizationId, transactionKey.ToString()).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.CommitQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void RollbackQueueItem(string token, string serverUrl, string organizationId, Guid transactionKey, string code, string error, bool isFatal, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionQueueItemsRollbackPutAsyncWithHttpInfo(apiVersion, organizationId, transactionKey.ToString(), code, error, isFatal).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.RollbackQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void ExtendQueueItem(string token, string serverUrl, string organizationId, Guid transactionKey, string apiVersion)
        {
            var apiInstance = GetQueueItemApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionQueueItemsExtendPutAsyncWithHttpInfo(apiVersion, organizationId, transactionKey.ToString()).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemsApi.ExtendQueueItem: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static List<QueueItemAttachment> GetAttachments(string token, string serverUrl, string organizationId, Guid? queueItemId, string apiVersion)
        {
            var apiInstance = GetAttachmentsApiInstance(token, serverUrl);

            try
            {
                var attachments = apiInstance.ApiVapiVersionQueueItemsQueueItemIdQueueItemAttachmentsGetAsyncWithHttpInfo(queueItemId.ToString(), apiVersion, organizationId).Result.Data.Items;
                var listString = JsonConvert.SerializeObject(attachments);
                var attachmentsList = JsonConvert.DeserializeObject<List<QueueItemAttachment>>(listString);

                return attachmentsList;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueueItemAttachmentsApi.GetQueueItemAttachments: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DownloadFile(string token, string serverUrl, string organizationId, QueueItemAttachment attachment, string directoryPath, string apiVersion)
        {
            var apiInstance = GetAttachmentsApiInstance(token, serverUrl);

            try
            {
                var response = apiInstance.ExportQueueItemAttachmentAsyncWithHttpInfo(attachment.Id.ToString(), apiVersion, organizationId, attachment.QueueItemId.ToString()).Result;
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
                    var fileApiInstance = new FilesApi(serverUrl);
                    fileApiInstance.Configuration.AccessToken = token;
                    var driveApiInstance = new DrivesApi(serverUrl);
                    driveApiInstance.Configuration.AccessToken = token;
                    string filter = "IsDefault eq true";
                    var driveResponse = driveApiInstance.ApiVapiVersionStorageDrivesGetAsyncWithHttpInfo(apiVersion, organizationId, filter).Result.Data.Items.FirstOrDefault();
                    var fileResponse = fileApiInstance.GetFileFolderAsyncWithHttpInfo(attachment.FileId.ToString(), apiVersion, organizationId, driveResponse.Id.ToString()).Result.Data;
                    fileName = fileResponse.Name;
                }
                var data = response.Data;
                byte[] fileArray = data.ToArray();
                System.IO.File.WriteAllBytes(SystemFile.Path.Combine(directoryPath, fileName), fileArray);
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
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

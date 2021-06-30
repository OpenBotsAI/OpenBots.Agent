using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class DocumentMethods
    {
        public static void ChangeStatus(UserInfo userInfo, Guid humanTaskId, string status)
        {
            var apiInstance = GetDocumentsApiInstance(userInfo.Token, userInfo.DocumentsUrl);

            try
            {
                apiInstance.ApiServicesAppDocumentprocessingengineserviceChangestatusPostWithHttpInfo(humanTaskId, status);
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling DocumentProcessingEngineServiceApi.ChangeStatus: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static DataTable CompareTables(List<string> ignoreColumns, List<string> lookupColumns, DataTable expected, DataTable actual)
        {
            TableComparisonManager tcm = new TableComparisonManager();
            tcm.IgnoreColumns = string.Join(",", ignoreColumns);
            tcm.LookupColumns = string.Join(",", lookupColumns);
            tcm.Compare(expected, actual);
            DataTable diff = tcm.Differences;
            return diff;
        }

        public static DocumentStatus GetDocumentStatus(UserInfo userInfo, Guid humanTaskId)
        {
            var apiInstance = GetDocumentsApiInstance(userInfo.Token, userInfo.DocumentsUrl);

            try
            {
                var docStatus = apiInstance.GetDocumentStatus(humanTaskId.ToString());
                return docStatus;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling DocumentProcessingEngineServiceApi.GetDocumentStatus: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static DocumentStatus AwaitProcessing(UserInfo userInfo, Guid humanTaskId, int timeoutInSeconds = 120)
        {
            bool continueTrying = true;
            DateTime startTime = DateTime.Now;
            DocumentStatus docStatus = new DocumentStatus();
            string status = "";
            while (continueTrying)
            {
                docStatus = GetDocumentStatus(userInfo, humanTaskId);
                status = docStatus.Status;

                if (status == "Processed" || status == "CompletedWithError" || DateTime.Now.Subtract(startTime).TotalSeconds >= timeoutInSeconds)
                {
                    continueTrying = false;
                    break;
                }
                else
                {
                    Task.Delay(TimeSpan.FromSeconds(20)).Wait();
                }
            }

            return docStatus;
        }

        public static void MarkDocumentAsVerified(UserInfo userInfo, Guid humanTaskId, Guid docId)
        {
            var apiInstance = GetDocumentsApiInstance(userInfo.Token, userInfo.DocumentsUrl);

            try
            {
                apiInstance.ApiServicesAppDocumentprocessingengineserviceMarkdocumentasverifiedPostWithHttpInfo(humanTaskId, docId);
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling DocumentProcessingEngineServiceApi.MarkDocumentAsVerified: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static DocumentResult SaveDocumentResults(UserInfo userInfo, Guid humanTaskId, bool awaitCompletion, bool savePageImages, bool savePageText,
            int timeout, string outputFolder, DataTable dataTable)
        {
            var apiInstance = GetDocumentsApiInstance(userInfo.Token, userInfo.DocumentsUrl);

            try
            {
                var result = apiInstance.SaveDocumentResults(humanTaskId, awaitCompletion, savePageImages, savePageText, timeout, outputFolder,
                    dataTable);
                var resultString = JsonConvert.SerializeObject(result);
                var docResult = JsonConvert.DeserializeObject<DocumentResult>(resultString);
                return docResult;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling DocumentProcessingEngineServiceApi.SaveDocumentResults: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static Dictionary<string, string> SubmitDocument(UserInfo userInfo, string fileToProcess, string taskQueueName, string name, string description, string caseType,
            string caseNumber, string assignedTo, DateTime dueOn)
        {
            var apiInstance = GetDocumentsApiInstance(userInfo.Token, userInfo.DocumentsUrl);

            try
            {
                var result = apiInstance.SubmitDocument(fileToProcess, taskQueueName, name, description, caseNumber, caseType, assignedTo, dueOn);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling DocumentProcessingEngineServiceApi.SubmitDocument: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static DocumentProcessingEngineServiceApi GetDocumentsApiInstance(string token, string documentsUrl)
        {
            var apiInstance = new DocumentProcessingEngineServiceApi(documentsUrl);
            apiInstance.Configuration.AccessToken = token;
            apiInstance.Configuration.BasePath = documentsUrl;

            return apiInstance;
        }
    }
}

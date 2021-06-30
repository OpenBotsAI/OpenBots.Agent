using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;
using System.Collections.Generic;
using SystemFile = System.IO;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class ServerEmailMethods
    {
        public static void SendServerEmail(string token, string serverUrl, string organizationId, EmailMessage emailMessage, List<string> attachments, string accountName, string apiVersion)
        {
            var apiInstance = new EmailsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            try
            {
                var emailMessageJson = JsonConvert.SerializeObject(emailMessage);

                if (attachments != null)
                {
                    List<SystemFile.FileStream> attachmentsList = new List<SystemFile.FileStream>();

                    foreach (var attachment in attachments)
                    {
                        SystemFile.FileStream _file = new SystemFile.FileStream(attachment, SystemFile.FileMode.Open, SystemFile.FileAccess.Read);
                        attachmentsList.Add(_file);
                    }

                    apiInstance.ApiVapiVersionEmailsSendPostAsyncWithHttpInfo(apiVersion, organizationId, emailMessageJson, attachmentsList, accountName).Wait();

                    foreach (var file in attachmentsList)
                    {
                        file.Flush();
                        file.Dispose();
                        file.Close();
                    }
                }
                else
                    apiInstance.ApiVapiVersionEmailsSendPostAsyncWithHttpInfo(apiVersion, organizationId, emailMessageJson, null, accountName).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling EmailsApi.SendEmail: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }
    }
}

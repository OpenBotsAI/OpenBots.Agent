/* 
 * OpenBots Server API
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp;
using OpenBots.Server.SDK.Client;

namespace OpenBots.Server.SDK.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public interface IAppApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Application version
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>string</returns>
        string ApplicationApplicationVersionGet (string application);

        /// <summary>
        /// Application version
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>ApiResponse of string</returns>
        ApiResponse<string> ApplicationApplicationVersionGetWithHttpInfo (string application);
        /// <summary>
        /// Minor version release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>string</returns>
        string ApplicationApplicationVersionMinorReleasePut (string application, string key = null);

        /// <summary>
        /// Minor version release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>ApiResponse of string</returns>
        ApiResponse<string> ApplicationApplicationVersionMinorReleasePutWithHttpInfo (string application, string key = null);
        /// <summary>
        /// Patch release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>string</returns>
        string ApplicationApplicationVersionPatchReleasePut (string application, string key = null);

        /// <summary>
        /// Patch release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>ApiResponse of string</returns>
        ApiResponse<string> ApplicationApplicationVersionPatchReleasePutWithHttpInfo (string application, string key = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Application version
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>Task of string</returns>
        System.Threading.Tasks.Task<string> ApplicationApplicationVersionGetAsync (string application);

        /// <summary>
        /// Application version
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>Task of ApiResponse (string)</returns>
        System.Threading.Tasks.Task<ApiResponse<string>> ApplicationApplicationVersionGetAsyncWithHttpInfo (string application);
        /// <summary>
        /// Minor version release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of string</returns>
        System.Threading.Tasks.Task<string> ApplicationApplicationVersionMinorReleasePutAsync (string application, string key = null);

        /// <summary>
        /// Minor version release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of ApiResponse (string)</returns>
        System.Threading.Tasks.Task<ApiResponse<string>> ApplicationApplicationVersionMinorReleasePutAsyncWithHttpInfo (string application, string key = null);
        /// <summary>
        /// Patch release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of string</returns>
        System.Threading.Tasks.Task<string> ApplicationApplicationVersionPatchReleasePutAsync (string application, string key = null);

        /// <summary>
        /// Patch release
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of ApiResponse (string)</returns>
        System.Threading.Tasks.Task<ApiResponse<string>> ApplicationApplicationVersionPatchReleasePutAsyncWithHttpInfo (string application, string key = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class AppApi : IAppApi
    {
        private OpenBots.Server.SDK.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AppApi(String basePath)
        {
            this.Configuration = new OpenBots.Server.SDK.Client.Configuration { BasePath = basePath };

            ExceptionFactory = OpenBots.Server.SDK.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppApi"/> class
        /// </summary>
        /// <returns></returns>
        public AppApi()
        {
            this.Configuration = OpenBots.Server.SDK.Client.Configuration.Default;

            ExceptionFactory = OpenBots.Server.SDK.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public AppApi(OpenBots.Server.SDK.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = OpenBots.Server.SDK.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = OpenBots.Server.SDK.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public OpenBots.Server.SDK.Client.Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public OpenBots.Server.SDK.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// Application version 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>string</returns>
        public string ApplicationApplicationVersionGet (string application)
        {
             ApiResponse<string> localVarResponse = ApplicationApplicationVersionGetWithHttpInfo(application);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Application version 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>ApiResponse of string</returns>
        public ApiResponse< string > ApplicationApplicationVersionGetWithHttpInfo (string application)
        {
            // verify the required parameter 'application' is set
            if (application == null)
                throw new ApiException(400, "Missing required parameter 'application' when calling AppApi->ApplicationApplicationVersionGet");

            var localVarPath = "/Application/{application}/Version";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (application != null) localVarPathParams.Add("application", this.Configuration.ApiClient.ParameterToString(application)); // path parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ApplicationApplicationVersionGet", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<string>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => string.Join(",", x.Value)),
                (string) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(string)));
        }

        /// <summary>
        /// Application version 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>Task of string</returns>
        public async System.Threading.Tasks.Task<string> ApplicationApplicationVersionGetAsync (string application)
        {
             ApiResponse<string> localVarResponse = await ApplicationApplicationVersionGetAsyncWithHttpInfo(application);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Application version 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <returns>Task of ApiResponse (string)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<string>> ApplicationApplicationVersionGetAsyncWithHttpInfo (string application)
        {
            // verify the required parameter 'application' is set
            if (application == null)
                throw new ApiException(400, "Missing required parameter 'application' when calling AppApi->ApplicationApplicationVersionGet");

            var localVarPath = "/Application/{application}/Version";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (application != null) localVarPathParams.Add("application", this.Configuration.ApiClient.ParameterToString(application)); // path parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ApplicationApplicationVersionGet", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<string>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => string.Join(",", x.Value)),
                (string) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(string)));
        }

        /// <summary>
        /// Minor version release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>string</returns>
        public string ApplicationApplicationVersionMinorReleasePut (string application, string key = null)
        {
             ApiResponse<string> localVarResponse = ApplicationApplicationVersionMinorReleasePutWithHttpInfo(application, key);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Minor version release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>ApiResponse of string</returns>
        public ApiResponse< string > ApplicationApplicationVersionMinorReleasePutWithHttpInfo (string application, string key = null)
        {
            // verify the required parameter 'application' is set
            if (application == null)
                throw new ApiException(400, "Missing required parameter 'application' when calling AppApi->ApplicationApplicationVersionMinorReleasePut");

            var localVarPath = "/Application/{application}/Version/Minor/Release";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (application != null) localVarPathParams.Add("application", this.Configuration.ApiClient.ParameterToString(application)); // path parameter
            if (key != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "key", key)); // query parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ApplicationApplicationVersionMinorReleasePut", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<string>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => string.Join(",", x.Value)),
                (string) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(string)));
        }

        /// <summary>
        /// Minor version release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of string</returns>
        public async System.Threading.Tasks.Task<string> ApplicationApplicationVersionMinorReleasePutAsync (string application, string key = null)
        {
             ApiResponse<string> localVarResponse = await ApplicationApplicationVersionMinorReleasePutAsyncWithHttpInfo(application, key);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Minor version release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of ApiResponse (string)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<string>> ApplicationApplicationVersionMinorReleasePutAsyncWithHttpInfo (string application, string key = null)
        {
            // verify the required parameter 'application' is set
            if (application == null)
                throw new ApiException(400, "Missing required parameter 'application' when calling AppApi->ApplicationApplicationVersionMinorReleasePut");

            var localVarPath = "/Application/{application}/Version/Minor/Release";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (application != null) localVarPathParams.Add("application", this.Configuration.ApiClient.ParameterToString(application)); // path parameter
            if (key != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "key", key)); // query parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ApplicationApplicationVersionMinorReleasePut", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<string>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => string.Join(",", x.Value)),
                (string) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(string)));
        }

        /// <summary>
        /// Patch release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>string</returns>
        public string ApplicationApplicationVersionPatchReleasePut (string application, string key = null)
        {
             ApiResponse<string> localVarResponse = ApplicationApplicationVersionPatchReleasePutWithHttpInfo(application, key);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Patch release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>ApiResponse of string</returns>
        public ApiResponse< string > ApplicationApplicationVersionPatchReleasePutWithHttpInfo (string application, string key = null)
        {
            // verify the required parameter 'application' is set
            if (application == null)
                throw new ApiException(400, "Missing required parameter 'application' when calling AppApi->ApplicationApplicationVersionPatchReleasePut");

            var localVarPath = "/Application/{application}/Version/Patch/Release";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (application != null) localVarPathParams.Add("application", this.Configuration.ApiClient.ParameterToString(application)); // path parameter
            if (key != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "key", key)); // query parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ApplicationApplicationVersionPatchReleasePut", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<string>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => string.Join(",", x.Value)),
                (string) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(string)));
        }

        /// <summary>
        /// Patch release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of string</returns>
        public async System.Threading.Tasks.Task<string> ApplicationApplicationVersionPatchReleasePutAsync (string application, string key = null)
        {
             ApiResponse<string> localVarResponse = await ApplicationApplicationVersionPatchReleasePutAsyncWithHttpInfo(application, key);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Patch release 
        /// </summary>
        /// <exception cref="OpenBots.Server.SDK.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="application"></param>
        /// <param name="key"> (optional)</param>
        /// <returns>Task of ApiResponse (string)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<string>> ApplicationApplicationVersionPatchReleasePutAsyncWithHttpInfo (string application, string key = null)
        {
            // verify the required parameter 'application' is set
            if (application == null)
                throw new ApiException(400, "Missing required parameter 'application' when calling AppApi->ApplicationApplicationVersionPatchReleasePut");

            var localVarPath = "/Application/{application}/Version/Patch/Release";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (application != null) localVarPathParams.Add("application", this.Configuration.ApiClient.ParameterToString(application)); // path parameter
            if (key != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "key", key)); // query parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ApplicationApplicationVersionPatchReleasePut", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<string>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => string.Join(",", x.Value)),
                (string) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(string)));
        }

    }
}

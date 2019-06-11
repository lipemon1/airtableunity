using System;
using System.Collections;
using System.Collections.Generic;
using AirtableUnity.PX.Model;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static UnityWebRequestExtension;

namespace AirtableUnity.PX
{
    public static class Proxy
    {
        private static string EndPoint_Airtable { get { return "https://api.airtable.com"; } }
        private static string ApiVersion;
        private static string AppKey;
        private static string ApiKey;

        #region Environment
        public static void SetEnvironment(string apiVersion, string appKey, string apiKey)
        {
            if(string.IsNullOrEmpty(apiVersion))
                Debug.LogError("Airtable Unity - Api Version informed is null or empty");
            
            if(string.IsNullOrEmpty(appKey))
                Debug.LogError("Airtable Unity - App Key informed is null or empty");
                
            if(string.IsNullOrEmpty(apiKey))
                Debug.LogError("Airtable Unity - Api Key informed is null or empty");
                
            ApiVersion = apiVersion;
            AppKey = appKey;
            ApiKey = apiKey;
            
            if(!string.IsNullOrEmpty(apiVersion) && !string.IsNullOrEmpty(AppKey) && !string.IsNullOrEmpty(ApiKey))
                Debug.Log("Airtable Unity - Environment prepared successfully");
        }
        #endregion

        #region Base Requests
        private static UnityWebRequest GetImageRequest(string url, Method method)
        {
            var request = GetRequest(url, method);
            request.SetDownloadHandler(true);
            return request;
        }

        private static UnityWebRequest GetRequest(string baseUri, string relativeUri, Method requestMethod, string data = null)
        {
            if (!baseUri.EndsWith("/"))
                baseUri += "/";
            var uriBase = new Uri(baseUri);
            var sendUri = new Uri(uriBase, relativeUri);
            var request = GetRequest(sendUri.OriginalString, requestMethod, data);
            return request;
        }
        
        private static string BaseRequestString(string tableName)
        {
            return $"{ApiVersion}/{AppKey}/{tableName}/";
        }

        private static UnityWebRequest GetRequest(string url, Method requestMethod, string data = null)
        {
            var request = new UnityWebRequest(url);
            request.SetDownloadHandler();
            request.SetUploadHandler(data);
            request.SetRequestMethod(requestMethod);
            request.SetRequestHeaders(new Dictionary<string, string>(){
                { "Content-Type", "application/json;charset=utf-8" },
                //{ "Cache-Control", "max-age=0, no-cache, no-store" },
                //{ "Pragma", "no-cache" }
            });

            Debug.Log("[UnitWebRequest]\n" +
                "Url: " + url + "\n" +
                "Method: " + requestMethod.ToString() + "\n" +
                "UploadHandler: " + data);
            return request;
        }
        #endregion

        #region Base Response
        public static Response GetResponse(UnityWebRequest request)
        {
            Response response = new Response();
            try
            {
                response = new Response();

                if (request.downloadHandler is DownloadHandler && request.responseCode != 404)
                    response = JsonConvert.DeserializeObject<Response>(request.downloadHandler.text);

                if (response == null)
                    response = new Response();

                response.Success = true;
                response.Message = request.downloadHandler.text;

                if (request.responseCode != 200)
                {
                    response.Success = false;
                    response.Message += "\nFail Http " + request.responseCode;

                    if (request.responseCode == 404)
                        response.Err.errors.Add("NOT_FOUND");

                    if (request.responseCode == 0)
                        response.Err.errors.Add("NO_CONNECTION");

                    if (request.responseCode == 400 || request.responseCode == 500)
                    {
                        response.Success = false;
                    }

                    if (response != null && response.Err != null)
                    {
                        response.Success = false;
                        response.Message += "\n" + response.Err;
                        Debug.LogError(response.Message + "\n" + request.url);
                    }

                    if (request.isNetworkError)
                    {
                        response.Success = false;
                        response.Message += "\n" + request.error;
                        Debug.LogError(response.Message + "\n" + request.url);
                    }

                    List<InternalError> errorsFound = InternalErrorHandler.GetPossibleErrors(response);
                    response.InternalErrors = errorsFound;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[RESPONSE ERROR] - [Message: " + e.Message + "] - [Request " + request.url + "]");
                response = new Response();
                response.Success = false;
                response.Message += "\n" + e.Message;
            }

            return response;
        }
        #endregion

        #region Requests

        #region List Records From Table

        public static IEnumerator ListRecordsCo<T>(string tableName, Action<List<BaseRecord<T>>> outputActionRecords = null)
        {
            var recordsToReturn = new List<BaseRecord<T>>();
            string curOffset = "";

            do
            {
                yield return ListRecords(tableName, curOffset).SendWebRequest(
                    (response) =>
                    {
                        var recordsFound = response?.GetAirtableData<T>()?.records;
                        
                        if(recordsFound?.Count > 0)
                            recordsToReturn.AddRange(recordsFound);
                        
                        curOffset = response?.GetAirtableData<T>()?.offset;
                    });
            } while (!string.IsNullOrEmpty(curOffset));

            outputActionRecords?.Invoke(recordsToReturn);
        }
        
        private static UnityWebRequest ListRecords(string tableName, string offset = "")
        {
            var relativeUri = "";
            var baseUri = BaseRequestString(tableName);
            
            if(string.IsNullOrEmpty(offset))
                relativeUri = $"{baseUri}?api_key={ApiKey}";
            else
                relativeUri = $"{baseUri}?api_key={ApiKey}&offset={offset}";
            
            return GetRequest(EndPoint_Airtable, relativeUri, Method.GET);
        }
        #endregion
        
        #region Get Record

        public static IEnumerator GetRecordCo<T>(string tableName, string recordId, Action<BaseRecord<T>> outputActionRecords = null)
        {
            var recordToReturn = new BaseRecord<T>();

            yield return GetRecord(tableName, recordId).SendWebRequest(
                (response) =>
                {
                    var recordFound = response?.GetAirtableRecord<T>();

                    if (recordFound != null)
                        recordToReturn = recordFound;
                });

            outputActionRecords?.Invoke(recordToReturn);
        }
        
        private static UnityWebRequest GetRecord(string tableName, string recordId)
        {
            var relativeUri = $"{BaseRequestString(tableName)}{recordId}/?api_key={ApiKey}";
            
            return GetRequest(EndPoint_Airtable, relativeUri, Method.GET);
        }
        #endregion  
        
        #region Create Record

        public static IEnumerator CreateRecordCo<T>(string tableName, string recordToCreate, Action<BaseRecord<T>> outputActionRecords = null)
        {
            var recordToReturn = new BaseRecord<T>();

            yield return CreateRecord(tableName, recordToCreate).SendWebRequest(
                (response) =>
                {
                    var recordFound = response?.GetAirtableRecord<T>();

                    if (recordFound != null)
                        recordToReturn = recordFound;
                });

            outputActionRecords?.Invoke(recordToReturn);
        }
        
        private static UnityWebRequest CreateRecord(string tableName, string recordToCreate)
        {
            var relativeUri = $"{BaseRequestString(tableName)}?api_key={ApiKey}";
            
            return GetRequest(EndPoint_Airtable, relativeUri, Method.POST, recordToCreate);
        }
        #endregion
        
        #region Update Record

        public static IEnumerator UpdateRecordCo<T>(string tableName, string recordId, string recordDataToUpdate, Action<BaseRecord<T>> outputActionRecords = null, bool hardUpdate = false)
        {
            var recordToReturn = new BaseRecord<T>();

            yield return UpdateRecord(tableName, recordId, recordDataToUpdate, hardUpdate).SendWebRequest(
                (response) =>
                {
                    var recordFound = response?.GetAirtableRecord<T>();

                    if (recordFound != null)
                        recordToReturn = recordFound;
                });

            outputActionRecords?.Invoke(recordToReturn);
        }
        
        private static UnityWebRequest UpdateRecord(string tableName, string recordId, string recordToCreate, bool hardUpdate)
        {
            var relativeUri = $"{BaseRequestString(tableName)}{recordId}/?api_key={ApiKey}";
            
            return GetRequest(EndPoint_Airtable, relativeUri, hardUpdate ? Method.PUT : Method.PATCH, recordToCreate);
        }
        #endregion

        #region Delete Record

        public static IEnumerator DeleteRecordCo<T>(string tableName, string recordId, Action<BaseRecord<T>> outputActionRecords = null)
        {
            var recordToReturn = new BaseRecord<T>();

            yield return DeleteRecord(tableName, recordId).SendWebRequest(
                (response) =>
                {
                    var recordFound = response?.GetAirtableRecord<T>();

                    if (recordFound != null)
                        recordToReturn = recordFound;
                });

            outputActionRecords?.Invoke(recordToReturn);
        }
        
        private static UnityWebRequest DeleteRecord(string tableName, string recordId)
        {
            var relativeUri = $"{BaseRequestString(tableName)}{recordId}/?api_key={ApiKey}";
            
            return GetRequest(EndPoint_Airtable, relativeUri, Method.DELETE);
        }

        #endregion
        #endregion
    }
}
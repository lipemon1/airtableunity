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

        private static UnityWebRequest GetAirtableRequest(string baseUri, string relativeUri, Method requestMethod, string data = null)
        {
            var request = GetRequest(baseUri, relativeUri, requestMethod, data);
            return request;
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

        #region Records From Table

        public static IEnumerator GetRecordsFromTable<T>(string tableName, Action<List<Record<T>>> outputActionRecords = null)
        {
            var recordsToReturn = new List<Record<T>>();
            string curOffset = "";

            do
            {
                yield return GetRecordsFromTable(tableName, curOffset).SendWebRequest(
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
        
        private static UnityWebRequest GetRecordsFromTable(string tableName, string offset = "")
        {
            var relativeUri = "";
            
            if(string.IsNullOrEmpty(offset))
                relativeUri = $"{ApiVersion}/{AppKey}/{tableName}?api_key={ApiKey}";
            else
                relativeUri = $"{ApiVersion}/{AppKey}/{tableName}?api_key={ApiKey}&offset={offset}";
            
            return GetRequest(EndPoint_Airtable, relativeUri, Method.GET);
        }
        
        #endregion
        #endregion
    }
}
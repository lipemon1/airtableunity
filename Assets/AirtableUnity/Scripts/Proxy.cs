using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static UnityWebRequestExtension;

namespace AirtableUnity.PX
{
    public static class Proxy
    {
        private static string EndPoint_Airtable { get { return "https://api.goepik.io"; } }
        private static string ApiVersion;
        private static string AppKey;
        private static string ApiKey;

        #region Environment
        public static void SetEnvironment(string apiVersion, string appKey, string apiKey)
        {
            ApiVersion = apiVersion;
            AppKey = appKey;
            ApiKey = apiKey;
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
        public static Response GetResposnse(UnityWebRequest request)
        {
            Response response = new Response();
            try
            {
                response = new Response();

                if (request.downloadHandler is DownloadHandlerTexture)
                    response.Image = (request.downloadHandler as DownloadHandlerTexture).texture;
                else if (request.downloadHandler is DownloadHandler && request.responseCode != 404)
                    response = JsonConvert.DeserializeObject<Response>(request.downloadHandler.text);

                if (response == null)
                    response = new Response();

                response.Success = true;
                response.Message = request.downloadHandler.text;
                response.Delete = false;

                if (request.responseCode != 200)
                {
                    response.Success = false;
                    response.Message += "\nFail Http " + request.responseCode.ToString();

                    if (request.responseCode == 403)
                        response.Delete = true;

                    if (request.responseCode == 404)
                        response.Err.errors.Add("NOT_FOUND");

                    if (request.responseCode == 0)
                        response.Err.errors.Add("NO_CONNECTION");

                    if (request.responseCode == 400 || request.responseCode == 500)
                    {
                        response.Delete = true;
                        response.Success = false;
                    }

                    if (response != null && response.Err != null)
                    {
                        response.Success = false;
                        response.Message += "\n" + response.Err;
                        response.Delete = true;
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
        public static UnityWebRequest GetRecordsFromTable(string tableName)
        {
            var relativeUri = $"/{ApiVersion}/{AppKey}/{tableName}?api_key={ApiKey}";
            return GetRequest(EndPoint_Airtable, relativeUri, Method.GET);
        }

        public static UnityWebRequest GetRecordsFromTable(string apiVersion, string appKey, string tableName, string apiKey)
        {
            var relativeUri = $"/{apiVersion}/{appKey}/{tableName}?api_key={apiKey}";
            return GetRequest(EndPoint_Airtable, relativeUri, Method.GET);
        }
        #endregion
        #endregion
    }
}
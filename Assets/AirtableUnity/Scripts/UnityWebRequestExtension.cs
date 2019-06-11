using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public static class UnityWebRequestExtension
{
    public enum Method { GET, POST, PUT, PATCH, DELETE };

    public static void SetRequestHeaders(this UnityWebRequest request, Dictionary<string, string> headers)
    {
        foreach (var item in headers)
            request.SetRequestHeader(item.Key, item.Value);
    }

    public static void SetUploadHandler(this UnityWebRequest request, string data)
    {
        if (data != null)
            request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(data));
    }

    public static void SetRequestMethod(this UnityWebRequest request, Method method)
    {
        request.method = method.ToString();
    }

    public static void SetDownloadHandler(this UnityWebRequest request, bool texture = false)
    {
        if (texture)
            request.downloadHandler = new DownloadHandlerTexture();
        else
            request.downloadHandler = new DownloadHandlerBuffer();
    }

    public static IEnumerator SendWebRequest(this UnityWebRequest request, System.Action<AirtableUnity.PX.Response> onSucess = null, System.Action<AirtableUnity.PX.Response> onFail = null)
    {
        yield return request.SendWebRequest();
        AirtableUnity.PX.Response response = AirtableUnity.PX.Proxy.GetResponse(request);
        var c1 = "<b>";
        var c2 = "</b>";

        Debug.Log($"[Response]\n" +
            $"{c1}Url: {c2}{request.url}\n" +
            $"{c1}Method: {c2}{request.method}\n" +
            $"{c1}UploadHandler: {c2}{request.uploadHandler}\n" +
            $"{c1}DownloadHandler: {c2}{request.downloadHandler.text}");

        if (response.Success)
        {
            try
            {
                onSucess?.Invoke(response);
            }
            catch (Exception e)
            {
                Debug.LogError(response.Message);
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.LogError(response.Message);
        }
    }
}
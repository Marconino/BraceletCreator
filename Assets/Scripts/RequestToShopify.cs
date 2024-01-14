using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class RequestToShopify
{
    static Task StartRequest(UnityWebRequest _webRequest)
    {
        var operation = _webRequest.SendWebRequest();
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        operation.completed += (asyncOperation) => taskCompletionSource.SetResult(true);
        return taskCompletionSource.Task;
    }

    public static async Task SendRequest(string _url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(_url);

        await StartRequest(webRequest);

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error : " + webRequest.error);
        }
    }

    public static async Task<T> GetResult<T>(string _url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(_url);

        await StartRequest(webRequest);

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error : " + webRequest.error);
            return default(T);
        }
        else
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(webRequest.downloadHandler.text, typeof(T));
            }
            catch (InvalidCastException)
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
            }

            return result;
        }
    }

    public static async Task<Texture2D> DownloadImage(string _url)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(_url);

        await StartRequest(webRequest);

        return DownloadHandlerTexture.GetContent(webRequest);
    }
}

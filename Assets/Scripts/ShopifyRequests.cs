using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class ShopifyRequests
{
    public class CollectionFromShopify
    {
        public string collectionTitle;
        public List<Products> products;
    }
    public class Products
    {
        public string title;
        public string price;
        public string[] imagesUrl;
        public Sprite[] images;
    }

    static Queue<UnityWebRequest> requests = new Queue<UnityWebRequest>();

    public static void StartRequest(string _url, bool _isTexture = false)
    {
        UnityWebRequest webRequest = _isTexture ? UnityWebRequestTexture.GetTexture(_url) : UnityWebRequest.Get(_url);
        requests.Enqueue(webRequest);
        webRequest.SendWebRequest();
    }

    public static bool HasRequestsStarted()
    {
        return requests.Count > 0;
    }

    public static bool IsFirstRequestCompleted()
    {
        return requests.Peek().isDone;
    }

    public static string GetStringData()
    {
        string result = string.Empty;
        UnityWebRequest webRequest = requests.Peek();

        if (webRequest.result == UnityWebRequest.Result.Success)
            result = webRequest.downloadHandler.text;
        else
            Debug.LogError("Error get data from request : " + webRequest.error);

        webRequest.Dispose();
        requests.Dequeue();
        return result;
    }

    public static Texture2D GetTexture()
    {
        Texture2D result = null;
        UnityWebRequest webRequest = requests.Peek();

        if (webRequest.result == UnityWebRequest.Result.Success)
            result = DownloadHandlerTexture.GetContent(webRequest);
        else
            Debug.LogError("Error get data from request : " + webRequest.error);

        webRequest.Dispose();
        requests.Dequeue();
        return result;
    }
}

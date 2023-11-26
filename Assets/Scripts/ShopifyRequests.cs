using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class ShopifyRequests
{
    public class ProductsJSON
    {
        public List<Product> products;
    }

    public class Product
    {
        public string title;
        public List<ProductVariant> variants;
        public List<ProductImage> images;
    }
    public class ProductVariant
    {
        public string price;
    }

    public class ProductImage
    {
        public string src;
    }

    static string token = "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";

    static string url = "https://ab7949-3.myshopify.com/admin/api/2023-10";
    static string collection = "/collections/609431978316";
    static string endpoint = "/products.json";

    static List<string> commandList = new List<string>();

    static List<string> ids = null;
    static ProductsJSON productsJSON = null;

    public static void AddCommand(string _parameter, params string[] _fields)
    {
        string startCharCommand = commandList.Count > 0 ? string.Empty : "?"; 
        string command = startCharCommand + _parameter + "=" + string.Join(',', _fields);
        commandList.Add(command);
    }

    public static void StartRequest(bool _fromCollection = false)
    {
        string commands = string.Join('&', commandList);

        string fullURL = _fromCollection ? url + collection + endpoint + commands : url + endpoint + commands;

        UnityWebRequest request = UnityWebRequest.Get(fullURL);
        request.SetRequestHeader("X-Shopify-Access-Token", token);
        request.SendWebRequest().completed += (operation) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (_fromCollection)
                {
                    string arrayText = FormatJSONTextToArrayText(request.downloadHandler.text);
                    ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(arrayText);
                }
                else
                    productsJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductsJSON>(request.downloadHandler.text);
            }
            else
                Debug.LogError(request.error);

            request.Dispose();
        };

        commandList.Clear();
    }

    static string FormatJSONTextToArrayText(string _json)
    {
        string json = _json;

        json = json.Replace("{", string.Empty);
        json = json.Replace("}", string.Empty);
        json = json.Remove(0, 11);
        json = json.Replace("\"id\":", string.Empty);

        return json;
    }

    public static bool IsIDsReceived()
    {
        return ids != null;
    }  
    public static bool IsProductsReceived()
    {
        return productsJSON != null;
    }

    public static List<string> GetIDs()
    {
        List<string> idsResult = ids;
        ids = null;
        return idsResult;
    }

    public static List<Product> GetProducts()
    {
        List<Product> productsResult = productsJSON.products;
        productsJSON = null;
        return productsResult;
    }  
}

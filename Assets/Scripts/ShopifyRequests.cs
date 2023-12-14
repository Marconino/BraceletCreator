using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    }

    static string token = "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";

    static string url = "https://ab7949-3.myshopify.com/admin/api/2023-10";
    static string collection = "/collections/609431978316";
    static string endpoint = "/products.json";

    static List<string> commandList = new List<string>();

    static List<string> ids = null;

    static CollectionFromShopify collectionFromShopify;

    public static void AddCommand(string _parameter, params string[] _fields)
    {
        string startCharCommand = commandList.Count > 0 ? string.Empty : "?"; 
        string command = startCharCommand + _parameter + "=" + string.Join(',', _fields);
        commandList.Add(command);
    }

    public static void StartRequest(bool _fromCollection = false)
    {
        //string apiUrl = "https://charremarc.fr/PHPShopify/get_products_collection.php?count=2"; // Remplacez par l'URL de votre boutique Shopify
        //UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        //request.SendWebRequest().completed += (operation) =>
        //{
        //    if (request.result == UnityWebRequest.Result.Success)
        //    {
        //        Debug.Log(request.downloadHandler.text);
        //        collectionFromShopify = Newtonsoft.Json.JsonConvert.DeserializeObject<CollectionFromShopify>(request.downloadHandler.text);
        //        Debug.Log(collectionFromShopify.collectionTitle);
        //        foreach(Products product in collectionFromShopify.products)
        //        {
        //            string productLog = "Title : " + product.title + " , Price : " + product.price + " , ImagesUrl : ";

        //            foreach(string url in product.imagesUrl)
        //            {
        //                UnityWebRequest test = UnityWebRequestTexture.GetTexture(url);

        //                test.SendWebRequest().completed += (operation) =>
        //                {

        //                };

        //                productLog += url + ", ";
        //            }
        //            productLog.Remove(productLog.Length - 1);
        //            Debug.Log(productLog);
        //        }
        //    }
        //    else
        //        Debug.LogError(request.error);

        //    request.Dispose();
        //};

        //        //string test = "{\"query\": \"mutation { cartCreate(input: { }) { cart { checkoutUrl } } } \"}";
        //        string test = "{\"query\": \"mutation { cartCreate(input: { lines:[{quantity: 2, merchandiseId: \\\"gid://shopify/ProductVariant/47389641245004\\\"}] }) { cart { checkoutUrl } } } \"}";

        //        UnityWebRequest request2 = UnityWebRequest.Post(apiUrl, test, "application/json");
        //        request2.SetRequestHeader("Accept", "application/json");

        //        request2.SetRequestHeader("X-Shopify-Storefront-Access-Token", "d89ae3d032979360074553ab9f6c97cb");

        //        request2.SendWebRequest().completed += (operation) =>
        //        {
        //            if (request2.result == UnityWebRequest.Result.Success)
        //            {
        //                if (_fromCollection)
        //                {
        //                    Debug.Log(request2.downloadHandler.text);
        //                    //string arrayText = FormatJSONTextToArrayText(request.downloadHandler.text);
        //                    //ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(arrayText);
        //                }
        //                else
        //                    productsJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductsJSON>(request2.downloadHandler.text);
        //            }
        //            else
        //                Debug.LogError(request2.error);

        //            request2.Dispose();
        //        };

        //        UnityWebRequest request3 = UnityWebRequest.Get("https://stylenzamineraux.fr/apps/braceletcreator");
        //        request3.SetRequestHeader("X-Shopify-Storefront-Access-Token", "d89ae3d032979360074553ab9f6c97cb");

        //        request3.SendWebRequest().completed += (operation) =>
        //        {
        //            if (request3.result == UnityWebRequest.Result.Success)
        //            {
        //                if (_fromCollection)
        //                {
        //                    Debug.Log(request3.downloadHandler.text);
        //                    //string arrayText = FormatJSONTextToArrayText(request.downloadHandler.text);
        //                    //ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(arrayText);
        //                }
        //                else
        //                    productsJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductsJSON>(request3.downloadHandler.text);
        //            }
        //            else
        //                Debug.LogError(request3.error);

        //            request3.Dispose();
        //        };
    }

    public static void StartPostRequest()
    {
        //string commands = string.Join('&', commandList);

        //string fullURL = "https://ab7949-3.myshopify.com/api/2023-10/graphql";

        ////Jaspe rouge roul�e
        //string jsonData = ConvertProductToJson("8719709241676", 1);

        //UnityWebRequest request = UnityWebRequest.Post(fullURL, "POST", "application/json");
        ////request.SetRequestHeader("Content-Type", "application/json");
        ////request.SetRequestHeader("X-Shopify-Storefront-Access-Token", "d89ae3d032979360074553ab9f6c97cb");
        //request.SetRequestHeader("X-Shopify-Access-Token", token);

        //string query = "{\"query\": \"query MyQuery { shop { name } }\"}";

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(query);
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        //request.downloadHandler = new DownloadHandlerBuffer();

        //request.SendWebRequest().completed += (operation) =>
        //{
        //    if (request.result == UnityWebRequest.Result.Success)
        //    {
        //        string result = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);

        //        Debug.Log("Requ�te POST envoy�e : " + result);

        //    }
        //    else
        //        Debug.LogError(request.error);

        //    request.Dispose();
        //};
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

    public static List<string> GetIDs()
    {
        List<string> idsResult = ids;
        ids = null;
        return idsResult;
    }
}

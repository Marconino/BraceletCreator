using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProductsManager : MonoBehaviour
{
    static ProductsManager instance;
    public static ProductsManager Instance { get => instance; }

    List<string> ids;
    bool hasReceivedAllData = false;
    int nbProducts = 0;

    [DllImport("__Internal")]
    private static extern void SendImageToJS(string _imageStr);

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddProductOnCart()
    {
        Application.OpenURL("https://stylenzamineraux.fr/apps/braceletcreator?variantId=47194252640588&quantity=5");
    }

    void Start()
    {
        ids = new List<string>();

        ShopifyRequests.StartRequest();
        GetCountProductsFromCollection();

        //StartRequestIDs();
        //ShopifyRequests.StartPostRequest();
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ScreenShot());
        }
    }
    IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] imageBytes = screenTexture.EncodeToPNG();
        string base64Image = Convert.ToBase64String(imageBytes);
        SendImageToJS(base64Image);
        Destroy(screenTexture);
    }
    void Update()
    {
        //if (!hasReceivedAllData)
        //{
        //    if (ShopifyRequests.IsIDsReceived())
        //    {
        //        ids = ShopifyRequests.GetIDs();
        //        StartRequestProductsFromIDs(ids);
        //    }

        //    if (ShopifyRequests.IsProductsReceived())
        //    {
        //        products = ShopifyRequests.GetProducts();
        //        hasReceivedAllData = true;
        //    }
        //}
    }

    void StartRequestIDs()
    {
        //ShopifyRequests.AddCommand("fields", "id");
        ShopifyRequests.StartRequest(true);
    }

    void StartRequestProductsFromIDs(List<string> _ids)
    {
        string idsString = string.Join(",", _ids);
        ShopifyRequests.AddCommand("ids", idsString);
        ShopifyRequests.AddCommand("fields", "images", "variants", "title");
        //ShopifyRequests.StartRequest();
    }

    void GetCountProductsFromCollection()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://charremarc.fr/PHPShopify/count_products_collection.php");

        request.SendWebRequest().completed += (operation) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                nbProducts = int.Parse(request.downloadHandler.text);
            }
            else
                Debug.LogError(request.error);

            request.Dispose();
        };
    }

}

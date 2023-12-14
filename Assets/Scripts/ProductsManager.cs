using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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

    ShopifyRequests.CollectionFromShopify collectionFromShopify;
    [SerializeField] Image image1;
    [SerializeField] Image image2;


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
        image1.sprite = null;
        image2.sprite = null;   
        ids = new List<string>();

        ShopifyRequests.StartRequest();
        GetCountProductsFromCollection();

        string apiUrl = "https://charremarc.fr/PHPShopify/get_products_collection.php?count=1"; // Remplacez par l'URL de votre boutique Shopify
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        request.SendWebRequest().completed += (operation) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                collectionFromShopify = Newtonsoft.Json.JsonConvert.DeserializeObject<ShopifyRequests.CollectionFromShopify>(request.downloadHandler.text);
                Debug.Log(collectionFromShopify.collectionTitle);
                foreach (ShopifyRequests.Products product in collectionFromShopify.products)
                {
                    string productLog = "Title : " + product.title + " , Price : " + product.price + " , ImagesUrl : ";

                    foreach (string url in product.imagesUrl)
                    {
                        UnityWebRequest test = UnityWebRequestTexture.GetTexture(url);

                        test.SendWebRequest().completed += (operation) =>
                        {
                            Texture2D texture = DownloadHandlerTexture.GetContent(test);
                            if (image1.sprite == null)
                            {
                                image1.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            }
                            else
                            {
                                image2.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            }
                        };

                        productLog += url + ", ";
                    }
                    productLog.Remove(productLog.Length - 1);
                    Debug.Log(productLog);
                }
            }
            else
                Debug.LogError(request.error);

            request.Dispose();
        };

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

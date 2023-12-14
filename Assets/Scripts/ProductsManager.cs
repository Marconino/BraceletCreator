using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ProductsManager : MonoBehaviour
{
    static ProductsManager instance;
    public static ProductsManager Instance { get => instance; }

    enum RequestSteps
    {
        GetProductsCount,
        GetProducts,
        GetImagesForProducts,
        CreateCustomBracelet,
        AddCustomImageOnCustomBracelet,
        AddBraceletOnClientCart
    }

    [DllImport("__Internal")]
    private static extern void SendImageToJS(string _imageStr);

    [SerializeField] GameObject productsGO;
    ShopifyRequests.CollectionFromShopify collectionFromShopify;
    RequestSteps currStep = RequestSteps.GetProductsCount;
    int currImageProduct = 0;


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
        ShopifyRequests.StartRequest("https://charremarc.fr/PHPShopify/count_products_collection.php");
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
        if (ShopifyRequests.HasRequestsStarted() && ShopifyRequests.IsFirstRequestCompleted())
        {
            switch (currStep)
            {
                case RequestSteps.GetProductsCount:
                    int nbProducts = int.Parse(ShopifyRequests.GetStringData());
                    for (int i = 0; i < nbProducts; i++)
                    {
                        GameObject child = new GameObject("Product_" + i);
                        child.AddComponent<RectTransform>().pivot = Vector2.zero;
                        Image image = child.AddComponent<Image>();
                        child.AddComponent<Button>().targetGraphic = image;
                        child.transform.parent = productsGO.transform;
                        child.transform.localScale = Vector3.one;
                    }
                    ShopifyRequests.StartRequest("https://charremarc.fr/PHPShopify/get_products_collection.php?count=" + nbProducts);
                    break;
                case RequestSteps.GetProducts:
                    collectionFromShopify = Newtonsoft.Json.JsonConvert.DeserializeObject<ShopifyRequests.CollectionFromShopify>(ShopifyRequests.GetStringData());

                    foreach (ShopifyRequests.Products products in collectionFromShopify.products)
                    {
                        foreach (string imageUrl in products.imagesUrl)
                        {
                            ShopifyRequests.StartRequest(imageUrl, true);
                        }
                    }
                    break;
                case RequestSteps.GetImagesForProducts:
                    Texture2D texture = ShopifyRequests.GetTexture();

                    if (collectionFromShopify.products[currImageProduct / 2].images == null)
                    {
                        collectionFromShopify.products[currImageProduct / 2].images = new Sprite[2];
                    }
                    collectionFromShopify.products[currImageProduct / 2].images[currImageProduct % 2] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    if (currImageProduct / 2 < productsGO.transform.childCount && currImageProduct % 2 == 0)
                    {
                        productsGO.transform.GetChild(currImageProduct / 2).GetComponent<Image>().sprite = collectionFromShopify.products[currImageProduct / 2].images[currImageProduct % 2];
                    }
                    currImageProduct++;
                    break;
                case RequestSteps.CreateCustomBracelet:
                    break;
                case RequestSteps.AddCustomImageOnCustomBracelet:
                    break;
                case RequestSteps.AddBraceletOnClientCart:
                    break;
            }

            if (currStep != RequestSteps.GetImagesForProducts || !ShopifyRequests.HasRequestsStarted())
                currStep++;
        }
    }
}
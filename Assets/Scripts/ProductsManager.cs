using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CollectionFromShopify
{
    public string collectionTitle;
    public List<Products> products;
}
public class Products
{
    public string title;
    public string handle;
    public string price;
    public string[] imagesUrl;
    public Sprite[] images;
}

public class ProductsManager : MonoBehaviour
{
    static ProductsManager instance;
    public static ProductsManager Instance { get => instance; }

    [DllImport("__Internal")]
    private static extern void SendImageToJS(string _imageStr);

    [SerializeField] Sprite logoStylenza;
    [SerializeField] GameObject productsGO;
    CollectionFromShopify collectionFromShopify;

    [SerializeField] int maxConcurrentDownloads = 5;
    Queue<string> downloadQueue;

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
        StartCoroutine(GetProductsCount("https://charremarc.fr/PHPShopify/count_products_collection.php"));
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ScreenShot());
        }
    }

    IEnumerator GetProductsCount(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de téléchargement: " + webRequest.error);
            }
            else
            {
                int nbProducts = int.Parse(webRequest.downloadHandler.text);

                for (int i = 0; i < nbProducts; i++)
                {
                    GameObject child = new GameObject("Product_" + i);
                    child.AddComponent<RectTransform>().pivot = Vector2.zero;
                    Image image = child.AddComponent<Image>();
                    image.sprite = logoStylenza;
                    child.AddComponent<Button>().targetGraphic = image;
                    child.transform.SetParent(productsGO.transform, false);
                    child.transform.localScale = Vector3.one;
                }

                StartCoroutine(GetProducts("https://charremarc.fr/PHPShopify/get_products_collection.php?count=" + nbProducts));
            }
        }
    }
    IEnumerator GetProducts(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de téléchargement: " + webRequest.error);
            }
            else
            {
                collectionFromShopify = Newtonsoft.Json.JsonConvert.DeserializeObject<CollectionFromShopify>(webRequest.downloadHandler.text);
                downloadQueue = new Queue<string>(collectionFromShopify.products.SelectMany(product => product.imagesUrl));

                for (int i = 0; i < maxConcurrentDownloads; i++)
                {
                    StartNextDownloadImage();
                }
            }
        }
    }
    void StartNextDownloadImage()
    {
        if (downloadQueue.Count > 0)
        {
            string url = downloadQueue.Dequeue();
            StartCoroutine(DownloadImage(url));
        }
    }
    IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de téléchargement: " + webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                int startNameStone = url.LastIndexOf('/');
                string stoneFilename = url.Substring(startNameStone + 1, url.LastIndexOf('.') - startNameStone - 1);
                string[] stoneStr = stoneFilename.Split('_');
                string stoneName = stoneStr[0];
                int stoneType = int.Parse(stoneStr[1]) - 1;

                Products product = collectionFromShopify.products.FirstOrDefault(p =>
                {
                    string name = p.handle;

                    if (name.Contains('-'))
                        name = name.Replace("-", string.Empty);

                    return name == stoneName;
                });

                if (product.images == null)
                    product.images = new Sprite[2];

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                product.images[stoneType] = sprite;
                
                if (stoneType == 0)
                {
                    int childIndex = collectionFromShopify.products.FindIndex(p => p.title == product.title);
                    productsGO.transform.GetChild(childIndex).GetComponent<Image>().sprite = sprite;
                }
            }

            // Lancer le téléchargement suivant
            StartNextDownloadImage();
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
}
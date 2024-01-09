using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
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
    public string[] tags;
    public Variants[] variants;
    public string pearlImageUrl;
    public Sprite pearlImage;
}

public class Variants
{
    public string price;
    public string imageUrl;
    public Sprite image;
}

public class ProductsManager : MonoBehaviour
{
    static ProductsManager instance;
    public static ProductsManager Instance { get => instance; }

    [DllImport("__Internal")]
    private static extern void SendImageToJS(string _imageStr, string _filename);

    [SerializeField] Sprite logoStylenza;
    [SerializeField] GameObject productsGO;
    [SerializeField] GameObject UIProductPrefab;
    CollectionFromShopify collectionFromShopify;

    [SerializeField] int maxConcurrentDownloads = 5;
    Queue<string> downloadQueue;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    IEnumerator PublishProduct(string _idBracelet, string _variantId)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://charremarc.fr/PHPShopify/publish_product.php?id=" + _idBracelet))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                StartCoroutine(AddProductOnCart(_variantId));
            }
        }
    }
    IEnumerator AddProductOnCart(string _idBracelet)
    {
        WWWForm form = new WWWForm();
        form.AddField("variantId", _idBracelet);
        form.AddField("quantity", "1");

        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://charremarc.fr/PHPShopify/testPost.php", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);




            }
        }
        //Application.OpenURL("https://stylenzamineraux.fr/apps/braceletcreator?variantId=" + _idBracelet + "&quantity=1");
        //Application.OpenURL("https://stylenzamineraux.fr/pages/loading?variantId=" + _idBracelet + "&quantity=1");
    }

    void GetProductFromShop(string _handleBracelet)
    {
        Application.OpenURL("https://stylenzamineraux.fr/products/" + _handleBracelet);
    }
    public IEnumerator GetHandleBraceletFromShop(string _handle)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://charremarc.fr/PHPShopify/get_bracelet_from_shop.php?filter=" + _handle))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
            }
            else
            {
                GetProductFromShop(webRequest.downloadHandler.text);
            }
        }
    }

    void Start()
    {
        StartCoroutine(GetProductsCount("https://charremarc.fr/PHPShopify/count_products_collection.php"));
    }

    IEnumerator GetProductsCount(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
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
                    child.AddComponent<ProductSelectable>();
                    child.transform.SetParent(productsGO.transform, false);
                    child.transform.localScale = Vector3.one;

                    Instantiate(UIProductPrefab, child.transform, false);
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
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
            }
            else
            {
                collectionFromShopify = Newtonsoft.Json.JsonConvert.DeserializeObject<CollectionFromShopify>(webRequest.downloadHandler.text);

                downloadQueue = new Queue<string>();

                foreach (Products product in collectionFromShopify.products)
                {
                    foreach (Variants variant in product.variants)
                    {
                        downloadQueue.Enqueue(variant.imageUrl);
                    }

                    downloadQueue.Enqueue(product.pearlImageUrl);
                }

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
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
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

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                if (stoneType < product.variants.Length)
                {
                    product.variants[stoneType].image = sprite;

                    if (stoneType == 0)
                    {
                        int childIndex = collectionFromShopify.products.FindIndex(p => p.title == product.title);
                        Transform productTransform = productsGO.transform.GetChild(childIndex);
                        productTransform.GetComponent<Image>().sprite = sprite;

                        Transform productUI = productTransform.GetChild(0);
                        productUI.GetChild(0).GetComponent<TMP_Text>().text = product.title;
                        productUI.GetChild(1).GetComponent<TMP_Text>().text = product.variants[stoneType].price + " �";
                    }
                }
                else
                {
                    product.pearlImage = sprite;
                }
            }

            // Lancer le t�l�chargement suivant
            StartNextDownloadImage();
        }
    }
    public void AddCustomBraceletToCart()
    {
        Pearl[] pearls = UIManager.Instance.GetPearlsInCurrentBracelet();

        string pearlSize = UIManager.Instance.GetCurrentPearlSize() == UIManager.FilterPearlSize.SizePearl8mm ? "8mm" : "10mm";

        string title = "Bracelet personnalis� " + pearlSize;
        string handle = "bracelet-personnalise-" + pearlSize;
        string description = string.Join("<br>", pearls.Select(p => p.title));
        string price = pearls.Select(p => float.Parse(p.price, CultureInfo.InvariantCulture)).Sum().ToString(CultureInfo.InvariantCulture);

        StartCoroutine(CreateCustomBracelet(title, handle, description, price));
    }
    IEnumerator CreateCustomBracelet(string _title, string _handle, string _description, string _price)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://charremarc.fr/PHPShopify/create_product.php?title=" + _title + "&handle=" + _handle + "&collection=612807442764&description=" + _description + "&price=" + _price))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
            }
            else
            {
                string[] idsBracelet = webRequest.downloadHandler.text.Split(',');

                string randomName = DateTime.Now.Ticks.ToString();
                StartCoroutine(ScreenShot(randomName, idsBracelet[0], idsBracelet[1]));
                Application.OpenURL("https://stylenzamineraux.fr/pages/loading");
            }
        }
    }

    IEnumerator AddCustomImageToCustomBracelet(string _filename, string _textImage, string _idBracelet, string _variantIdBracelet)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://charremarc.fr/PHPShopify/add_image_product.php?filename=" + _filename + "&text=" + _textImage + "&id=" + _idBracelet))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur de t�l�chargement: " + webRequest.error);
            }
            else
            { 
                //StartCoroutine(PublishProduct(_idBracelet, _variantIdBracelet));
            }
        }
    }

    IEnumerator ScreenShot(string _filename, string _idBracelet, string _variantIdBracelet)
    {
        yield return new WaitForEndOfFrame();

        int nbPearls = UIManager.Instance.GetNbPearlsInBracelet();
        float distanceBetweenPearls = UIManager.Instance.GetDistanceBetweenPearlsInCercle();
        float perimeter = nbPearls * distanceBetweenPearls;
        float radius = perimeter / (2 * Mathf.PI);
        float diameter = 2 * radius + 80f; //offset

        RectTransform rectTransform = UIManager.Instance.CreateBoundingRectangle(diameter);
        rectTransform.sizeDelta = new Vector2(diameter, diameter);

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int width = ((int)corners[3].x - (int)corners[0].x);
        int height = (int)corners[1].y - (int)corners[0].y;
        var startX = corners[0].x;
        var startY = corners[0].y;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        screenShot.Apply();

        byte[] imageBytes = screenShot.EncodeToJPG(50);

        //byte[] imageBytes = screenShot.EncodeToPNG();
        string base64Image = Convert.ToBase64String(imageBytes);
        SendImageToJS(base64Image, _filename);
        Destroy(screenShot);
        Destroy(rectTransform.gameObject);

        StartCoroutine(AddCustomImageToCustomBracelet(_filename + ".jpg", "Image en cours de t�l�chargement...", _idBracelet, _variantIdBracelet));
    }

    public void FilterProduct(UIManager.FilterPearlSize _filterType, string _name)
    {
        int childIndex = 0;

        collectionFromShopify.products.ForEach(product =>
        {
            Transform child = productsGO.transform.GetChild(childIndex);
            Image productImage = child.GetComponent<Image>();
            bool isActiveProduct = true;

            if (_filterType == UIManager.FilterPearlSize.SizePearl10mm && product.variants.Length == 1 || _name != string.Empty && !product.title.ToLower().StartsWith(_name.ToLower()))
            {
                isActiveProduct = false;
            }
            else
            {
                Transform productUI = child.GetChild(0);
                productUI.GetChild(1).GetComponent<TMP_Text>().text = product.variants[(int)_filterType].price + " �";
                productImage.sprite = product.variants[(int)_filterType].image;
            }

            if (child.gameObject.activeSelf != isActiveProduct)
                child.gameObject.SetActive(isActiveProduct);

            childIndex++;
        });
    }

    public Sprite GetPearlSpriteOfProduct(int _indexProduct)
    {
        return collectionFromShopify.products[_indexProduct].pearlImage;
    }

    public string GetPriceOfProduct(int _indexProduct, UIManager.FilterPearlSize _filterType)
    {
        return collectionFromShopify.products[_indexProduct].variants[(int)_filterType].price;
    }

    public string GetTitleOfProduct(int _indexProduct)
    {
        return collectionFromShopify.products[_indexProduct].title;
    }

    public string GetHandleOfProduct(int _indexProduct)
    {
        return collectionFromShopify.products[_indexProduct].handle;
    }

    public string GetKeywordsOfProduct(int _indexProduct)
    {
        if (collectionFromShopify == null || collectionFromShopify.products.Count < _indexProduct)
            return string.Empty;

        string[] tags = collectionFromShopify.products[_indexProduct].tags;

        if (tags == null)
            return string.Empty;

        return string.Join(", ", tags.Select((tag, index) => index == 0 ? tag : tag.ToLower()));
    }
}
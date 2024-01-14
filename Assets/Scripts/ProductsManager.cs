using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Adapted for Shopify JSON
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
    [SerializeField] Sprite logoStylenza;
    [SerializeField] GameObject UIProductPrefab;
    CollectionFromShopify collectionFromShopify;

    [SerializeField] int maxConcurrentDownloads = 5;
    Queue<string> downloadQueue;

    async void Start()
    {
        int nbProducts = await RequestToShopify.GetResult<int>("https://createurdebraceletstylenza.fr/PHPShopify/count_products_collection.php");
        CreateProducts(nbProducts);

        collectionFromShopify = await RequestToShopify.GetResult<CollectionFromShopify>("https://createurdebraceletstylenza.fr/PHPShopify/get_products_collection.php?count=" + nbProducts);

        downloadQueue = new Queue<string>();
        InitDownloadQueue();

        for (int i = 0; i < maxConcurrentDownloads; i++)
        {
            StartNextDownloadImage();
        }
    }

    void CreateProducts(int _nbProducts)
    {
        for (int i = 0; i < _nbProducts; i++)
        {
            GameObject child = new GameObject("Product_" + i);
            child.AddComponent<RectTransform>().pivot = Vector2.zero;
            Image image = child.AddComponent<Image>();
            image.sprite = logoStylenza;
            child.AddComponent<Button>().targetGraphic = image;
            child.AddComponent<ProductSelectable>();
            child.transform.SetParent(transform, false);
            child.transform.localScale = Vector3.one;

            Instantiate(UIProductPrefab, child.transform, false);
        }
    }
    void InitDownloadQueue()
    {
        foreach (Products product in collectionFromShopify.products)
        {
            foreach (Variants variant in product.variants)
            {
                downloadQueue.Enqueue(variant.imageUrl);
            }

            downloadQueue.Enqueue(product.pearlImageUrl);
        }
    }
    async void StartNextDownloadImage()
    {
        if (downloadQueue.Count > 0)
        {
            string url = downloadQueue.Dequeue();
            Texture2D texture = await RequestToShopify.DownloadImage(url);
            AddDownloadedImageToGO(url, texture);
            StartNextDownloadImage();
        }
    }
    void AddDownloadedImageToGO(string _url, Texture2D _texture)
    {
        int startNameStone = _url.LastIndexOf('/');
        string stoneFilename = _url.Substring(startNameStone + 1, _url.LastIndexOf('.') - startNameStone - 1);
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
        Sprite sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
        if (stoneType < product.variants.Length)
        {
            product.variants[stoneType].image = sprite;

            if (stoneType == 0)
            {
                int childIndex = collectionFromShopify.products.FindIndex(p => p.title == product.title);
                Transform productTransform = transform.GetChild(childIndex);
                productTransform.GetComponent<Image>().sprite = sprite;

                Transform productUI = productTransform.GetChild(0);
                productUI.GetChild(0).GetComponent<TMP_Text>().text = product.title;
                productUI.GetChild(1).GetComponent<TMP_Text>().text = product.variants[stoneType].price + " €";
            }
        }
        else
        {
            product.pearlImage = sprite;
        }
    }

    public void FilterProduct(FilterPearlSize _filterType, string _name)
    {
        int childIndex = 0;

        collectionFromShopify.products.ForEach(product =>
        {
            Transform child = transform.GetChild(childIndex);
            Image productImage = child.GetComponent<Image>();
            bool isActiveProduct = true;

            if (_filterType == FilterPearlSize.SizePearl10mm && product.variants.Length == 1 || _name != string.Empty && !product.title.ToLower().StartsWith(_name.ToLower()))
            {
                isActiveProduct = false;
            }
            else
            {
                Transform productUI = child.GetChild(0);
                productUI.GetChild(1).GetComponent<TMP_Text>().text = product.variants[(int)_filterType].price + " €";
                productImage.sprite = product.variants[(int)_filterType].image;
            }

            if (child.gameObject.activeSelf != isActiveProduct)
                child.gameObject.SetActive(isActiveProduct);

            childIndex++;
        });
    }

    #region Functions Utilities
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
    public string GetPriceOfProduct(int _indexProduct, FilterPearlSize _filterType)
    {
        return collectionFromShopify.products[_indexProduct].variants[(int)_filterType].price;
    }
    public Sprite GetPearlSpriteOfProduct(int _indexProduct)
    {
        return collectionFromShopify.products[_indexProduct].pearlImage;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class ProductsManager : MonoBehaviour
{
    static ProductsManager instance;
    public static ProductsManager Instance { get => instance; }

    List<string> ids;
    List<ShopifyRequests.Product> products;
    bool hasReceivedAllData = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        ids = new List<string>();
        products = new List<ShopifyRequests.Product>();

        StartRequestIDs();
        //ShopifyRequests.StartPostRequest();
        ScreenCapture.CaptureScreenshot("screenshot_test.png");
        string dataPah = Application.dataPath;
        string path = dataPah.Substring(0, dataPah.Length - 7);
        string imagePath = path + "/screenshot_test.png"; // Chemin de votre image dans le projet Unity
        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageBytes, "screenshot.png", "image/png");

        // Effectuer une requête POST vers le serveur
        UnityWebRequest www = UnityWebRequest.Post("https://charremarc.fr/Images/upload.php", form);
        www.SendWebRequest().completed += (operation) =>
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Image envoyée !");
            }
            else
            {
                Debug.LogError(www.error);
            }
        };
    }

    void Update()
    {
        if (!hasReceivedAllData)
        {
            if (ShopifyRequests.IsIDsReceived())
            {
                ids = ShopifyRequests.GetIDs();
                StartRequestProductsFromIDs(ids);
            }

            if (ShopifyRequests.IsProductsReceived())
            {
                products = ShopifyRequests.GetProducts();
                hasReceivedAllData = true;
            }
        }
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
        ShopifyRequests.StartRequest();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //StartRequestIDs();
        ShopifyRequests.StartPostRequest();
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
        ShopifyRequests.AddCommand("fields", "id");
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

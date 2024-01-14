using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FilterPearlSize
{
    SizePearl8mm,
    SizePearl10mm
}

public enum FilterWristSize
{
    SizeWrist14cm,
    SizeWrist15cm,
    SizeWrist16cm,
    SizeWrist17cm,
    SizeWrist18cm,
    SizeWrist19cm,
    SizeWrist20cm,
    SizeWrist21cm,
    SizeWrist22cm,
    SizeWrist23cm,
    SizeWrist24cm
}

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance { get => instance; }

    [Header("Products Manager")]
    [SerializeField] ProductsManager ProductsM;

    [Header("Prefabs")]
    [SerializeField] GameObject popupPrefab;

    [Header("UI refs")]
    [SerializeField] Canvas canvas;
    [SerializeField] Toggle filter8mm;
    [SerializeField] Toggle filter10mm;
    [SerializeField] TMP_Dropdown filterWrist;
    [SerializeField] InputField searchBar;
    [SerializeField] Button validateBracelet;
    [SerializeField] Transform bracelet;
    [SerializeField] GameObject warningFilter;

    [Header("Parameters")]
    [SerializeField] float distanceBetweenPearlsInCercle = 80f;
    bool isInCercle = false;

    FilterPearlSize currentFilter = FilterPearlSize.SizePearl8mm;

    string handleForGetBraceletFromShop = string.Empty;

    GameObject imagePearlOnMouse;
    PopUp popUpOnMouse;

    bool canDisplayWarningFilter = true;
    bool IsWarningFilterDisplayed = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        imagePearlOnMouse = new GameObject("PearlOnMouse");
        Image image = imagePearlOnMouse.AddComponent<Image>();
        image.raycastTarget = false;
        image.enabled = false;

        imagePearlOnMouse.transform.SetParent(canvas.transform);
        imagePearlOnMouse.transform.localPosition = Vector3.zero;
        imagePearlOnMouse.transform.localScale = Vector3.one;
        imagePearlOnMouse.SetActive(false);

        GameObject popupOnMouse = Instantiate(popupPrefab, imagePearlOnMouse.transform);
        popUpOnMouse = popupOnMouse.GetComponent<PopUp>();
        popUpOnMouse.Init();

        UpdateNbPearls();
    }

    void Update()
    {
        if (!IsWarningFilterDisplayed)
        {
            UpdateCurrentFilter();
        }

        if (imagePearlOnMouse.activeSelf)
        {
            imagePearlOnMouse.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }

    public ProductsManager GetProductsManager()
    {
        return ProductsM;
    }

    public async void AddCustomBraceletToCart()
    {
        Pearl[] currentPearlsInBracelet = bracelet.Cast<Transform>().Where(child => child.gameObject.activeSelf).
                                                                     Select(child => child.GetComponent<Pearl>()).ToArray();

        string pearlSize = currentFilter == FilterPearlSize.SizePearl8mm ? "8mm" : "10mm";

        string title = "Bracelet personnalisé " + pearlSize;
        string handle = "bracelet-personnalise-" + pearlSize;
        string description = string.Join("<br>", currentPearlsInBracelet.Select(p => p.GetTitle()));
        string price = currentPearlsInBracelet.Select(p => float.Parse(p.GetPrice(), CultureInfo.InvariantCulture)).Sum().ToString(CultureInfo.InvariantCulture);

        string result = await RequestToShopify.GetResult<string>("https://createurdebraceletstylenza.fr/PHPShopify/create_product.php?title=" + title + "&handle=" + handle + "&collection=612807442764&description=" + description + "&price=" + price);
        string idBracelet = result.Split(',')[0]; //I only want id, not variantId

        Application.OpenURL("https://stylenzamineraux.fr/pages/loading"); //Redirect to loading page while waiting for screenshot to be assigned to bracelet
        AddImageBraceletToBraceletOnShopify(idBracelet);
    }
    async void AddImageBraceletToBraceletOnShopify(string _idBracelet)
    {
        string randomName = DateTime.Now.Ticks.ToString();
        int nbPearls = GetNbPearlsInBracelet();
        StartCoroutine(ScreenshotBracelet.TakeScreenshot(bracelet.parent, GetDiameterOfBracelet(nbPearls), randomName));  
        await RequestToShopify.SendRequest("https://createurdebraceletstylenza.fr/PHPShopify/add_image_product.php?filename=" + randomName + ".jpg" + "&text=Image en cours de téléchargement..." + "&id=" + _idBracelet);
    }
    public async void GetProductFromShop()
    {
        string fullHandle = await RequestToShopify.GetResult<string>("https://createurdebraceletstylenza.fr/PHPShopify/get_bracelet_from_shop.php?filter=" + handleForGetBraceletFromShop);
        Application.OpenURL("https://stylenzamineraux.fr/products/" + fullHandle);
    }

    public void DisplayWarningFilter()
    {
        if (canDisplayWarningFilter && filter8mm.isOn && filter10mm.isOn)
        {
            warningFilter.SetActive(true);
            IsWarningFilterDisplayed = true;
        }
    }
    public void EndOfWarning()
    {
        string currentState = EventSystem.current.currentSelectedGameObject.name;

        if (currentState == "Yes")
        {
            Toggle toggle = warningFilter.transform.GetChild(0).GetChild(3).GetComponent<Toggle>();
            canDisplayWarningFilter = !toggle.isOn;
        }
        else if (currentState == "No")
        {
            if (currentFilter == FilterPearlSize.SizePearl8mm)
                filter10mm.isOn = false;
            else
                filter8mm.isOn = false;
        }

        warningFilter.SetActive(false);
        IsWarningFilterDisplayed = false;
    }

    #region Update filter and pearls
    void UpdateCurrentFilter()
    {
        bool canFilter = false;
        if (filter10mm.isOn && !filter8mm.interactable)
        {
            canFilter = true;
            filter8mm.interactable = true;
            filter8mm.isOn = false;
            filter10mm.interactable = false;
        }
        else if (filter8mm.isOn && !filter10mm.interactable)
        {
            canFilter = true;
            filter10mm.interactable = true;
            filter10mm.isOn = false;
            filter8mm.interactable = false;
        }

        if (canFilter)
        {
            currentFilter = currentFilter == FilterPearlSize.SizePearl8mm ? FilterPearlSize.SizePearl10mm : FilterPearlSize.SizePearl8mm;
            ProductsM.FilterProduct(currentFilter, searchBar.text);
            ResetBracelet();
            UpdateNbPearls();
        }
    }
    void UpdateNbPearls()
    {
        //55 = size of 8mm pearls, 68 = size of 10mm pearls
        ((RectTransform)imagePearlOnMouse.transform).sizeDelta = filter8mm.isOn ? new Vector2(55, 55) : new Vector2(68, 68);

        int baseValue = filter8mm.isOn ? 18 : 14;
        int nbPearls = baseValue + filterWrist.value;

        for (int i = 0; i < bracelet.childCount; i++)
        {
            GameObject child = bracelet.GetChild(i).gameObject;
            ((RectTransform)child.transform).sizeDelta = filter10mm.isOn ? new Vector2(68, 68) : new Vector2(55, 55);
            child.GetComponent<CircleCollider2D>().radius = filter10mm.isOn ? 35 : 30;
            child.SetActive(i < nbPearls ? true : false);
        }

        validateBracelet.interactable = IsTheBraceletFinished();
    }
    public void UpdateFirstAvailablePearl(int _currentProductIndex)
    {
        string title = ProductsM.GetTitleOfProduct(_currentProductIndex);
        string handle = ProductsM.GetHandleOfProduct(_currentProductIndex);
        string price = ProductsM.GetPriceOfProduct(_currentProductIndex, currentFilter);
        Sprite sprite = ProductsM.GetPearlSpriteOfProduct(_currentProductIndex);

        bracelet.Cast<Transform>().FirstOrDefault(child => child.gameObject.activeSelf && !child.GetComponent<Pearl>().HasValues())
            ?.GetComponent<Pearl>().SetPearlValues(title, handle, price, sprite);

        validateBracelet.interactable = IsTheBraceletFinished();
    }
    public void UpdatePearl(int _currentProductIndex)
    {
        //Physics test for drag and drop
        Collider2D pearlCollider = Physics2D.OverlapCircleAll(imagePearlOnMouse.transform.position, 35f)
            .OrderBy(p => Mathf.Abs(p.transform.localPosition.x - imagePearlOnMouse.transform.localPosition.x)).FirstOrDefault();

        if (pearlCollider)
        {
            string title = ProductsM.GetTitleOfProduct(_currentProductIndex);
            string handle = ProductsM.GetHandleOfProduct(_currentProductIndex);
            string price = ProductsM.GetPriceOfProduct(_currentProductIndex, currentFilter);
            pearlCollider.GetComponent<Pearl>().SetPearlValues(title, handle, price, imagePearlOnMouse.GetComponent<Image>().sprite);
        }

        if (_currentProductIndex != -1)
            _currentProductIndex = -1;

        validateBracelet.interactable = IsTheBraceletFinished();
    }
    #endregion

    #region Pearl on Mouse
    public void SetImagePearlOnMouse(Sprite _pearl)
    {
        imagePearlOnMouse.SetActive(true);
        Image image = imagePearlOnMouse.GetComponent<Image>();
        image.enabled = true;
        image.sprite = _pearl;
        imagePearlOnMouse.tag = "SelectedImagePearl";
    }
    public void RemoveImagePearlOnMouse()
    {
        imagePearlOnMouse.SetActive(false);
        imagePearlOnMouse.transform.localPosition = Vector3.zero;
        Image image = imagePearlOnMouse.GetComponent<Image>();
        image.enabled = false;
        image.sprite = null;
        imagePearlOnMouse.tag = "Untagged";
    }
    #endregion

    #region Utilities functions
    public void FilterWithName()
    {
        ProductsM.FilterProduct(currentFilter, searchBar.text);
    }

    public void SwitchMenu()
    {
        if (isInCercle)
        {
            bracelet.GetComponent<HorizontalLayoutGroup>().enabled = true;
            bracelet.GetComponent<PearlsMovement>().enabled = true;
        }
        else
            ArrangeInCircle();

        isInCercle = !isInCercle;

        string firstHandlePearl = bracelet.Cast<Transform>().First().GetComponent<Pearl>().GetHandle();

        //if the wrist size is above 16 cm and below 22 cm + all pearls are the same = bracelet already exist in shop
        bool currentBraceletAlreadyExistInShop = filterWrist.value > 2 && filterWrist.value < 8 && bracelet.Cast<Transform>().Where(child => child.gameObject.activeSelf).All(child => child.GetComponent<Pearl>().GetHandle() == firstHandlePearl);
        handleForGetBraceletFromShop = currentBraceletAlreadyExistInShop ? NormalizedHandlePearl(firstHandlePearl) : string.Empty;

        for (int i = 2; i < canvas.transform.childCount - 2; i++) //start at 2 because background is the first go, bracelet is the second go
        {
            GameObject child = canvas.transform.GetChild(i).gameObject;

            if ((child.name == "AddProductToCart" && !currentBraceletAlreadyExistInShop) ||
                (child.name == "GetProductFromShop" && currentBraceletAlreadyExistInShop) ||
                (child.name != "AddProductToCart" && child.name != "GetProductFromShop"))
            {
                child.SetActive(!child.activeSelf);
            }
        }

        Image image = bracelet.GetComponent<Image>();
        image.enabled = !image.enabled;

        float y = bracelet.transform.localPosition.y;
        bracelet.transform.localPosition = new Vector3(0, y == -465f ? 145 : -465f, 0); //center of bracelet in circle or in line
    }
    void ArrangeInCircle()
    {
        bracelet.GetComponent<HorizontalLayoutGroup>().enabled = false;
        bracelet.GetComponent<PearlsMovement>().enabled = false;

        int nbPearls = GetNbPearlsInBracelet();
        float diameter = GetDiameterOfBracelet(nbPearls);
        float radius = diameter / 2;

        //Angle between each
        float angleIncrement = 360f / nbPearls;

        for (int i = 0; i < nbPearls; i++)
        {
            //Convert angle to radians
            float radians = Mathf.Deg2Rad * (angleIncrement * i);

            //Calculate x and y positions on the circle
            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            bracelet.GetChild(i).localPosition = new Vector3(x, y, 0);
        }
    }

    float GetDiameterOfBracelet(int _nbPearls)
    {
        float perimeter = _nbPearls * distanceBetweenPearlsInCercle;
        float radius = perimeter / (2 * Mathf.PI);
        float diameter = 2 * radius + 80f; //80 = offset
        return diameter;
    }
    int GetNbPearlsInBracelet()
    {
        return bracelet.Cast<Transform>().Count(pearl => pearl.gameObject.activeSelf);
    }

    bool IsTheBraceletFinished()
    {
        for (int i = 0; i < bracelet.childCount; i++)
        {
            Transform child = bracelet.GetChild(i);

            if (!child.gameObject.activeSelf)
                break;

            if (!child.GetComponent<Pearl>().HasValues())
                return false;
        }
        return true;
    }
    public void ResetBracelet()
    {
        validateBracelet.interactable = false;
        for (int i = 0; i < bracelet.childCount; i++)
        {
            bracelet.GetChild(i).GetComponent<Pearl>().ResetPearl();
        }
    }

    //Normalized handle pearl exceptions
    string NormalizedHandlePearl(string _handlePearl)
    {
        if (_handlePearl.Contains("pierre-de-lune"))
        {
            _handlePearl = "pierre-de-lune";
        }
        else if (_handlePearl == "obsidienne-mouchetee")
        {
            _handlePearl = "obsidienne-neige";
        }

        return _handlePearl;
    }
    #endregion

    #region PopUp
    public void OpenPopupOnMouse(string _keywords, int _currentProductIndex)
    {
        imagePearlOnMouse.SetActive(true);

        int currentProductIndex = _currentProductIndex + 1;
        popUpOnMouse.UpdateText(_keywords, currentProductIndex != 0 && currentProductIndex % 4 == 0);
    }
    public void ClosePopupOnMouse()
    {
        popUpOnMouse.ClearUI();

    }
    #endregion
}

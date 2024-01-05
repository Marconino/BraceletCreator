using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance { get => instance; }

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

    [SerializeField] Canvas canvas;
    [SerializeField] Toggle filter8mm;
    [SerializeField] Toggle filter10mm;
    [SerializeField] TMP_Dropdown filterWrist;
    [SerializeField] InputField searchBar;
    [SerializeField] Button validateBracelet;
    [SerializeField] Transform bracelet;
    bool isInCercle = false;
    string handleForGetBraceletFromShop = string.Empty;

    GameObject imagePearlOnMouse;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        imagePearlOnMouse = new GameObject("PearlOnMouse");
        Image image = imagePearlOnMouse.AddComponent<Image>();

        imagePearlOnMouse.transform.SetParent(canvas.transform);
        imagePearlOnMouse.transform.localPosition = Vector3.zero;
        imagePearlOnMouse.transform.localScale = Vector3.one;
        imagePearlOnMouse.SetActive(false);

        UpdateNbPearls();
    }

    void Update()
    {
        if (filter10mm.isOn && !filter8mm.interactable)
        {
            filter8mm.interactable = true;
            filter8mm.isOn = false;
            filter10mm.interactable = false;
            ProductsManager.Instance.FilterProduct(FilterPearlSize.SizePearl10mm, searchBar.text);
            ResetBracelet();
            UpdateNbPearls();
        }
        else if (filter8mm.isOn && !filter10mm.interactable)
        {
            filter10mm.interactable = true;
            filter10mm.isOn = false;
            filter8mm.interactable = false;
            ProductsManager.Instance.FilterProduct(FilterPearlSize.SizePearl8mm, searchBar.text);
            ResetBracelet();
            UpdateNbPearls();
        }

        if (imagePearlOnMouse.activeSelf)
        {
            imagePearlOnMouse.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }

    public void UpdateNbPearls()
    {
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

    public void FilterWithName()
    {
        ProductsManager.Instance.FilterProduct(filter8mm.isOn ? FilterPearlSize.SizePearl8mm : FilterPearlSize.SizePearl10mm, searchBar.text);
    }

    public void SetImagePearlOnMouse(Sprite _pearl)
    {
        imagePearlOnMouse.SetActive(true);
        imagePearlOnMouse.GetComponent<Image>().sprite = _pearl;
        imagePearlOnMouse.tag = "SelectedImagePearl";
    }

    public void RemoveImagePearlOnMouse()
    {
        imagePearlOnMouse.SetActive(false);
        imagePearlOnMouse.transform.localPosition = Vector3.zero;
        imagePearlOnMouse.GetComponent<Image>().sprite = null;
        imagePearlOnMouse.tag = "Untagged";
    }

    public void UpdateFirstAvailablePearl(int _currentProductIndex)
    {
        string title = ProductsManager.Instance.GetTitleOfProduct(_currentProductIndex);
        string handle = ProductsManager.Instance.GetHandleOfProduct(_currentProductIndex);
        string price = ProductsManager.Instance.GetPriceOfProduct(_currentProductIndex, filter8mm.isOn ? FilterPearlSize.SizePearl8mm : FilterPearlSize.SizePearl10mm);
        Sprite sprite = ProductsManager.Instance.GetPearlSpriteOfProduct(_currentProductIndex);

        bracelet.Cast<Transform>().FirstOrDefault(child => child.gameObject.activeSelf && !child.GetComponent<Pearl>().HasValues())
            ?.GetComponent<Pearl>().SetPearlValues(title, handle, price, sprite);

        validateBracelet.interactable = IsTheBraceletFinished();
    }

    public void UpdatePearl(int _currentProductIndex)
    {
        Collider2D pearlCollider = Physics2D.OverlapCircleAll(imagePearlOnMouse.transform.position, 35f)
            .OrderBy(p => Mathf.Abs(p.transform.localPosition.x - imagePearlOnMouse.transform.localPosition.x)).FirstOrDefault();

        if (pearlCollider)
        {
            string title = ProductsManager.Instance.GetTitleOfProduct(_currentProductIndex);
            string handle = ProductsManager.Instance.GetHandleOfProduct(_currentProductIndex);
            string price = ProductsManager.Instance.GetPriceOfProduct(_currentProductIndex, filter8mm.isOn ? FilterPearlSize.SizePearl8mm : FilterPearlSize.SizePearl10mm);
            pearlCollider.GetComponent<Pearl>().SetPearlValues(title, handle,  price, imagePearlOnMouse.GetComponent<Image>().sprite);
        }

        if (_currentProductIndex != -1)
            _currentProductIndex = -1;

        validateBracelet.interactable = IsTheBraceletFinished();
    }

    public void ResetBracelet()
    {
        validateBracelet.interactable = false;
        for (int i = 0; i < bracelet.childCount; i++)
        {
            bracelet.GetChild(i).GetComponent<Pearl>().ResetPearl();
        }
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

    void ArrangeInCircle()
    {
        int childCount = bracelet.Cast<Transform>().Count(child => child.gameObject.activeSelf);

        bracelet.GetComponent<HorizontalLayoutGroup>().enabled = false;
        bracelet.GetComponent<PearlsMovement>().enabled = false;

        float distanceBetweenPearls = 80f;
        // Calculer le périmètre du cercle nécessaire pour espacer les objets.
        float perimeter = childCount * distanceBetweenPearls;

        // Déterminer le rayon du cercle à partir du périmètre (C = 2 * π * r).
        float radius = perimeter / (2 * Mathf.PI);

        // Calculer l'angle entre chaque GameObject actif.
        float angleIncrement = 360f / childCount;

        for (int i = 0; i < childCount; i++)
        {
            // Convertir l'angle en radians pour la trigonométrie.
            float radians = Mathf.Deg2Rad * (angleIncrement * i);

            // Calculer la position x et y sur le cercle.
            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            // Positionner le GameObject actif.
            bracelet.GetChild(i).localPosition = new Vector3(x, y, 0);
        }
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
        bool currentBraceletAlreadyExistInShop = bracelet.Cast<Transform>().Where(child => child.gameObject.activeSelf).All(child => child.GetComponent<Pearl>().GetHandle() == firstHandlePearl);
        handleForGetBraceletFromShop = currentBraceletAlreadyExistInShop ? NormalizedHandlePearl(firstHandlePearl) : string.Empty;

        for (int i = 2; i < canvas.transform.childCount - 1; i++) //start at 2 because background is the first go, bracelet is the second go
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
        bracelet.transform.localPosition = new Vector3(0, y == -465f ? 145 : -465f, 145);
    }
    
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

    public Pearl[] GetPearlsInCurrentBracelet()
    {
        return bracelet.Cast<Transform>().Where(child => child.gameObject.activeSelf).
            Select(child => child.GetComponent<Pearl>()).ToArray();
    }

    public void GetProductFromShop()
    {
        StartCoroutine(ProductsManager.Instance.GetHandleBraceletFromShop(handleForGetBraceletFromShop));
    }
}

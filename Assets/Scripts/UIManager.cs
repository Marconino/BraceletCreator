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
    [SerializeField] TMP_InputField searchBar;
    [SerializeField] Button validateBracelet;
    [SerializeField] Transform bracelet;
    int nbPearls = 0;

    GameObject imagePearlOnMouse;
    int currentProductIndex = -1;

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

        if (Input.GetMouseButtonDown(0) && imagePearlOnMouse.activeSelf)
        {
            UpdatePearl();
            RemoveImagePearlOnMouse();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ArrangeInCircle();
        }
    }

    public void UpdateNbPearls()
    {
        int baseValue = filter8mm.isOn ? 18 : 14;
        nbPearls = baseValue + filterWrist.value;

        for (int i = 0; i < bracelet.childCount; i++)
        {
            GameObject child = bracelet.GetChild(i).gameObject;
            ((RectTransform)child.transform).sizeDelta = filter10mm.isOn ? new Vector2(80, 80) : new Vector2(68, 68);
            child.GetComponent<CircleCollider2D>().radius = filter10mm.isOn ? 35 : 30;
            child.SetActive(i < nbPearls ? true : false);
        }

        if (IsTheBraceletFinished())
        {
            validateBracelet.interactable = true;
        }
        else
        {
            validateBracelet.interactable = false;
        }
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

    public void SetCurrentProductIndex(int _indexProduct)
    {
        currentProductIndex = _indexProduct;
    }

    public void UpdatePearl()
    {
        Collider2D pearlCollider = Physics2D.OverlapCircleAll(imagePearlOnMouse.transform.position, 35f)
            .OrderBy(p => Mathf.Abs(p.transform.localPosition.x - imagePearlOnMouse.transform.localPosition.x)).FirstOrDefault();

        if (pearlCollider)
        {
            string title = ProductsManager.Instance.GetTitleOfProduct(currentProductIndex);
            string price = ProductsManager.Instance.GetPriceOfProduct(currentProductIndex, filter8mm.isOn ? FilterPearlSize.SizePearl8mm : FilterPearlSize.SizePearl10mm);
            pearlCollider.GetComponent<Pearl>().SetPearlValues(title, price, imagePearlOnMouse.GetComponent<Image>().sprite);
        }

        if (currentProductIndex != -1)
            currentProductIndex = -1;

        if (IsTheBraceletFinished())
        {
            validateBracelet.interactable = true;
        }
        else
        {
            validateBracelet.interactable = false;
        }
    }

    public void ResetBracelet()
    {
        validateBracelet.interactable = false;
        for (int i = 0; i <  bracelet.childCount; i++)
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
}

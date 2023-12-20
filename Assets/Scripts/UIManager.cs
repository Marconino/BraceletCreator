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
    [SerializeField] Transform bracelet;
    int nbPearls = 0;

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
        //BoxCollider2D boxCollider = imagePearlOnMouse.AddComponent<BoxCollider2D>();
        //boxCollider.isTrigger = true;
        //boxCollider.size = ((RectTransform)imagePearlOnMouse.transform).sizeDelta;
        //Rigidbody2D rb = imagePearlOnMouse.AddComponent<Rigidbody2D>();
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;

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
            UpdateNbPearls();
        }
        else if (filter8mm.isOn && !filter10mm.interactable)
        {
            filter10mm.interactable = true;
            filter10mm.isOn = false;
            filter8mm.interactable = false;
            ProductsManager.Instance.FilterProduct(FilterPearlSize.SizePearl8mm, searchBar.text);
            UpdateNbPearls();
        }

        if (imagePearlOnMouse.activeSelf)
        {
            imagePearlOnMouse.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }

        if (Input.GetMouseButtonDown(0) && imagePearlOnMouse.activeSelf)
        {
            UpdatePearlImage();
            RemoveImagePearlOnMouse();
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

        StartCoroutine(PearlsMovement.Instance.UpdateAnchorPos());
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

    public void UpdatePearlImage()
    {
        Collider2D pearl = Physics2D.OverlapCircleAll(imagePearlOnMouse.transform.position, 35f)
            .OrderBy(p => Mathf.Abs(p.transform.localPosition.x - imagePearlOnMouse.transform.localPosition.x)).FirstOrDefault();

        if (pearl)
            pearl.GetComponent<Image>().sprite = imagePearlOnMouse.GetComponent<Image>().sprite;
    }
}

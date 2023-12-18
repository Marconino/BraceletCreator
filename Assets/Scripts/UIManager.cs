using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance { get => instance; }

    public enum FilterType
    {
        Size8mm,
        Size10mm
    }

    [SerializeField] Toggle filter8mm;
    [SerializeField] Toggle filter10mm;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if (filter10mm.isOn && !filter8mm.interactable)
        {
            filter8mm.interactable = true;
            filter8mm.isOn = false;
            filter10mm.interactable = false;
            ProductsManager.Instance.FilterProduct(FilterType.Size10mm);
        }
        else if (filter8mm.isOn && !filter10mm.interactable)
        {
            filter10mm.interactable = true;
            filter10mm.isOn = false;
            filter8mm.interactable = false;
            ProductsManager.Instance.FilterProduct(FilterType.Size8mm);
        }
    }
}

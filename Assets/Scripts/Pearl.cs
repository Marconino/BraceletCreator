using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pearl : MonoBehaviour
{
    public string title;
    public string price;
    Image image;

    private void Start()
    {
        title = string.Empty;
        price = string.Empty;
        image = GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.CompareTag("SelectedPearl"))
        {
            transform.SetSiblingIndex(collision.transform.GetSiblingIndex());
        }
    }

    public void SetPearlValues(string _title, string _price, Sprite _sprite)
    {
        title = _title;
        price = _price;
        image.sprite = _sprite;
    }

    public void ResetPearl()
    {
        title = string.Empty;
        price = string.Empty;
        image.sprite = null;
    }

    public bool HasValues()
    {
        return title != string.Empty && price != string.Empty && image.sprite != null;
    }
}

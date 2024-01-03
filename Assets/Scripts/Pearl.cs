using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pearl : MonoBehaviour
{
    public string title;
    public string price;
    Image currImage;
    Image baseImage;
    float pixelPerUnitMultiplier = 0f;

    private void Start()
    {
        title = string.Empty;
        price = string.Empty;
        currImage = GetComponent<Image>();
        baseImage = currImage;
        pixelPerUnitMultiplier = baseImage.pixelsPerUnitMultiplier;
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
        currImage.sprite = _sprite;
    }

    public void ResetPearl()
    {
        title = string.Empty;
        price = string.Empty;
        currImage.sprite = null;
        currImage.sprite = baseImage.sprite;
        currImage.pixelsPerUnitMultiplier = pixelPerUnitMultiplier;
    }

    public bool HasValues()
    {
        return title != string.Empty && price != string.Empty;
    }

    public string GetTitle()
    {
        return title;
    }
}

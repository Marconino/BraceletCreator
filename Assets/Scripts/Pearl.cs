using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pearl : MonoBehaviour
{
    public string title;
    public string handle;
    public string price;
    Image currImage;
    Sprite baseSprite;

    private void Start()
    {
        title = string.Empty;
        price = string.Empty;
        currImage = GetComponent<Image>();
        baseSprite = Sprite.Create(currImage.sprite.texture, currImage.sprite.rect, currImage.sprite.pivot, currImage.sprite.pixelsPerUnit);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.CompareTag("SelectedPearl"))
        {
            transform.SetSiblingIndex(collision.transform.GetSiblingIndex());
        }
    }

    public void SetPearlValues(string _title, string _handle, string _price, Sprite _sprite)
    {
        title = _title;
        handle = _handle;
        price = _price;
        currImage.sprite = _sprite;
    }

    public void ResetPearl()
    {
        title = string.Empty;
        handle = string.Empty;
        price = string.Empty;
        //currImage.sprite = null;
        currImage.sprite = baseSprite;
        //currImage.pixelsPerUnitMultiplier = pixelPerUnitMultiplier;
    }

    public bool HasValues()
    {
        return title != string.Empty && handle != string.Empty && price != string.Empty;
    }

    public string GetTitle()
    {
        return title;
    } 

    public string GetHandle()
    {
        return handle;
    }
}

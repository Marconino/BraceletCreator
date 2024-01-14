using UnityEngine;
using UnityEngine.UI;

public class Pearl : MonoBehaviour
{
    string title;
    string handle;
    string price;
    Image currImage;
    Sprite baseSprite;

    void Start()
    {
        title = string.Empty;
        price = string.Empty;
        currImage = GetComponent<Image>();
        baseSprite = Sprite.Create(currImage.sprite.texture, currImage.sprite.rect, currImage.sprite.pivot, currImage.sprite.pixelsPerUnit);
    }

    void OnTriggerExit2D(Collider2D collision)
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
        currImage.sprite = baseSprite;
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

    public string GetPrice()
    {
        return price;
    }
}

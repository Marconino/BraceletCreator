using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductSelectable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isDragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        Sprite pearl = ProductsManager.Instance.GetPearlSpriteOfProduct(transform.GetSiblingIndex());
        UIManager.Instance.SetImagePearlOnMouse(pearl);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        UIManager.Instance.UpdatePearl(transform.GetSiblingIndex());
        UIManager.Instance.RemoveImagePearlOnMouse();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
        {
            UIManager.Instance.UpdateFirstAvailablePearl(transform.GetSiblingIndex());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string keywords = ProductsManager.Instance.GetKeywordsOfProduct(transform.GetSiblingIndex());
        UIManager.Instance.OpenPopupOnMouse(keywords, transform.GetSiblingIndex());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.ClosePopupOnMouse();
    }
}

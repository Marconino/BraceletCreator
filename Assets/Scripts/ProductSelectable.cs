using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductSelectable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    bool isDragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        Sprite pearl = ProductsManager.Instance.GetPearlSpriteOfProduct(transform.GetSiblingIndex());
        UIManager.Instance.SetImagePearlOnMouse(pearl);
        UIManager.Instance.SetCurrentProductIndex(transform.GetSiblingIndex());
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        UIManager.Instance.SetCurrentProductIndex(transform.GetSiblingIndex());
        UIManager.Instance.UpdatePearl();
        UIManager.Instance.RemoveImagePearlOnMouse();
        Debug.Log("EndDrag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
        {
            Debug.Log("Click");
            Sprite pearl = ProductsManager.Instance.GetPearlSpriteOfProduct(transform.GetSiblingIndex());
            UIManager.Instance.SetImagePearlOnMouse(pearl);
            UIManager.Instance.SetCurrentProductIndex(transform.GetSiblingIndex());
        }
    }
}

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
        Debug.Log("BeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
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
        }
    }
}

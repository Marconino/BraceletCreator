using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    RectTransform backgroundRect;
    TMP_Text textComponent;
    [SerializeField] Vector2 offsetSizeBackground;
    [SerializeField] Vector2 offsetPosWithMouse;

    bool updateRequired = false;
    bool isRevert = false;
    public void Init()
    {
        backgroundRect = GetComponent<RectTransform>();
        textComponent = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void UpdateText(string _text, bool _isRevert)
    {
        textComponent.text = _text;
        updateRequired = true;
        isRevert = _isRevert;
    }

    private void Update()
    {
        if (updateRequired)
        {
            backgroundRect.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x + offsetSizeBackground.x, textComponent.preferredHeight + offsetSizeBackground.y);
            float x = isRevert ? -(backgroundRect.sizeDelta.x / 2f + offsetPosWithMouse.x) : backgroundRect.sizeDelta.x / 2f + offsetPosWithMouse.x;
            transform.localPosition = new Vector3(x, backgroundRect.sizeDelta.y/2f + offsetPosWithMouse.y, 0);
            updateRequired = false;
        }
    }
}

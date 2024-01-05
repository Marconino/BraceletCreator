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

    public void Init()
    {
        backgroundRect = GetComponent<RectTransform>();
        textComponent = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void UpdateText(string _text)
    {
        textComponent.text = _text;
        updateRequired = true;
    }

    private void Update()
    {
        if (updateRequired)
        {
            backgroundRect.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x + offsetSizeBackground.x, textComponent.preferredHeight + offsetSizeBackground.y);
            transform.localPosition = new Vector3(backgroundRect.sizeDelta.x/2f + offsetPosWithMouse.x, backgroundRect.sizeDelta.y/2f + offsetPosWithMouse.y, 0);
            updateRequired = false;
        }
    }
}

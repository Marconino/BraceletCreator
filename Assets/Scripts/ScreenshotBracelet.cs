using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ScreenshotBracelet
{
    [DllImport("__Internal")]
    static extern void SendImageToJS(string _imageStr, string _filename);

    static RectTransform CreateBoundingRectangle(Transform _parent, float _diameter)
    {
        GameObject boundingRectangle = new GameObject("BoundingRectangle");
        boundingRectangle.transform.SetParent(_parent, false);
        boundingRectangle.transform.localScale = Vector3.one;
        boundingRectangle.transform.localPosition = new Vector3(0, 145, 0); //Center of bracelet
        RectTransform rectTransform = boundingRectangle.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(_diameter, _diameter);

        return rectTransform;
    }

    public static IEnumerator TakeScreenshot(Transform _parent, float _diameter, string _filename)
    {
        yield return new WaitForEndOfFrame(); //wait drawing frame

        RectTransform rectTransform = CreateBoundingRectangle(_parent, _diameter);

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int width = ((int)corners[3].x - (int)corners[0].x);
        int height = (int)corners[1].y - (int)corners[0].y;
        var startX = corners[0].x;
        var startY = corners[0].y;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        texture.Apply();

        byte[] textureBytes = texture.EncodeToJPG(50);

        string base64Image = Convert.ToBase64String(textureBytes);
        SendImageToJS(base64Image, _filename);
        UnityEngine.Object.Destroy(rectTransform.gameObject);
    }
}

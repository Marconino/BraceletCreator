using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pearl : MonoBehaviour
{
    Vector3 currAnchorPos;

    void Start()
    {
        currAnchorPos = transform.localPosition;
    }

    public void SetAnchorPos(Vector3 _anchorPos)
    {
        currAnchorPos = _anchorPos;
    }

    public void SetPosToAnchorPos()
    {
        transform.localPosition = currAnchorPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.CompareTag("SelectedPearl"))
        {
            Vector3 collisionPos = collision.gameObject.transform.localPosition;
            collision.gameObject.transform.localPosition = currAnchorPos;
            collision.GetComponent<Pearl>().SetAnchorPos(currAnchorPos);
            currAnchorPos = collisionPos;
        }
    }
}

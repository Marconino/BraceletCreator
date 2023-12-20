using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pearl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.CompareTag("SelectedPearl"))
        {
            transform.SetSiblingIndex(collision.transform.GetSiblingIndex());
        }
    }
}

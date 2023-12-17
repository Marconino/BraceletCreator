using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PearlsMovement : MonoBehaviour
{
    Vector3[] pearlsInitialPos;
    GameObject goSelected;

    void Start()
    {
        pearlsInitialPos = new Vector3[transform.childCount];

        for (int i = 0; i < pearlsInitialPos.Length; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 childPos = child.localPosition;

            pearlsInitialPos[i] = childPos;
            Debug.Log("child : " + i + " pos : " + childPos);
        }
    }


    bool HasSelectedPearl()
    {
        return Input.GetMouseButton(0) && goSelected;
    }

    void PearlMovement()
    {
        Vector3 pos = goSelected.transform.position;
        goSelected.transform.position = new Vector3(Input.mousePosition.x, pos.y, pos.z);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject?.transform.parent == transform)
            goSelected = EventSystem.current.currentSelectedGameObject;

        if (HasSelectedPearl())
        {
            //Debug.Log("HasSelectedPearl");
            PearlMovement();

            if (goSelected.CompareTag("Untagged"))
            {
                goSelected.tag = "SelectedPearl";
            }
        }
        else
        {
            //Debug.Log("HasNotSelectedPearl");

            if (goSelected != null)
            {
                goSelected.GetComponent<Pearl>().SetPosToAnchorPos();
                goSelected.tag = "Untagged";
                goSelected = null;
            }
        }
    }
}

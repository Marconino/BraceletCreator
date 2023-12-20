using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PearlsMovement : MonoBehaviour
{
    static PearlsMovement instance;
    public static PearlsMovement Instance { get => instance; }

    GameObject goSelected;
    Pearl[] pearls;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        pearls = new Pearl[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 childPos = child.localPosition;

            pearls[i] = child.GetComponent<Pearl>();
            pearls[i].SetAnchorPos(childPos);
        }

        GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    public IEnumerator UpdateAnchorPos()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < pearls.Length; i++)
        {
            pearls[i].SetAnchorPos(pearls[i].transform.localPosition);
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
        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.parent == transform)
            goSelected = EventSystem.current.currentSelectedGameObject;

        if (HasSelectedPearl())
        {
            PearlMovement();

            if (goSelected.CompareTag("Untagged"))
            {
                goSelected.tag = "SelectedPearl";
            }
        }
        else
        {
            if (goSelected != null)
            {
                goSelected.GetComponent<Pearl>().SetPosToAnchorPos();
                goSelected.tag = "Untagged";
                goSelected = null;
            }
        }
    }
}

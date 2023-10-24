using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PearlsMovement : MonoBehaviour
{
    GameObject goSelected;
    CircleCollider2D circleColliderSelected;
    CircleCollider2D[] pearls;

    void Start()
    {
        pearls = new CircleCollider2D[24];
        for (int i = 0; i < pearls.Length; i++)
        {
            pearls[i] = transform.GetChild(i).GetComponent<CircleCollider2D>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject != null)
        {
            Debug.Log("Selected");
            goSelected = EventSystem.current.currentSelectedGameObject;

            if (circleColliderSelected == null)
                circleColliderSelected = goSelected.GetComponent<CircleCollider2D>();

            Vector3 pos = goSelected.transform.position;
            goSelected.transform.position = new Vector3(Input.mousePosition.x, pos.y, pos.z);

        }
        else
        {
            goSelected = null;
            circleColliderSelected = null;
            Debug.Log("Not selected");
        }

    }

    private void FixedUpdate()
    {
        if (goSelected != null && circleColliderSelected != null)
        {
            for (int i = 0; i < pearls.Length; i++)
            {
                if (pearls[i].IsTouching(circleColliderSelected))
                {
                    Debug.Log(circleColliderSelected  + " is touching " + pearls[i]);
                }
            }
        }
    }
}

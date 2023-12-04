using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PearlsMovement : MonoBehaviour
{
    Vector3[] pearlsInitialPos;

    GameObject goSelected;
    CircleCollider2D circleColliderSelected;
    CircleCollider2D[] pearls;

    void Start()
    {
        pearlsInitialPos = new Vector3[transform.childCount];
        pearls = new CircleCollider2D[transform.childCount];

        HorizontalLayoutGroup horizontalLayoutGroup = transform.GetComponent<HorizontalLayoutGroup>();
        
        for (int i = 0; i < pearls.Length; i++)
        {

            RectTransform childRectTransform = transform.GetChild(i) as RectTransform;
            
            Transform child = transform.GetChild(i);
            pearls[i] = child.GetComponent<CircleCollider2D>();
            pearlsInitialPos[i] = pearls[i].transform.position;
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
                    int newIndexPos = pearls[i].transform.position.x - goSelected.transform.position.x > 0 ? i - 1 : i + 1;
                    
                    goSelected.transform.position = pearlsInitialPos[i];
                    pearls[i].transform.position = pearlsInitialPos[newIndexPos];

                    goSelected = null;
                    circleColliderSelected = null;
                    break;
                }
            }
        }
    }
}

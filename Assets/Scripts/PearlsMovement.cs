using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PearlsMovement : MonoBehaviour
{
    HorizontalLayoutGroup layoutGroup;
    GameObject goSelected;

    void Start()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
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
                goSelected.tag = "Untagged";
                goSelected = null;
                layoutGroup.SetLayoutHorizontal();
            }
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
}

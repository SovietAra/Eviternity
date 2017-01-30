using UnityEngine;
using UnityEngine.EventSystems;

public class KeepButtonsScript : MonoBehaviour
{
    public EventSystem EventSys;
    private GameObject storeSelected;

    // Use this for initialization
    private void Start ()
    {
        storeSelected = EventSys.firstSelectedGameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (EventSys.currentSelectedGameObject != storeSelected)
        {
            if (EventSys.currentSelectedGameObject == null)
            {
                EventSys.SetSelectedGameObject(storeSelected);
            }
            else
            {
                storeSelected = EventSys.currentSelectedGameObject;
            }
        }
    }
}

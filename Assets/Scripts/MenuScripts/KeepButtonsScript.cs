using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeepButtonsScript : MonoBehaviour {

    public EventSystem ES;
    private GameObject StoreSelected;
    public AudioClip ClickClip;
    private AudioSource Click;

    // Use this for initialization
    void Start () {
        Click= gameObject.AddComponent<AudioSource>();
        StoreSelected = ES.firstSelectedGameObject;
        Click.clip = ClickClip;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (ES.currentSelectedGameObject == StoreSelected) return;
	    if (ES.currentSelectedGameObject == null)
	    {
	        ES.SetSelectedGameObject(StoreSelected);
	    }
	    else
	    {
	        StoreSelected = ES.currentSelectedGameObject;

	        Click.Play();
	    }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeepButtonsScript : MonoBehaviour {

    public EventSystem ES;
    private GameObject StoreSelected;
    public AudioClip SwitchClip;
    private AudioSource Switch;
    public AudioClip ClickClip;
    private AudioSource Click;

    // Use this for initialization
    void Start () {
        Switch = gameObject.AddComponent<AudioSource>();
        StoreSelected = ES.firstSelectedGameObject;
        Switch.clip = SwitchClip;

        Click = gameObject.AddComponent<AudioSource>();
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
            Switch.Play();
        }
	}

    public void pressButton()
    {
        Click.Play();
    }
}

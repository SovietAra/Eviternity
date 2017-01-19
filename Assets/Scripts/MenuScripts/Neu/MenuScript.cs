using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public EventSystem ES;
    private GameObject StoreSelected;
    // Use this for initialization
    void Start ()
    {
        StoreSelected = ES.firstSelectedGameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ES.currentSelectedGameObject != StoreSelected)
        {
            if (ES.currentSelectedGameObject == null)
            {
                ES.SetSelectedGameObject(StoreSelected);
            }
            else
            {
                StoreSelected = ES.currentSelectedGameObject;
            }
        }
    }

    public void PressBackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PressPlay()
    {
        SceneManager.LoadScene("PlayerAssignment");
    }

    public void PressRestart()
    {
        SceneManager.LoadScene("RestartMenu");
    }

    public void PressOptions()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void PressCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

    public class menuScript : MonoBehaviour {

    public Canvas restartMenu;
    public Canvas optionsMenu;
    public Canvas creditScreen;
    public Canvas playerSelectScreen;
    public GameObject startText;
    public GameObject optionsText;
    public GameObject restartText;
    public Button backText;

    public EventSystem ES;
    
    private GameObject StoreSelected;
    public GameObject backButtonCredits;
    public GameObject backButtonOptions;
    public GameObject backButtonPlayerSelect;

    // Use this for initialization
    public void Start ()
	 {
		restartMenu = restartMenu.GetComponent<Canvas> ();
		optionsMenu = optionsMenu.GetComponent<Canvas> ();
		creditScreen = creditScreen.GetComponent<Canvas> ();
		playerSelectScreen= playerSelectScreen.GetComponent<Canvas> ();
		startText = startText.GetComponent<GameObject> ();
		optionsText = optionsText.GetComponent<GameObject> ();
		restartText = restartText.GetComponent<GameObject> ();
		backText = backText.GetComponent<Button> ();
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;

        StoreSelected = ES.firstSelectedGameObject;
       // test = test.GetComponent<Canvas>();
    }

    void Update()
    {
        //CheckMenu();
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
        if (creditScreen.enabled == true)
        {
            ES.SetSelectedGameObject(backButtonCredits);
        }
        else if (optionsMenu.enabled == true)
        {
            ES.SetSelectedGameObject(backButtonOptions);
        }

    }

    public void ExitPress()
	{
		restartMenu.enabled = true;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		backText.enabled = false;
	}

	public void CreditPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = true;
		playerSelectScreen.enabled = false;
		backText.enabled = true;
	}

	public void OptionsPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = true;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		backText.enabled = true;
	}

	public void BackPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		backText.enabled = false;
	}

		public void PlayerSelectPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = true;
		backText.enabled = true;
	}

	public void NoPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		backText.enabled = false;
	}

	public void RestartGame()
	{
		Application.Quit ();
	}
    
   // public bool menuOpened = false;               geht, aber nicht wie gedacht, man switched direkt zwischen menüs, ohne Back-Buttons zu nutzen
   // public Canvas test;
    /*public void CheckMenu()
    {
        if(test.enabled == true)
        {
            menuOpened = true;   
        }
        if (menuOpened == true)
            menuOpened = false;

        if (menuOpened == false)
            ES.SetSelectedGameObject(startText);

    }*/
}

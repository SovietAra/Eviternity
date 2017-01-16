using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class menuScript : MonoBehaviour {
public Canvas restartMenu;
public Canvas optionsMenu;
public Canvas creditScreen;
public Canvas playerSelectScreen;
public Button startText;
public Button optionsText;
public Button restartText;
public Button backText;

	// Use this for initialization
	public void Start ()
	 {
		restartMenu = restartMenu.GetComponent<Canvas> ();
		optionsMenu = optionsMenu.GetComponent<Canvas> ();
		creditScreen = creditScreen.GetComponent<Canvas> ();
		playerSelectScreen= playerSelectScreen.GetComponent<Canvas> ();
		startText = startText.GetComponent<Button> ();
		optionsText = optionsText.GetComponent<Button> ();
		restartText = restartText.GetComponent<Button> ();
		backText = backText.GetComponent<Button> ();
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
	}
    
    public void ExitPress()
	{
		restartMenu.enabled = true;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		startText.enabled = false;
		optionsText.enabled = false;
		restartText.enabled = false;
		backText.enabled = false;
	}

	public void CreditPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = true;
		playerSelectScreen.enabled = false;
		startText.enabled = false;
		optionsText.enabled = false;
		restartText.enabled = false;
		backText.enabled = true;
	}

	public void OptionsPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = true;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		startText.enabled = false;
		optionsText.enabled = false;
		restartText.enabled = false;
		backText.enabled = true;
	}

	public void BackPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		startText.enabled = true;
		optionsText.enabled = true;
		restartText.enabled = true;
		backText.enabled = false;
	}

		public void PlayerSelectPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = true;
		startText.enabled = false;
		optionsText.enabled = false;
		restartText.enabled = false;
		backText.enabled = true;
	}

	public void NoPress()
	{
		restartMenu.enabled = false;
		optionsMenu.enabled = false;
		creditScreen.enabled = false;
		playerSelectScreen.enabled = false;
		startText.enabled = true;
		optionsText.enabled = true;
		restartText.enabled = true;
		backText.enabled = false;
	}

	public void RestartGame()
	{
		Application.Quit ();
	}
}

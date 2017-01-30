using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class MenuScript : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void PressBackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PressPlay()
    {
        EditorSceneManager.LoadScene("PlayerAssignment");
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

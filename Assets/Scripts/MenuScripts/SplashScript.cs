using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class SplashScript : MonoBehaviour {

    public Text Text1;
    public Text Text2;
    public float targetTime = 3.0f;
    public float partTime = 1.0f;
    public float timerTime = 1.0f;
    // Use this for initialization
    void Start ()
    {
        Text1.enabled = true;
        Text2.enabled = false;
	}
    
	// Update is called once per frame
	void Update ()
    {
        targetTime -= Time.deltaTime;
        partTime -= Time.deltaTime;

        if(targetTime <= 0.0f || Input.GetButton("Submit"))
        {
            timerEnded();
        }
        if(partTime <= 0.0f)
        {
            changeTextToBlack();
            if(timerTime <= 0.0f)
            changeTextToRed();
        }
	}

    public void timerEnded()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void changeTextToBlack()
    {
        Text1.enabled = false;
        Text2.enabled = true;
        timerTime -= Time.deltaTime;
    }

    public void changeTextToRed()
    {
        Text1.enabled = true;
        Text2.enabled = false;
        partTime = 1.0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class SplashScript : MonoBehaviour {

    public float targetTime = 5.0f;
    // Use this for initialization
    void Start ()
    {
	
	}
    
	// Update is called once per frame
	void Update ()
    {
        targetTime -= Time.deltaTime;

        if(targetTime <= 0.0f || Input.GetButton("Submit"))
        {
            timerEnded();
        }
	}

    public void timerEnded()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

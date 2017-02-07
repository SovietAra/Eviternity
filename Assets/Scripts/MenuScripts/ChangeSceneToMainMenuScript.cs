using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class ChangeSceneToMainMenuScript : MonoBehaviour {
    public float mainTime = 37.0f;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mainTime -= Time.deltaTime;
        if (mainTime <= 0.0f || Input.anyKey)
        {
            ChangeSceneToMain();
        }
    }

    public void ChangeSceneToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


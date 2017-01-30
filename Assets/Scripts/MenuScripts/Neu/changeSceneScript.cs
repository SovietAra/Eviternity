using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class changeSceneScript : MonoBehaviour {
    public float targetTime = 30.0f;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        targetTime -= Time.deltaTime;
        if (targetTime <= 0.0f)
        {
            changeScene();
        }
	}
    public void changeScene()
    {
        SceneManager.LoadScene("Credits");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class changeSceneToCreditsScript : MonoBehaviour
{
    public float creditTime = 30.0f;
    public float inputTime = 0.0f;
    
    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        creditTime -= Time.deltaTime;
        inputTime += Time.deltaTime;
        if (creditTime <= 0.0f || inputTime >= 30.0f)
        {
            ChangeSceneToCredits();
        }

        if (Input.anyKey || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
        {
            inputTime = 0.0f;
        }
    }
    public void ChangeSceneToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScript : MonoBehaviour {

    public Text Text1;
    public Text Text2;
    private float targetTime = 3.0f;
    private float partTime = 1.0f;
    private float timerTime = 1.0f;
    // Use this for initialization
    private void Start ()
    {
        Text1.enabled = true;
        Text2.enabled = false;
	}
    
	// Update is called once per frame
	private void Update ()
    {
        targetTime -= Time.deltaTime;
        partTime -= Time.deltaTime;

        if(targetTime <= 0.0f || Input.GetButton("Submit"))
        {
            TimerEnded();
        }
        if(partTime <= 0.0f)
        {
            ChangeTextToBlack();
            if(timerTime <= 0.0f)
            ChangeTextToRed();
        }
	}

    private void TimerEnded()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ChangeTextToBlack()
    {
        Text1.enabled = false;
        Text2.enabled = true;
        timerTime -= Time.deltaTime;
    }

    private void ChangeTextToRed()
    {
        Text1.enabled = true;
        Text2.enabled = false;
        partTime = 1.0f;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

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

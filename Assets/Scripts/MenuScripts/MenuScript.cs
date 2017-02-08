using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        Time.timeScale = 1;
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
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
        foreach (PlayerState item in GlobalReferences.PlayerStates)
        {
            GamePadManager.Disconnect(item.Index);
        }
        GlobalReferences.PlayerStates.Clear();
        SceneManager.LoadScene("PlayerAssignment");
    }

    public void PressCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}

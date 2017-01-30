using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MenuScript : MonoBehaviour
{
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

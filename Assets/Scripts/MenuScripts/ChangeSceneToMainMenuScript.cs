using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneToMainMenuScript : MonoBehaviour
{
    private float mainTime = 30.0f;  

    // Update is called once per frame
    private void Update()
    {
        mainTime -= Time.deltaTime;
        if (mainTime <= 0.0f || Input.anyKey)
        {
            ChangeSceneToMain();
        }
    }

    private void ChangeSceneToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


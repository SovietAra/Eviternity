using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneToMainMenuScript : MonoBehaviour {
    public float mainTime = 36.0f;
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


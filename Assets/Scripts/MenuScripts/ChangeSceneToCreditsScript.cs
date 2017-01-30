using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class ChangeSceneToCreditsScript : MonoBehaviour
{
    private float creditTime = 30.0f;
    private float inputTime = 0.0f;

    // Update is called once per frame
    private void Update()
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

    private void ChangeSceneToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

}

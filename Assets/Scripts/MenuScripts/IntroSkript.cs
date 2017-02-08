/* 
 * Purpose: pielt IntroSewuenz ab und springt danach oder bei PlayerInput zur LoadingScene
 * Author: Gregor von Frankenberg
 * Date: 8.2.2017
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class IntroSkript : MonoBehaviour
{
    public float duration;
    public MovieTexture intro;
    public RawImage Intro;
    GamePadState[] prevState = new GamePadState[4];
    public float elapsedTime;

    // Use this for initialization
    void Start()
    {
        elapsedTime = 0;
        duration = intro.duration;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (intro != null)
        {
            GetComponent<RawImage>().texture = intro as MovieTexture;
            intro.Play();


        }
        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed ||
            elapsedTime >= duration)
        {
            SceneManager.LoadScene("Loading");
        }
    }
}


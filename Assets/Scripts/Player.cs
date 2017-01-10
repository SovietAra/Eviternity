using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
public class Player : MonoBehaviour
{
    private PlayerIndex index;
    private bool hasPlayerIndex;
    private GamePadState prevState;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!hasPlayerIndex)
        {
            index = GamePadManager.GetPlayerIndex();
            hasPlayerIndex = true;
        }
        else
        {
            GamePadState state = GamePad.GetState(index);
            if (state.IsConnected)
            {
                transform.position += transform.forward * state.ThumbSticks.Left.Y * Time.deltaTime;
                transform.position += transform.right * state.ThumbSticks.Left.X * Time.deltaTime;

                transform.localRotation *= Quaternion.Euler(0.0f, state.ThumbSticks.Right.X * 25.0f * Time.deltaTime, 0.0f);
            }
            else
            {
                GamePadManager.ConnectionLost(index);
                hasPlayerIndex = false;
            }
        }
	}
}

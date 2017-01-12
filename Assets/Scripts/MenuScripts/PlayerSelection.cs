using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class PlayerSelection : MonoBehaviour
{
	// Use this for initialization

    private bool gameStarted;

	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        if (!gameStarted)
        {
            CheckPlayerJoins();

            int readyCount = CheckPlayerInput();
            if (readyCount == GlobalReferences.PlayerStates.Count && readyCount > 0)
            {
                StartNewGame();
            }
        }
	}

    private void CheckPlayerJoins()
    {
        PlayerIndex[] freePads = GamePadManager.GetFreeControllers();
        for (int i = 0; i < freePads.Length; i++)
        {
            PlayerIndex index = freePads[i];
            GamePadState state = GamePad.GetState(index);
            if (state.Buttons.A == ButtonState.Pressed)
            {
                GamePadManager.Connect((int)index);
                GlobalReferences.PlayerStates.Add(new PlayerState(index, state));
                //TODO: Change menu
            }
        }
    }

    private int CheckPlayerInput()
    {
        int readyCount = 0;
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            GamePadState state = GamePad.GetState(GlobalReferences.PlayerStates[i].Index);
            if (state.Buttons.Start == ButtonState.Pressed && GlobalReferences.PlayerStates[i].PrevState.Buttons.Start == ButtonState.Released)
            {
                GlobalReferences.PlayerStates[i] = new PlayerState(GlobalReferences.PlayerStates[i], state, !GlobalReferences.PlayerStates[i].Ready, GlobalReferences.PlayerStates[i].ClassId);
                //TODO: Change menu
            }
            
            if(state.Buttons.B == ButtonState.Pressed)
            {
                GamePadManager.Disconnect(GlobalReferences.PlayerStates[i].Index);
                GlobalReferences.PlayerStates.RemoveAt(i);
                //TODO: Change menu
            }

            //TODO: Change classes here

            if (GlobalReferences.PlayerStates[i].Ready)
                readyCount++;
        }

        return readyCount;
    }

    private void StartNewGame()
    {
        gameStarted = true;
        SceneManager.LoadScene("PlayerTestScene");
    }
}

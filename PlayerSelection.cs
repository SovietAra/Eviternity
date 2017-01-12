using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerSelection : MonoBehaviour
{
	// Use this for initialization

    private bool gameStarted;
	public Canvas Player1join;
	public Canvas Player2join;
	public Canvas Player3join;
	public Canvas Player4join;
	public Canvas Player1assigned;
	public Canvas Player2assigned;
	public Canvas Player3assigned;
	public Canvas Player4assigned;

	void Start ()
    {
	Player1assigned.enabled = false;
	Player2assigned.enabled = false;
	Player3assigned.enabled = false;
	Player4assigned.enabled = false;
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
				if(index == PlayerIndex.One)
				{
					Player1join.enabled = false;
					Player1assigned.enabled = true;
				}

				if(index == PlayerIndex.Two)
				{
					Player2join.enabled = false;
					Player2assigned.enabled = true;
				}

				if(index == PlayerIndex.Three)
				{
					Player3join.enabled = false;
					Player3assigned.enabled = true;
				}

				if(index == PlayerIndex.Four)
				{
					Player4join.enabled = false;
					Player4assigned.enabled = true;
				}
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
				if(GlobalReferences.PlayerStates[i].Index==PlayerIndex.One)
				{
				Player1join.enabled = true;
				Player1assigned.enabled = false;
				}

				if(GlobalReferences.PlayerStates[i].Index==PlayerIndex.Two)
				{
				Player2join.enabled = true;
				Player2assigned.enabled = false;
				}

				if(GlobalReferences.PlayerStates[i].Index==PlayerIndex.Three)
				{
				Player3join.enabled = true;
				Player3assigned.enabled = false;
				}

				if(GlobalReferences.PlayerStates[i].Index==PlayerIndex.Four)
				{
				Player4join.enabled = true;
				Player4assigned.enabled = false;
				}

                GamePadManager.Disconnect(GlobalReferences.PlayerStates[i].Index);
                GlobalReferences.PlayerStates.RemoveAt(i);
				
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

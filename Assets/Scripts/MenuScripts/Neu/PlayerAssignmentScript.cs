using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerAssignmentScript : MonoBehaviour {

    private bool gameStarted;
    public Canvas playerAssignmentScreen;

    public Text playerOneJoin;
    public Text playerOneAssigned;

    public Text playerTwoJoin;
    public Text playerTwoAssigned;

    public Text playerThreeJoin;
    public Text playerThreeAssigned;

    public Text playerFourJoin;
    public Text playerFourAssigned;
    // Use this for initialization
    void Start ()
    {
        playerAssignmentScreen = playerAssignmentScreen.GetComponent<Canvas>();

        playerOneJoin = playerOneJoin.GetComponent<Text>();
        playerOneAssigned = playerOneAssigned.GetComponent<Text>();

        playerTwoJoin = playerTwoJoin.GetComponent<Text>();
        playerTwoAssigned = playerTwoAssigned.GetComponent<Text>();

        playerThreeJoin = playerThreeJoin.GetComponent<Text>();
        playerThreeAssigned = playerThreeAssigned.GetComponent<Text>();

        playerFourJoin = playerFourJoin.GetComponent<Text>();
        playerFourAssigned = playerFourAssigned.GetComponent<Text>();
        
        playerOneJoin.enabled = true;
        playerOneAssigned.enabled = false;

        playerTwoJoin.enabled = true;
        playerTwoAssigned.enabled = false;

        playerThreeJoin.enabled = true;
        playerThreeAssigned.enabled = false;

        playerFourJoin.enabled = true;
        playerFourAssigned.enabled = false;

        playerAssignmentScreen.enabled = true;
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
                if (index == PlayerIndex.One)
                {
                    playerOneJoin.enabled = false;
                    playerOneAssigned.enabled = true;
                }

                if (index == PlayerIndex.Two)
                {
                    playerTwoJoin.enabled = false;
                    playerTwoAssigned.enabled = true;
                }

                if (index == PlayerIndex.Three)
                {
                    playerThreeJoin.enabled = false;
                    playerThreeAssigned.enabled = true;
                }

                if (index == PlayerIndex.Four)
                {
                    playerFourJoin.enabled = false;
                    playerFourAssigned.enabled = true;
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
                if (playerAssignmentScreen.enabled == true)
                    GlobalReferences.PlayerStates[i] = new PlayerState(GlobalReferences.PlayerStates[i], state, !GlobalReferences.PlayerStates[i].Ready, GlobalReferences.PlayerStates[i].ClassId);
                //TODO: Change menu

            }

            if (state.Buttons.B == ButtonState.Pressed)
            {
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.One)
                {
                    playerOneJoin.enabled = true;
                    playerOneAssigned.enabled = false;
                }

                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Two)
                {
                    playerTwoJoin.enabled = true;
                    playerTwoAssigned.enabled = false;
                }

                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Three)
                {
                    playerThreeJoin.enabled = true;
                    playerThreeAssigned.enabled = false;

                }

                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Four)
                {
                    playerFourJoin.enabled = true;
                    playerFourAssigned.enabled = false;
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

        SceneManager.LoadScene("PrototypeScene");
    }

    public void PressBackToPlayerAssignment()
    {
        SceneManager.LoadScene("PlayerAssignment");
    }

    public void PressStartGame()
    {
        StartNewGame();
    }

    public void PressNext()
    {
        SceneManager.LoadScene("CharacterSelection");
    }
}

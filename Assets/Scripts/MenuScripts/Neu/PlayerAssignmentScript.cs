using Assets.Scripts;
using System;
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

    public Canvas charSelectPlayerOne;
    public Image playerOneAegis;
    public Image playerOneStalker;

    public Canvas charSelectPlayerTwo;
    public Image playerTwoAegis;
    public Image playerTwoStalker;

    public Canvas charSelectPlayerThree;
    public Image playerThreeAegis;
    public Image playerThreeStalker;

    public Canvas charSelectPlayerFour;
    public Image playerFourAegis;
    public Image playerFourStalker;

    public Dictionary<int, List <Image>> ImageList = new Dictionary<int, List<Image>>();

    GamePadState[] prevState = new GamePadState[4];

    int[] index = new int[4] { 0, 0, 0, 0 };
    int[] prevIndex = new int[4] { 0, 0, 0, 0 };
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

        charSelectPlayerOne = charSelectPlayerOne.GetComponent<Canvas>();
        playerOneAegis = playerOneAegis.GetComponent<Image>();
        playerOneStalker = playerOneStalker.GetComponent<Image>();
        charSelectPlayerOne.enabled = false;

        charSelectPlayerTwo = charSelectPlayerTwo.GetComponent<Canvas>();
        playerTwoAegis = playerTwoAegis.GetComponent<Image>();
        playerTwoStalker = playerTwoStalker.GetComponent<Image>();
        charSelectPlayerTwo.enabled = false;

        charSelectPlayerThree = charSelectPlayerThree.GetComponent<Canvas>();
        playerThreeAegis = playerThreeAegis.GetComponent<Image>();
        playerThreeStalker = playerThreeStalker.GetComponent<Image>();
        charSelectPlayerThree.enabled = false;

        charSelectPlayerFour = charSelectPlayerFour.GetComponent<Canvas>();
        playerFourAegis = playerFourAegis.GetComponent<Image>();
        playerFourStalker = playerFourStalker.GetComponent<Image>();
        charSelectPlayerFour.enabled = false;

        List<Image> TempImageList = new List<Image>();
        TempImageList.Add(playerOneAegis);
        TempImageList.Add(playerOneStalker);
        ImageList.Add(0, TempImageList);

        TempImageList = new List<Image>();
        TempImageList.Add(playerTwoAegis);
        TempImageList.Add(playerTwoStalker);
        ImageList.Add(1, TempImageList);

        TempImageList = new List<Image>();
        TempImageList.Add(playerThreeAegis);
        TempImageList.Add(playerThreeStalker);
        ImageList.Add(2, TempImageList);

        TempImageList = new List<Image>();
        TempImageList.Add(playerFourAegis);
        TempImageList.Add(playerFourStalker);
        ImageList.Add(3, TempImageList);
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
                    charSelectPlayerOne.enabled = true;
                }

                if (index == PlayerIndex.Two)
                {
                    playerTwoJoin.enabled = false;
                    playerTwoAssigned.enabled = true;
                    charSelectPlayerTwo.enabled = true;
                }

                if (index == PlayerIndex.Three)
                {
                    playerThreeJoin.enabled = false;
                    playerThreeAssigned.enabled = true;
                    charSelectPlayerThree.enabled = true;
                }

                if (index == PlayerIndex.Four)
                {
                    playerFourJoin.enabled = false;
                    playerFourAssigned.enabled = true;
                    charSelectPlayerFour.enabled = true;
                }
            }
        }
    }

    private int CheckPlayerInput()
    {
        int readyCount = 0;
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            int playerIndex = (int)GlobalReferences.PlayerStates[i].Index;
            GamePadState state = GamePad.GetState(GlobalReferences.PlayerStates[i].Index);
            if (state.Buttons.Start == ButtonState.Pressed && GlobalReferences.PlayerStates[i].PrevState.Buttons.Start == ButtonState.Released)
            {
                if (playerAssignmentScreen.enabled == true)
                    GlobalReferences.PlayerStates[i] = new PlayerState(GlobalReferences.PlayerStates[i], state, !GlobalReferences.PlayerStates[i].Ready, GlobalReferences.PlayerStates[i].ClassId);
                //TODO: Change menu

            }
            if(state.DPad.Left == ButtonState.Pressed && prevState[playerIndex].DPad.Left == ButtonState.Released)
            {
                    prevIndex[playerIndex] = index[playerIndex];
                    index[playerIndex]++;
                    if(index[playerIndex] >= ImageList.Count)
                    {
                        index[playerIndex] = 0;
                    }

                ChangeImage(playerIndex, index[playerIndex], prevIndex[playerIndex]);
            }

            if (state.DPad.Right == ButtonState.Pressed && prevState[playerIndex].DPad.Right == ButtonState.Released)
            {
                    prevIndex[playerIndex] = index[playerIndex];
                    index[playerIndex]--;
                    if (index[playerIndex] < 0)
                    {
                    index[playerIndex] = ImageList.Count - 1;
                    }
                ChangeImage(playerIndex, index[playerIndex], prevIndex[playerIndex]);
            }
            
            if (state.Buttons.B == ButtonState.Pressed)
            {
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.One)
                {
                    playerOneJoin.enabled = true;
                    playerOneAssigned.enabled = false;
                    charSelectPlayerOne.enabled = false;
                }

                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Two)
                {
                    playerTwoJoin.enabled = true;
                    playerTwoAssigned.enabled = false;
                    charSelectPlayerTwo.enabled = false;
                }
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Three)
                {
                    playerThreeJoin.enabled = true;
                    playerThreeAssigned.enabled = false;
                    charSelectPlayerThree.enabled = false;
                }
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Four)
                {
                    playerFourJoin.enabled = true;
                    playerFourAssigned.enabled = false;
                    charSelectPlayerFour.enabled = false;
                }
                GamePadManager.Disconnect(GlobalReferences.PlayerStates[i].Index);
                GlobalReferences.PlayerStates.RemoveAt(i);
            }
            //TODO: Change classes here
            if (GlobalReferences.PlayerStates[i].Ready)
                readyCount++;

            prevState[playerIndex] = state;
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
        SceneManager.LoadScene("PrototypeScene");
    }

    public void ChangeImage(int playerIndex, int index, int prevIndex)
    {
        ImageList[playerIndex][index].enabled = true;
        ImageList[playerIndex][prevIndex].enabled = false;
    }
}
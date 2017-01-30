/* 
 * Purpose: Händelt die Character- und Waffenzuweisung
 * Author: Gregor von Frankenberg
 * Date: 27.01.2017
 */


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

    int[] index = new int[2] { 0, 0 };
    int[] prevIndex = new int[2] { 0, 0 };

    public Canvas weaponSelectionPlayerOne;
    public Image player1_weapon1_left;
    public Image player1_weapon2_left;
    public Image player1_weapon3_left;

    public Image player1_weapon1_right;
    public Image player1_weapon2_right;
    public Image player1_weapon3_right;

    public Canvas weaponSelectionPlayerTwo;
    public Image player2_weapon1_left;
    public Image player2_weapon2_left;
    public Image player2_weapon3_left;

    public Image player2_weapon1_right;
    public Image player2_weapon2_right;
    public Image player2_weapon3_right;

    public Canvas weaponSelectionPlayerThree;
    public Image player3_weapon1_left;
    public Image player3_weapon2_left;
    public Image player3_weapon3_left;

    public Image player3_weapon1_right;
    public Image player3_weapon2_right;
    public Image player3_weapon3_right;

    public Canvas weaponSelectionPlayerFour;
    public Image player4_weapon1_left;
    public Image player4_weapon2_left;
    public Image player4_weapon3_left;

    public Image player4_weapon1_right;
    public Image player4_weapon2_right;
    public Image player4_weapon3_right;

    public Dictionary<int, List<Image>> WeaponListLeft = new Dictionary<int, List<Image>>();
    public Dictionary<int, List<Image>> WeaponListRight = new Dictionary<int, List<Image>>();

    int[] leftWeaponIndex = new int[3] { 0, 0, 0 };
    int[] leftWeaponPrevIndex = new int[3] { 0, 0, 0 };

    int[] rightWeaponIndex = new int[3] { 0, 0, 0 };
    int[] rightWeaponPrevIndex = new int[3] { 0, 0, 0 };
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

        weaponSelectionPlayerOne = weaponSelectionPlayerOne.GetComponent<Canvas>();
        player1_weapon1_left = player1_weapon1_left.GetComponent<Image>();
        player1_weapon2_left = player1_weapon2_left.GetComponent<Image>();
        player1_weapon3_left = player1_weapon3_left.GetComponent<Image>();

        player1_weapon1_right = player1_weapon1_right.GetComponent<Image>();
        player1_weapon2_right = player1_weapon2_right.GetComponent<Image>();
        player1_weapon3_right = player1_weapon3_right.GetComponent<Image>();
        weaponSelectionPlayerOne.enabled = true;

        weaponSelectionPlayerTwo = weaponSelectionPlayerTwo.GetComponent<Canvas>();
        player2_weapon1_left = player2_weapon1_left.GetComponent<Image>();
        player2_weapon2_left = player2_weapon2_left.GetComponent<Image>();
        player2_weapon3_left = player2_weapon3_left.GetComponent<Image>();

        player2_weapon1_right = player2_weapon1_right.GetComponent<Image>();
        player2_weapon2_right = player2_weapon2_right.GetComponent<Image>();
        player2_weapon3_right = player2_weapon3_right.GetComponent<Image>();
        weaponSelectionPlayerTwo.enabled = true;

        weaponSelectionPlayerThree = weaponSelectionPlayerThree.GetComponent<Canvas>();
        player3_weapon1_left = player3_weapon1_left.GetComponent<Image>();
        player3_weapon2_left = player3_weapon2_left.GetComponent<Image>();
        player3_weapon3_left = player3_weapon3_left.GetComponent<Image>();

        player3_weapon1_right = player3_weapon1_right.GetComponent<Image>();
        player3_weapon2_right = player3_weapon2_right.GetComponent<Image>();
        player3_weapon3_right = player3_weapon3_right.GetComponent<Image>();
        weaponSelectionPlayerThree.enabled = true;

        weaponSelectionPlayerFour = weaponSelectionPlayerFour.GetComponent<Canvas>();
        player4_weapon1_left = player4_weapon1_left.GetComponent<Image>();
        player4_weapon2_left = player4_weapon2_left.GetComponent<Image>();
        player4_weapon3_left = player4_weapon3_left.GetComponent<Image>();

        player4_weapon1_right = player4_weapon1_right.GetComponent<Image>();
        player4_weapon2_right = player4_weapon2_right.GetComponent<Image>();
        player4_weapon3_right = player4_weapon3_right.GetComponent<Image>();
        weaponSelectionPlayerFour.enabled = true;

        List<Image> TempWeaponListLeft = new List<Image>();
        TempWeaponListLeft.Add(player1_weapon1_left);
        TempWeaponListLeft.Add(player1_weapon2_left);
        TempWeaponListLeft.Add(player1_weapon3_left);
        WeaponListLeft.Add(0, TempWeaponListLeft);

        List<Image> TempWeaponListRight = new List<Image>();
        TempWeaponListRight.Add(player1_weapon1_right);
        TempWeaponListRight.Add(player1_weapon2_right);
        TempWeaponListRight.Add(player1_weapon3_right);
        WeaponListRight.Add(0, TempWeaponListRight);
        ////////////////////////////////////////////
        TempWeaponListLeft = new List<Image>();
        TempWeaponListLeft.Add(player2_weapon1_left);
        TempWeaponListLeft.Add(player2_weapon2_left);
        TempWeaponListLeft.Add(player2_weapon3_left);
        WeaponListLeft.Add(1, TempWeaponListLeft);

        TempWeaponListRight = new List<Image>();
        TempWeaponListRight.Add(player2_weapon1_right);
        TempWeaponListRight.Add(player2_weapon2_right);
        TempWeaponListRight.Add(player2_weapon3_right);
        WeaponListRight.Add(1, TempWeaponListRight);

        TempWeaponListLeft = new List<Image>();
        TempWeaponListLeft.Add(player3_weapon1_left);
        TempWeaponListLeft.Add(player3_weapon2_left);
        TempWeaponListLeft.Add(player3_weapon3_left);
        WeaponListLeft.Add(2, TempWeaponListLeft);

        TempWeaponListRight = new List<Image>();
        TempWeaponListRight.Add(player3_weapon1_right);
        TempWeaponListRight.Add(player3_weapon2_right);
        TempWeaponListRight.Add(player3_weapon3_right);
        WeaponListRight.Add(2, TempWeaponListRight);

        TempWeaponListLeft = new List<Image>();
        TempWeaponListLeft.Add(player4_weapon1_left);
        TempWeaponListLeft.Add(player4_weapon2_left);
        TempWeaponListLeft.Add(player4_weapon3_left);
        WeaponListLeft.Add(3, TempWeaponListLeft);

        TempWeaponListRight = new List<Image>();
        TempWeaponListRight.Add(player4_weapon1_right);
        TempWeaponListRight.Add(player4_weapon2_right);
        TempWeaponListRight.Add(player4_weapon3_right);
        WeaponListRight.Add(3, TempWeaponListRight);
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
                    //weaponSelectionPlayerOne.enabled = true;
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
                index[playerIndex] -= 1;
                Debug.Log("index:" + index[playerIndex]);
                if (index[playerIndex] < 0 )
                    {
                        Debug.Log("Sprung auf Ende der List");
                        index[playerIndex] = 1;
                    }

                ChangeImage(playerIndex, index[playerIndex], prevIndex[playerIndex]);
            }

            if (state.DPad.Right == ButtonState.Pressed && prevState[playerIndex].DPad.Right == ButtonState.Released)
            {
                prevIndex[playerIndex] = index[playerIndex];
                index[playerIndex] += 1;
                Debug.Log("index:" + index[playerIndex]);
                if (index[playerIndex] > 1)
                {
                    Debug.Log("Sprung auf Anfang der Liste");
                    index[playerIndex] = 0;
                }
                ChangeImage(playerIndex, index[playerIndex], prevIndex[playerIndex]);
            }
            
            if (state.Buttons.LeftShoulder == ButtonState.Pressed && prevState[playerIndex].Buttons.LeftShoulder == ButtonState.Released)
            {
                leftWeaponPrevIndex[playerIndex] = leftWeaponIndex[playerIndex];
                leftWeaponIndex[playerIndex] += 1;
                Debug.Log("index:" + leftWeaponIndex[playerIndex]);
                if (leftWeaponIndex[playerIndex] > 2)
                {
                    Debug.Log("Links\nSprung auf Anfang der Liste");
                    leftWeaponIndex[playerIndex] = 0;
                }
                ChangeLeftWeapon(playerIndex, leftWeaponIndex[playerIndex], leftWeaponPrevIndex[playerIndex]);
            }

            if (state.Triggers.Left > 0.8 && prevState[playerIndex].Triggers.Left < 0.8)
            {
                leftWeaponPrevIndex[playerIndex] = leftWeaponIndex[playerIndex];
                leftWeaponIndex[playerIndex] -= 1;
                Debug.Log("index:" + leftWeaponIndex[playerIndex]);
                if (leftWeaponIndex[playerIndex] < 0)
                {
                    Debug.Log("Links\nSprung auf Anfang der Liste");
                    leftWeaponIndex[playerIndex] = 2;
                }
                ChangeLeftWeapon(playerIndex, leftWeaponIndex[playerIndex], leftWeaponPrevIndex[playerIndex]);
            }

            if (state.Buttons.RightShoulder == ButtonState.Pressed && prevState[playerIndex].Buttons.RightShoulder == ButtonState.Released)
            {
                rightWeaponPrevIndex[playerIndex] = rightWeaponIndex[playerIndex];
                rightWeaponIndex[playerIndex] += 1;
                Debug.Log("index:" + rightWeaponIndex[playerIndex]);
                if (rightWeaponIndex[playerIndex] > 2)
                {
                    Debug.Log("Rechts\nSprung auf Anfang der Liste");
                    rightWeaponIndex[playerIndex] = 0;
                }
                ChangeRightWeapon(playerIndex, rightWeaponIndex[playerIndex], rightWeaponPrevIndex[playerIndex]);
            }

            if (state.Triggers.Right > 0.8 && prevState[playerIndex].Triggers.Right < 0.8)
            {
                rightWeaponPrevIndex[playerIndex] = rightWeaponIndex[playerIndex];
                rightWeaponIndex[playerIndex] -= 1;
                Debug.Log("index:" + rightWeaponIndex[playerIndex]);
                if (rightWeaponIndex[playerIndex] < 0)
                {
                    Debug.Log("Rechts\nSprung auf Anfang der Liste");
                    rightWeaponIndex[playerIndex] = 2;
                }
                ChangeRightWeapon(playerIndex, rightWeaponIndex[playerIndex], rightWeaponPrevIndex[playerIndex]);
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

        SceneManager.LoadScene(7);
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
        SceneManager.LoadScene(7);
    }

    public void ChangeImage(int playerIndex, int index, int prevIndex)
    {
        ImageList[playerIndex][index].enabled = true;
        ImageList[playerIndex][prevIndex].enabled = false;
    }

    public void ChangeLeftWeapon(int playerIndex, int index, int prevIndex)
    {
        WeaponListLeft[playerIndex][index].enabled = true;
        WeaponListLeft[playerIndex][prevIndex].enabled = false;
    }

    public void ChangeRightWeapon(int playerIndex, int index, int prevIndex)
    {
        WeaponListRight[playerIndex][index].enabled = true;
        WeaponListRight[playerIndex][prevIndex].enabled = false;
    }
}
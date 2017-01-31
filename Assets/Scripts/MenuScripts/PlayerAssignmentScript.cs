/* 
 * Purpose: Händelt die Character- und Waffenzuweisung
 * Author: Marcel Croonenbroeck / Gregor von Frankenberg
 * Date: 31.01.2017
 */


using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerAssignmentScript : MonoBehaviour {

    #region GameObjects
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

    public Canvas weaponSelectionPlayerOne;

    public Canvas weaponSelectionPlayerTwo;

    public Canvas weaponSelectionPlayerThree;

    public Canvas weaponSelectionPlayerFour;

    public Image player1_Aegis_Bohrer;
    public Image player1_Aegis_GGun;
    public Image player1_Aegis_Granatenwerfer;
    public Image player1_Aegis_Flammenwerfer;

    public Image player2_Aegis_Bohrer;
    public Image player2_Aegis_GGun;
    public Image player2_Aegis_Granatenwerfer;
    public Image player2_Aegis_Flammenwerfer;

    public Image player3_Aegis_Bohrer;
    public Image player3_Aegis_GGun;
    public Image player3_Aegis_Granatenwerfer;
    public Image player3_Aegis_Flammenwerfer;

    public Image player4_Aegis_Bohrer;
    public Image player4_Aegis_GGun;
    public Image player4_Aegis_Granatenwerfer;
    public Image player4_Aegis_Flammenwerfer;

    public Image player1_Stalker_Sniper;
    public Image player1_Stalker_EMP_Gun;
    public Image player1_Stalker_Schwert;
    public Image player1_Stalker_Shotgun;

    public Image player2_Stalker_Sniper;
    public Image player2_Stalker_EMP_Gun;
    public Image player2_Stalker_Schwert;
    public Image player2_Stalker_Shotgun;

    public Image player3_Stalker_Sniper;
    public Image player3_Stalker_EMP_Gun;
    public Image player3_Stalker_Schwert;
    public Image player3_Stalker_Shotgun;

    public Image player4_Stalker_Sniper;
    public Image player4_Stalker_EMP_Gun;
    public Image player4_Stalker_Schwert;
    public Image player4_Stalker_Shotgun;
    #endregion

    #region Listen und ihre ints
    public Dictionary<int, List <Image>> ImageList = new Dictionary<int, List<Image>>();

    GamePadState[] prevState = new GamePadState[4];

    int[] index = new int[2] { 0, 0 };
    int[] prevIndex = new int[2] { 0, 0 };
    
    public Dictionary<int, List<Image>> Weaponset_Aegis_links = new Dictionary<int, List<Image>>();
    public Dictionary<int, List<Image>> Weaponset_Aegis_rechts = new Dictionary<int, List<Image>>();

    int[] AegisIndex_links = new int[2] { 0, 0 };
    int[] AegisPrevIndex_links = new int[2] { 0, 0 };

    int[] AegisIndex_rechts = new int[2] { 0, 0 };
    int[] AegisPrevIndex_rechts = new int[2] { 0, 0 };

    

    public Dictionary<int, List<Image>> Weaponset_Stalker_links = new Dictionary<int, List<Image>>();
    public Dictionary<int, List<Image>> Weaponset_Stalker_rechts = new Dictionary<int, List<Image>>();

    int[] StalkerIndex_links = new int[2] { 0, 0 };
    int[] StalkerPrevIndex_links = new int[2] { 0, 0 };

    int[] StalkerIndex_rechts = new int[2] { 0, 0 };
    int[] StalkerPrevIndex_rechts = new int[2] { 0, 0 };
    #endregion
    


    bool changeMenu = false;
    // Use this for initialization
    void Start ()
    {
        #region GameObjects getter
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
        #endregion

        #region Characterlisten
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
        weaponSelectionPlayerOne.enabled = true;

        weaponSelectionPlayerTwo = weaponSelectionPlayerTwo.GetComponent<Canvas>();
        weaponSelectionPlayerTwo.enabled = true;

        weaponSelectionPlayerThree = weaponSelectionPlayerThree.GetComponent<Canvas>();
        weaponSelectionPlayerThree.enabled = true;

        weaponSelectionPlayerFour = weaponSelectionPlayerFour.GetComponent<Canvas>();
        weaponSelectionPlayerFour.enabled = true;
        #endregion

        #region Waffenlisten
        List<Image> TempWeaponset_Aegis_links = new List<Image>();
        TempWeaponset_Aegis_links.Add(player1_Aegis_GGun);
        TempWeaponset_Aegis_links.Add(player1_Aegis_Granatenwerfer);
        Weaponset_Aegis_links.Add(0, TempWeaponset_Aegis_links);

        TempWeaponset_Aegis_links = new List<Image>();
        TempWeaponset_Aegis_links.Add(player2_Aegis_GGun);
        TempWeaponset_Aegis_links.Add(player2_Aegis_Granatenwerfer);
        Weaponset_Aegis_links.Add(1, TempWeaponset_Aegis_links);

        TempWeaponset_Aegis_links = new List<Image>();
        TempWeaponset_Aegis_links.Add(player3_Aegis_GGun);
        TempWeaponset_Aegis_links.Add(player3_Aegis_Granatenwerfer);
        Weaponset_Aegis_links.Add(2, TempWeaponset_Aegis_links);

        TempWeaponset_Aegis_links = new List<Image>();
        TempWeaponset_Aegis_links.Add(player4_Aegis_GGun);
        TempWeaponset_Aegis_links.Add(player4_Aegis_Granatenwerfer);
        Weaponset_Aegis_links.Add(3, TempWeaponset_Aegis_links);

        List<Image> TempWeaponset_Aegis_rechts = new List<Image>();
        TempWeaponset_Aegis_rechts.Add(player1_Aegis_Bohrer);
        TempWeaponset_Aegis_rechts.Add(player1_Aegis_Flammenwerfer);
        Weaponset_Aegis_rechts.Add(0, TempWeaponset_Aegis_rechts);

        TempWeaponset_Aegis_rechts = new List<Image>();
        TempWeaponset_Aegis_rechts.Add(player2_Aegis_Bohrer);
        TempWeaponset_Aegis_rechts.Add(player2_Aegis_Flammenwerfer);
        Weaponset_Aegis_rechts.Add(1, TempWeaponset_Aegis_rechts);

        TempWeaponset_Aegis_rechts = new List<Image>();
        TempWeaponset_Aegis_rechts.Add(player3_Aegis_Bohrer);
        TempWeaponset_Aegis_rechts.Add(player3_Aegis_Flammenwerfer);
        Weaponset_Aegis_rechts.Add(2, TempWeaponset_Aegis_rechts);

        TempWeaponset_Aegis_rechts = new List<Image>();
        TempWeaponset_Aegis_rechts.Add(player4_Aegis_Bohrer);
        TempWeaponset_Aegis_rechts.Add(player4_Aegis_Flammenwerfer);
        Weaponset_Aegis_rechts.Add(3, TempWeaponset_Aegis_rechts);

        List<Image> TempWeaponset_Stalker_links = new List<Image>();
        TempWeaponset_Stalker_links.Add(player1_Stalker_Sniper);
        TempWeaponset_Stalker_links.Add(player1_Stalker_EMP_Gun);
        Weaponset_Stalker_links.Add(0, TempWeaponset_Stalker_links);

        TempWeaponset_Stalker_links = new List<Image>();
        TempWeaponset_Stalker_links.Add(player2_Stalker_Sniper);
        TempWeaponset_Stalker_links.Add(player2_Stalker_EMP_Gun);
        Weaponset_Stalker_links.Add(1, TempWeaponset_Stalker_links);

        TempWeaponset_Stalker_links = new List<Image>();
        TempWeaponset_Stalker_links.Add(player3_Stalker_Sniper);
        TempWeaponset_Stalker_links.Add(player3_Stalker_EMP_Gun);
        Weaponset_Stalker_links.Add(2, TempWeaponset_Stalker_links);

        TempWeaponset_Stalker_links = new List<Image>();
        TempWeaponset_Stalker_links.Add(player4_Stalker_Sniper);
        TempWeaponset_Stalker_links.Add(player4_Stalker_EMP_Gun);
        Weaponset_Stalker_links.Add(3, TempWeaponset_Stalker_links);

        List<Image> TempWeaponset_Stalker_rechts = new List<Image>();
        TempWeaponset_Stalker_rechts.Add(player1_Stalker_Schwert);
        TempWeaponset_Stalker_rechts.Add(player1_Stalker_Shotgun);
        Weaponset_Stalker_rechts.Add(0, TempWeaponset_Stalker_rechts);

        TempWeaponset_Stalker_rechts = new List<Image>();
        TempWeaponset_Stalker_rechts.Add(player2_Stalker_Schwert);
        TempWeaponset_Stalker_rechts.Add(player2_Stalker_Shotgun);
        Weaponset_Stalker_rechts.Add(1, TempWeaponset_Stalker_rechts);

        TempWeaponset_Stalker_rechts = new List<Image>();
        TempWeaponset_Stalker_rechts.Add(player3_Stalker_Schwert);
        TempWeaponset_Stalker_rechts.Add(player3_Stalker_Shotgun);
        Weaponset_Stalker_rechts.Add(2, TempWeaponset_Stalker_rechts);

        TempWeaponset_Stalker_rechts = new List<Image>();
        TempWeaponset_Stalker_rechts.Add(player4_Stalker_Schwert);
        TempWeaponset_Stalker_rechts.Add(player4_Stalker_Shotgun);
        Weaponset_Stalker_rechts.Add(3, TempWeaponset_Stalker_rechts);
        #endregion
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
    private void StartNewGame()
    {
        gameStarted = true;
        changeMenu = true;
        SceneManager.LoadScene("Prototyp");
    }
    #region player Funktionen
    private void CheckPlayerJoins()
    {
        PlayerIndex[] freePads = GamePadManager.GetFreeControllers();
        for (int i = 0; i < freePads.Length; i++)
        {
            PlayerIndex index = freePads[i];
            GamePadState state = GamePad.GetState(index);
            if (state.Buttons.A == ButtonState.Pressed && !changeMenu)
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
            #region Ausnamebedingungen Waffen
            if (playerOneAegis.enabled == true)
            {
                player1_Stalker_EMP_Gun.enabled = false;
                player1_Stalker_Schwert.enabled = false;
                player1_Stalker_Shotgun.enabled = false;
                player1_Stalker_Sniper.enabled = false;
            }
            if (playerTwoAegis.enabled == true)
            {
                player2_Stalker_EMP_Gun.enabled = false;
                player2_Stalker_Schwert.enabled = false;
                player2_Stalker_Shotgun.enabled = false;
                player2_Stalker_Sniper.enabled = false;
            }
            if (playerThreeAegis.enabled == true)
            {
                player3_Stalker_EMP_Gun.enabled = false;
                player3_Stalker_Schwert.enabled = false;
                player3_Stalker_Shotgun.enabled = false;
                player3_Stalker_Sniper.enabled = false;
            }
            if (playerFourAegis.enabled == true)
            {
                player4_Stalker_EMP_Gun.enabled = false;
                player4_Stalker_Schwert.enabled = false;
                player4_Stalker_Shotgun.enabled = false;
                player4_Stalker_Sniper.enabled = false;
            }

            if (playerOneStalker.enabled == true)
            {
                player1_Aegis_Bohrer.enabled = false;
                player1_Aegis_Flammenwerfer.enabled = false;
                player1_Aegis_GGun.enabled = false;
                player1_Aegis_Granatenwerfer.enabled = false;
            }
            if (playerTwoStalker.enabled == true)
            {
                player2_Aegis_Bohrer.enabled = false;
                player2_Aegis_Flammenwerfer.enabled = false;
                player2_Aegis_GGun.enabled = false;
                player2_Aegis_Granatenwerfer.enabled = false;
            }
            if (playerThreeStalker.enabled == true)
            {
                player3_Aegis_Bohrer.enabled = false;
                player3_Aegis_Flammenwerfer.enabled = false;
                player3_Aegis_GGun.enabled = false;
                player3_Aegis_Granatenwerfer.enabled = false;
            }
            if (playerFourStalker.enabled == true)
            {
                player4_Aegis_Bohrer.enabled = false;
                player4_Aegis_Flammenwerfer.enabled = false;
                player4_Aegis_GGun.enabled = false;
                player4_Aegis_Granatenwerfer.enabled = false;
            }
            #endregion
            if (state.Buttons.LeftShoulder == ButtonState.Pressed && prevState[playerIndex].Buttons.LeftShoulder == ButtonState.Released)
            {
                if (playerOneAegis.enabled == true || playerTwoAegis.enabled == true || playerThreeAegis.enabled == true || playerFourAegis.enabled == true)
                {
                    AegisPrevIndex_links[playerIndex] = AegisIndex_links[playerIndex];
                    AegisIndex_links[playerIndex] += 1;
                    Debug.Log("AegisIndex_links:" + AegisIndex_links[playerIndex]);
                    if (AegisIndex_links[playerIndex] >= 2)
                    {
                        Debug.Log("Links\nSprung auf Anfang der Liste");
                        AegisIndex_links[playerIndex] = 0;
                    }
                    ChangeLeftWeaponAegis(playerIndex, AegisIndex_links[playerIndex], AegisPrevIndex_links[playerIndex]);
                }

                if (playerOneStalker.enabled == true || playerTwoStalker.enabled == true || playerThreeStalker.enabled == true || playerFourStalker.enabled == true)
                {
                    StalkerPrevIndex_links[playerIndex] = StalkerIndex_links[playerIndex];
                    StalkerIndex_links[playerIndex] += 1;
                    Debug.Log("StalkerIndex_links:" + StalkerIndex_links[playerIndex]);
                    if (StalkerIndex_links[playerIndex] >= 2)
                    {
                        Debug.Log("Links\nSprung auf Anfang der Liste");
                        StalkerIndex_links[playerIndex] = 0;
                    }
                    ChangeLeftWeaponStalker(playerIndex, StalkerIndex_links[playerIndex], StalkerPrevIndex_links[playerIndex]);
                }
            }

            if (state.Triggers.Left > 0.8 && prevState[playerIndex].Triggers.Left < 0.8)
            {
                if (playerOneAegis.enabled == true || playerTwoAegis.enabled == true || playerThreeAegis.enabled == true || playerFourAegis.enabled == true)
                {
                    AegisPrevIndex_links[playerIndex] = AegisIndex_links[playerIndex];
                    AegisIndex_links[playerIndex] -= 1;
                    Debug.Log("AegisIndex_links:" + AegisIndex_links[playerIndex]);
                    if (AegisIndex_links[playerIndex] < 0)
                    {
                        Debug.Log("Links\nSprung auf Anfang der Liste");
                        AegisIndex_links[playerIndex] = 1;
                    }
                    ChangeLeftWeaponAegis(playerIndex, AegisIndex_links[playerIndex], AegisPrevIndex_links[playerIndex]);
                }

                if (playerOneStalker.enabled == true || playerTwoStalker.enabled == true || playerThreeStalker.enabled == true || playerFourStalker.enabled == true)
                {
                    StalkerPrevIndex_links[playerIndex] = StalkerIndex_links[playerIndex];
                    StalkerIndex_links[playerIndex] -= 1;
                    Debug.Log("StalkerIndex_links:" + StalkerIndex_links[playerIndex]);
                    if (StalkerIndex_links[playerIndex] < 0)
                    {
                        Debug.Log("Links\nSprung auf Anfang der Liste");
                        StalkerIndex_links[playerIndex] = 1;
                    }
                    ChangeLeftWeaponStalker(playerIndex, StalkerIndex_links[playerIndex], StalkerPrevIndex_links[playerIndex]);
                }
            }

            if (state.Buttons.RightShoulder == ButtonState.Pressed && prevState[playerIndex].Buttons.RightShoulder == ButtonState.Released)
            {
                if (playerOneAegis.enabled == true || playerTwoAegis.enabled == true || playerThreeAegis.enabled == true || playerFourAegis.enabled == true)
                {
                    AegisPrevIndex_rechts[playerIndex] = AegisIndex_rechts[playerIndex];
                    AegisIndex_rechts[playerIndex] += 1;
                    Debug.Log("AegisIndex_rechts:" + AegisIndex_rechts[playerIndex]);
                    if (AegisIndex_rechts[playerIndex] >= 2)
                    {
                        Debug.Log("Rechts\nSprung auf Anfang der Liste");
                        AegisIndex_rechts[playerIndex] = 0;
                    }
                    ChangeRightWeaponAegis(playerIndex, AegisIndex_rechts[playerIndex], AegisPrevIndex_rechts[playerIndex]);
                }

                if (playerOneStalker.enabled == true || playerTwoStalker.enabled == true || playerThreeStalker.enabled == true || playerFourStalker.enabled == true)
                {
                    StalkerPrevIndex_rechts[playerIndex] = StalkerIndex_rechts[playerIndex];
                    StalkerIndex_rechts[playerIndex] += 1;
                    Debug.Log("StalkerIndex_rechts:" + StalkerIndex_rechts[playerIndex]);
                    if (StalkerIndex_rechts[playerIndex] >= 2)
                    {
                        Debug.Log("Rechts\nSprung auf Anfang der Liste");
                        StalkerIndex_rechts[playerIndex] = 0;
                    }
                    ChangeRightWeaponStalker(playerIndex, StalkerIndex_rechts[playerIndex], StalkerPrevIndex_rechts[playerIndex]);
                }
            }

            if (state.Triggers.Right > 0.8 && prevState[playerIndex].Triggers.Right < 0.8)
            {
                if (playerOneAegis.enabled == true || playerTwoAegis.enabled == true || playerThreeAegis.enabled == true || playerFourAegis.enabled == true)
                {
                    AegisPrevIndex_rechts[playerIndex] = AegisIndex_rechts[playerIndex];
                    AegisIndex_rechts[playerIndex] -= 1;
                    Debug.Log("AegisIndex_rechts:" + AegisIndex_rechts[playerIndex]);
                    if (AegisIndex_rechts[playerIndex] < 0)
                    {
                        Debug.Log("Rechts\nSprung auf Anfang der Liste");
                        AegisIndex_rechts[playerIndex] = 1;
                    }
                    ChangeRightWeaponAegis(playerIndex, AegisIndex_rechts[playerIndex], AegisPrevIndex_rechts[playerIndex]);
                }

                if (playerOneStalker.enabled == true || playerTwoStalker.enabled == true || playerThreeStalker.enabled == true || playerFourStalker.enabled == true)
                {
                    StalkerPrevIndex_rechts[playerIndex] = StalkerIndex_rechts[playerIndex];
                    StalkerIndex_rechts[playerIndex] -= 1;
                    Debug.Log("StalkerIndex_rechts:" + StalkerIndex_rechts[playerIndex]);
                    if (StalkerIndex_rechts[playerIndex] < 0)
                    {
                        Debug.Log("Rechts\nSprung auf Anfang der Liste");
                        StalkerIndex_rechts[playerIndex] = 1;
                    }
                    ChangeRightWeaponStalker(playerIndex, StalkerIndex_rechts[playerIndex], StalkerPrevIndex_rechts[playerIndex]);
                }
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
                Debug.Log(GlobalReferences.PlayerStates.Count);
            }
            //TODO: Change classes here
            if (GlobalReferences.PlayerStates[i].Ready)
                readyCount++;

            prevState[playerIndex] = state;
        }
        return readyCount;
    }
    #endregion

    #region Button Funktionen
    public void PressBackToPlayerAssignment()
    {
        changeMenu = true;
        SceneManager.LoadScene("PlayerAssignment");
    }

    public void PressBackToMain()
    {
        changeMenu = true;
        foreach (PlayerState item in GlobalReferences.PlayerStates)
        {
            GamePadManager.Disconnect(item.Index);
        }
        GlobalReferences.PlayerStates.Clear();

        SceneManager.LoadScene("MainMenu");
    }

    public void PressStartGame()
    {
        StartNewGame();
    }

    public void PressNext()
    {
        SceneManager.LoadScene("Prototyp");
    }
    #endregion

    #region -change Funktionen
    public void ChangeImage(int playerIndex, int index, int prevIndex)
    {
        ImageList[playerIndex][index].enabled = true;
        ImageList[playerIndex][prevIndex].enabled = false;
    }

    public void ChangeLeftWeaponAegis(int playerIndex, int index, int prevIndex)
    {
       Weaponset_Aegis_links[playerIndex][index].enabled = true;
       Weaponset_Aegis_links[playerIndex][prevIndex].enabled = false;
    }

    public void ChangeRightWeaponAegis(int playerIndex, int index, int prevIndex)
    {
       Weaponset_Aegis_rechts[playerIndex][index].enabled = true;
       Weaponset_Aegis_rechts[playerIndex][prevIndex].enabled = false;
    }

    public void ChangeLeftWeaponStalker(int playerIndex, int index, int prevIndex)
    {
        Weaponset_Stalker_links[playerIndex][index].enabled = true;
        Weaponset_Stalker_links[playerIndex][prevIndex].enabled = false;
    }

    public void ChangeRightWeaponStalker(int playerIndex, int index, int prevIndex)
    {
        Weaponset_Stalker_rechts[playerIndex][index].enabled = true;
        Weaponset_Stalker_rechts[playerIndex][prevIndex].enabled = false;
    }
    #endregion
}
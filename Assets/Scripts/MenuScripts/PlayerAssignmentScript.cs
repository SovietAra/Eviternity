/* 
 * Purpose: Händelt die Character- und Waffenzuweisung
 * Author: Gregor von Frankenberg / Marcel Croonenbroeck
 * Date: 2.2.2017
 */


using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerAssignmentScript : MonoBehaviour
{
    #region GameObjects
    public static bool gameStarted;
    public Canvas playerAssignmentScreen;
    
    public int[] playerChoice;

    public Text playerOneJoin;

    public Text playerTwoJoin;

    public Text playerThreeJoin;

    public Text playerFourJoin;

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

    public AudioClip ChangeClip;
    private AudioSource Change;
    #endregion

    #region Listen und ihre ints
    public Dictionary<int, List<Image>> ImageList = new Dictionary<int, List<Image>>();

    GamePadState[] prevState = new GamePadState[4];

    int[] index = new int[4] { 0, 0, 0, 0 };
    int[] prevIndex = new int[4] { 0, 0, 0, 0 };
    #endregion

    private bool[] playerJoined;

    bool changeMenu = false;
    // Use this for initialization
    void Start()
    {
        #region GameObjects getter
        playerAssignmentScreen = playerAssignmentScreen.GetComponent<Canvas>();

        playerOneJoin = playerOneJoin.GetComponent<Text>();

        playerTwoJoin = playerTwoJoin.GetComponent<Text>();

        playerThreeJoin = playerThreeJoin.GetComponent<Text>();

        playerFourJoin = playerFourJoin.GetComponent<Text>();

        playerOneJoin.enabled = true;

        playerTwoJoin.enabled = true;

        playerThreeJoin.enabled = true;

        playerFourJoin.enabled = true;

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

        Change = gameObject.AddComponent<AudioSource>();
        Change.clip = ChangeClip;
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
        #endregion

        playerChoice = new int[4];

        playerJoined = new bool[4];
        playerJoined[0] = false;
        playerJoined[1] = false;
        playerJoined[2] = false;
        playerJoined[3] = false;
        Time.timeScale = 1;
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
    }

    // Update is called once per frame
    void Update()
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
        SceneManager.LoadScene("Loading");
        //SceneManager.LoadScene("LevelZero");
    }
    #region player Funktionen
    private void CheckPlayerJoins()
    {
        PlayerIndex[] freePads = GamePadManager.GetFreeControllers();
        for (int i = 0; i < freePads.Length; i++)
        {
            PlayerIndex index = freePads[i];
            GamePadState state = GamePad.GetState(index);
            GamePadState prevState = state;
            if (state.Buttons.Start == ButtonState.Pressed && !changeMenu)
            {
                GamePadManager.Connect((int)index);
                GlobalReferences.PlayerStates.Add(new PlayerState(index, state));

                //TODO: Change menu
                if (index == PlayerIndex.One)
                {
                    playerOneJoin.enabled = false;
                    charSelectPlayerOne.enabled = true;
                    playerJoined[0] = true;
                }

                if (index == PlayerIndex.Two)
                {
                    playerTwoJoin.enabled = false;
                    charSelectPlayerTwo.enabled = true;
                    playerJoined[1] = true;
                }

                if (index == PlayerIndex.Three)
                {
                    playerThreeJoin.enabled = false;
                    charSelectPlayerThree.enabled = true;
                    playerJoined[2] = true;
                }

                if (index == PlayerIndex.Four)
                {
                    playerFourJoin.enabled = false;
                    charSelectPlayerFour.enabled = true;
                    playerJoined[3] = true;
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

            if ((state.DPad.Left == ButtonState.Pressed && prevState[playerIndex].DPad.Left == ButtonState.Released) || (state.ThumbSticks.Left.X <= -0.1 && prevState[playerIndex].ThumbSticks.Left.X >= -0.1))
            {
                Change.Play();
                prevIndex[playerIndex] = index[playerIndex];
                index[playerIndex] -= 1;
                Debug.Log("index:" + index[playerIndex]);
                if (index[playerIndex] < 0)
                {
                    Debug.Log("Sprung auf Ende der List");
                    index[playerIndex] = 1;
                }

                ChangeImage(playerIndex, index[playerIndex], prevIndex[playerIndex]);
            }

            if ((state.DPad.Right == ButtonState.Pressed && prevState[playerIndex].DPad.Right == ButtonState.Released) || (state.ThumbSticks.Left.X >= 0.1 && prevState[playerIndex].ThumbSticks.Left.X <= 0.1))
            {
                Change.Play();
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

            if (state.Buttons.B == ButtonState.Pressed)
            {
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.One)
                {
                    playerOneJoin.enabled = true;
                    charSelectPlayerOne.enabled = false;
                    playerJoined[0] = false;
                }

                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Two)
                {
                    playerTwoJoin.enabled = true;
                    charSelectPlayerTwo.enabled = false;
                    playerJoined[1] = false;
                }
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Three)
                {
                    playerThreeJoin.enabled = true;
                    charSelectPlayerThree.enabled = false;
                    playerJoined[2] = false;
                }
                if (GlobalReferences.PlayerStates[i].Index == PlayerIndex.Four)
                {
                    playerFourJoin.enabled = true;
                    charSelectPlayerFour.enabled = false;
                    playerJoined[3] = false;
                }
                GamePadManager.Disconnect(GlobalReferences.PlayerStates[i].Index);
                GlobalReferences.PlayerStates.RemoveAt(i);
                i--;
                continue;
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

        playerJoined[0] = false;
        playerJoined[1] = false;
        playerJoined[2] = false;
        playerJoined[3] = false;
    }

    public void PressStartGame()
    {
        if (playerJoined[0] || playerJoined[1] || playerJoined[2] || playerJoined[3])
        {
            StartNewGame();
        }
    }
    #endregion

    #region -change Funktion
    public void ChangeImage(int playerIndex, int index, int prevIndex)
    {
        ImageList[playerIndex][index].enabled = true;
        ImageList[playerIndex][prevIndex].enabled = false;

        playerChoice[playerIndex] = index;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class CharacterSelectionScript : MonoBehaviour
{
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
    // Use this for initialization
    void Start ()
    {
        charSelectPlayerOne = charSelectPlayerOne.GetComponent<Canvas>();
        playerOneAegis = playerOneAegis.GetComponent<Image>();
        playerOneStalker = playerOneStalker.GetComponent<Image>();


        charSelectPlayerTwo = charSelectPlayerTwo.GetComponent<Canvas>();
        playerTwoAegis = playerTwoAegis.GetComponent<Image>();
        playerTwoStalker = playerTwoStalker.GetComponent<Image>();

        charSelectPlayerThree = charSelectPlayerThree.GetComponent<Canvas>();
        playerThreeAegis = playerThreeAegis.GetComponent<Image>();
        playerThreeStalker = playerThreeStalker.GetComponent<Image>();

        charSelectPlayerFour = charSelectPlayerFour.GetComponent<Canvas>();
        playerFourAegis = playerFourAegis.GetComponent<Image>();
        playerFourStalker = playerFourStalker.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
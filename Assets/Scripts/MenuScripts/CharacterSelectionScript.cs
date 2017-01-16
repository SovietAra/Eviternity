using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class CharacterSelectionScript : MonoBehaviour 
{
	/*public Canvas Ca_CharacterOne;
	public Canvas Ca_CharacterTwo;
	public Canvas Ca_CharacterThree;
	public Canvas Ca_CharacterFour;

	public Image I_CharOne;
	public Image I_CharTwo;*/

	// Use this for initialization

	void Start () 
	{
		/*Ca_CharacterOne = Ca_CharacterOne.GetComponent<Canvas>();
		Ca_CharacterTwo = Ca_CharacterTwo.GetComponent<Canvas>();
		Ca_CharacterThree = Ca_CharacterThree.GetComponent<Canvas>();
		Ca_CharacterFour = Ca_CharacterFour.GetComponent<Canvas>();

		I_CharOne = I_CharOne.GetComponent<Image>();
		I_CharTwo = I_CharTwo.GetComponent<Image>();

		Ca_CharacterOne.enabled = true;
		I_CharOne.enabled = true;
		Ca_CharacterTwo.enabled = false;
		I_CharTwo.enabled = false;
		Ca_CharacterThree.enabled = false;
		Ca_CharacterFour.enabled = false;*/

		List<Character> characters = new List<Character>();

        characters.Add( new Character(GetComponent<Image>())); 
	}
	void PressNext () 
	{
		PlayerIndex[] freePads = GamePadManager.GetFreeControllers();
        for (int i = 0; i < freePads.Length; i++)
        {
            PlayerIndex index = freePads[i];
            GamePadState state = GamePad.GetState(index);
            if (state.DPad.Right == ButtonState.Pressed)
            {
                GamePadManager.Connect((int)index);
                GlobalReferences.PlayerStates.Add(new PlayerState(index, state));

			if(index == PlayerIndex.One)
				{
					/*Ca_CharacterOne.enabled = false;
					Ca_CharacterTwo.enabled = true;*/
				}
			}
		}
	 }
}

public class Character
{
	public Image i_charOne;

		public Character(Image newi_charOne)
    {
		i_charOne = i_charOne.GetComponent<Image>();
		i_charOne = newi_charOne;
	}
}
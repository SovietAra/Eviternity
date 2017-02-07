using UnityEngine;
using XInputDotNetPure;

public class PlayerChoice : MonoBehaviour {

    private PlayerAssignmentScript playerAssignment;

    public int[] choices;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);

        choices = new int[4];
        playerAssignment = GameObject.FindObjectOfType<PlayerAssignmentScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (playerAssignment != null)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i] = playerAssignment.playerChoice[i];
                if (GamePad.GetState(PlayerIndex.Two).DPad.Up == ButtonState.Pressed)
                {
                    Debug.Log(choices[1]);
                }
                if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    Debug.Log(choices[0]);
                }
            }
        }
    }
}
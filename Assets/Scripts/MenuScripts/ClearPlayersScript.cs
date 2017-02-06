using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class ClearPlayersScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update() {

        
	}

    public void pressExit()
    {
        GamePadManager.Disconnect(PlayerIndex.One);
        GamePadManager.Disconnect(PlayerIndex.Two);
        GamePadManager.Disconnect(PlayerIndex.Three);
        GamePadManager.Disconnect(PlayerIndex.Four);

        GlobalReferences.PlayerStates.Clear();
    }
}

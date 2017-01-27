using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    private ColorSwitch_check colorSwitch;

    private void Start()
    {
        colorSwitch = GetComponent<ColorSwitch_check>();
    }
    private void OnTriggerEnter(Collider other)
    {
        colorSwitch.ChangeColor();
        Debug.Log("It might have worked");
        Player.LastCheckpointPosition = transform.position;
       // Debug.Log(GetComponent<Player>().checkpos);
    }
}

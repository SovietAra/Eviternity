using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloor : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //  List<GameObject> AvailablePlayer = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        Player player = other.GetComponent<Player>();
        if (player != null)
            player.OnIce = true;
        Debug.Log("Ice");
    }
    private void OnTriggerExit(Collider other)
    {

        //- player.PutOffIce();
        Player player = other.GetComponent<Player>();
        if (player != null)
            player.OnIce = false;
    }




}

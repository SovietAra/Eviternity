using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Animator animator;
    private bool DoorOpen;

	// Use this for initialization
	void Start ()
	{
	    DoorOpen = false;
	    animator = GetComponent<Animator>();
	    float width = transform.localScale.x;
	    float height = transform.localScale.y;
	    float length = transform.localScale.z;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnTriggerEnter(Collider col)
    {
        
        if (col.gameObject.CompareTag("Player"))
        {
            print("Open!");
            DoorOpen = true;
            Door("OpenDoor");
        }
    }

    

    void OnTriggerExit(Collider col)
    {
        if (DoorOpen == true)
        {
            DoorOpen = false;
            Door("CloseDoor");
        }
    }

    void Door(string direction)
    {
        animator.SetTrigger(direction);
    }
}

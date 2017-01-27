using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Animator animator;
    private bool DoorOpen;
    public List <GameObject> Enemies;
    private GameObject EnemyAlive;

	// Use this for initialization
	void Start ()
	{
	    DoorOpen = false;
	    animator = GetComponent<Animator>();
        EnemyAlive = GameObject.FindWithTag("Enemy");
    }
	
	// Update is called once per frame
	void Update ()
    {
        CountEnemies();

    }

    private void CountEnemies()
    {
        print(Enemies.Count);
        for (int i = Enemies.Count - 1; i > -1; i--)
        {
            if (Enemies[i] == null)
                Enemies.RemoveAt(i);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        
        if (col.gameObject.CompareTag("Player") && Enemies.Count == 0)
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

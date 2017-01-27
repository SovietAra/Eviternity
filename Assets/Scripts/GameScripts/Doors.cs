using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private AudioSource DoorSound;

    private Animator animator;
    private bool DoorOpen;
    public List<GameObject> Enemies;
    private GameObject EnemyAlive;

    // Use this for initialization
    private void Start()
    {
        //GameObject.FindGameObjectWithTag("DoorHolder").
        DoorSound = GetComponent<AudioSource>();
        DoorOpen = false;
        animator = GetComponent<Animator>();
        EnemyAlive = GameObject.FindWithTag("Enemy");
    }

    // Update is called once per frame
    private void Update()
    {
        CountEnemies();
    }

    private void CountEnemies()
    {
        print(Enemies.Count);
        for (var i = Enemies.Count - 1; i > -1; i--)
        {
            if (Enemies[i] == null)
                Enemies.RemoveAt(i);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && Enemies.Count == 0)
        {
            PlaySound();
            //      print("Open!");
            DoorOpen = true;

            Door("OpenDoor");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (DoorOpen == true)
        {
            PlaySound();
            DoorOpen = false;

            Door("CloseDoor");
        }
    }

    private void Door(string direction)
    {
        animator.SetTrigger(direction);
    }

    private void PlaySound()
    {
        if (DoorSound.isPlaying) DoorSound.Stop();
        DoorSound.Play();
    }
}
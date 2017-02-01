using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private AudioSource DoorSound;
    private Animator animator;
    private bool DoorOpen;
    private GameObject EnemyAlive;

    public List<GameObject> Enemies;

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

            DoorAnimations("OpenDoor");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (DoorOpen == true)
        {
            PlaySound();
            DoorOpen = false;

            DoorAnimations("CloseDoor");
        }
    }

    private void DoorAnimations(string direction)
    {
        animator.SetTrigger(direction);
    }

    private void PlaySound()
    {
        if (DoorSound.isPlaying) DoorSound.Stop();
        DoorSound.Play();
    }
}
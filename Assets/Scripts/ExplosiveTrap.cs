using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{

    [Range(0.1f, 10)]
    public float range;
    private float distanceToObject;
    private GameObject[] playerList;
    private GameObject[] enemyList;
    private bool triggered;
    [Range(1, 1000)]
    public int damage;

    // Use this for initialization
    void Start()
    {
        triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < playerList.Length; i++)
        {
            distanceToObject = Vector3.Distance(playerList[i].transform.position, transform.position);

            if (distanceToObject < range)
            {
                triggered = true;
            }
        }

        if (triggered)
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (distanceToObject < range)
                {

                }
            }

            Destroy(this);
        }
    }
}

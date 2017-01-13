using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{

    private float distanceToObject;
    private GameObject[] playerList;
    private GameObject[] enemyList;
    private bool triggered, collided;
    public bool triggerByCollision, triggerByRange;

    [Range(0.1f, 10)]
    public float range;

    [Range(1, 1000)]
    public int damage;

    // Use this for initialization
    void Start()
    {
        triggered = false;
        collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");

        if (triggerByRange)
        {
            for (int i = 0; i < playerList.Length; i++)
            {
                distanceToObject = Vector3.Distance(playerList[i].transform.position, transform.position);
                Player player = playerList[i].GetComponent<Player>();

                if (distanceToObject < range)
                {
                    player.DealDamage(damage);
                    triggered = true;
                }
            }

            if (triggered)
            {
                for (int i = 0; i < enemyList.Length; i++)
                {
                    distanceToObject = Vector3.Distance(enemyList[i].transform.position, transform.position);
                    Enemy enemy = enemyList[i].GetComponent<Enemy>();

                    if (distanceToObject < range)
                    {
                        // damage
                    }
                }

                Destroy(gameObject);
            }
        }

        if (triggerByCollision)
        {
            if (collided)
            {
                for (int i = 0; i < playerList.Length; i++)
                {
                    distanceToObject = Vector3.Distance(playerList[i].transform.position, transform.position);
                    Player player = playerList[i].GetComponent<Player>();
                    if (distanceToObject < range)
                    {
                        // damage
                        player.DealDamage(damage);
                    }
                }

                for (int i = 0; i < enemyList.Length; i++)
                {
                    distanceToObject = Vector3.Distance(enemyList[i].transform.position, transform.position);
                    Enemy enemy = enemyList[i].GetComponent<Enemy>();

                    if (distanceToObject < range)
                    {
                        // damage
                    }
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            collided = true;
        }
    }
}

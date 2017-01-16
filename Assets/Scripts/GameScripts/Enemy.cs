/* 
 * Purpose: Defining the Enemies
 * Author: Fabian Subat
 * Date: 10.01.2016 - TBA
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float rotationSpeed = 3.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float moveSpeed = 3.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float attackRange = 6.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float viewRange = 15.0f;

    
    [SerializeField]
    [Range(1f, 10000f)]
    private float maxHealth = 10;

    [SerializeField]
    [Range(0.1f, 10000f)]
    private float health = 10.0f;

    [SerializeField]
    [Range(1, 3)]
    private int enemyType = 1;

    [SerializeField]
    [Range(0.1f, 60f)]
    private float attackDelay = 1f;
    private float elapsedAttackDelay = 0f;

    private float enemyfront;
    private float distanceToPlayer;
    private float aimDirection;
    private GameObject targetPlayer;
    public GameObject Projectile;
    private float alivePlayers;

    private bool isValidTarget = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckAlivePlayers();

        //TODO consider viewrange
        if (isValidTarget) //On player found
        {
            distanceToPlayer = Vector3.Distance(targetPlayer.transform.position, transform.position);
            //Look at Player
            transform.rotation = Quaternion.Slerp(transform.rotation
                                                 , Quaternion.LookRotation(targetPlayer.transform.position - transform.position)
                                                 , rotationSpeed * Time.deltaTime);

            //Follow Player
            if (attackRange < distanceToPlayer && distanceToPlayer < viewRange)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
            else if(distanceToPlayer < viewRange)
            {
                targetPlayer = null;
                isValidTarget = false;
            }

            if (distanceToPlayer < attackRange)
            {
                TryShoot();
            }

            enemyfront = transform.eulerAngles.y;
        }
    }

    private void TryShoot()
    {
        //Todo: Ersetze code durch waffensystem und erstelle für alle gegnertypen Prefabs mit waffen, dann benötigst du keine unterscheidung mehr zwische klassen
        elapsedAttackDelay += Time.deltaTime;
        if (elapsedAttackDelay > attackDelay && enemyType == 1)
        {
            elapsedAttackDelay = 0f;
            GameObject gobj = Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Projectile projectile = gobj.GetComponent<Projectile>();
            projectile.AttackerTag = tag;
        }
        else if (elapsedAttackDelay > attackDelay && enemyType == 2)
        {
            elapsedAttackDelay = 0f;
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront + 27.5f, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront - 27.5f, 0f));
        }
        else if (elapsedAttackDelay > attackDelay && enemyType == 3)
        {
            elapsedAttackDelay = 0f;
            GameObject gobj = Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Projectile projectile = gobj.GetComponent<Projectile>();
            projectile.AttackerTag = transform.tag;
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront + 45, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront - 45, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront + 90, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront - 90, 0f));
        }
    }


    private void CheckAlivePlayers()
    {
        if (targetPlayer != null && distanceToPlayer <= viewRange) //TODO und in range - sinn erfragen
            return;

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players != null)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Player checkedPlayer = Players[i].GetComponent<Player>();
                if (!checkedPlayer.IsDead)
                {
                    isValidTarget = true;
                    targetPlayer = Players[i];
                    return; ;
                }
                else
                {
                    isValidTarget = false;
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectileScript = collision.gameObject.GetComponent<Projectile>();
            DealDamage(projectileScript.Damage);
        }
    }

    public void DealDamage(float damage)
    {
        health -= damage;
        //TODO Herrausfinden warum der Gegner sofort stirbt und Players.Length danach auf 0 gesetzt wird.
        //health -= damage * (damage / alivePlayers);
        //print(alivePlayers);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

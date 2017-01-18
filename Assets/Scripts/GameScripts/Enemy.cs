/* 
 * Purpose: Defining the Enemies
 * Author: Fabian Subat
 * Date: 10.01.2016 - TBA
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Range(0.1f, 10000f)]
    private float health = 10.0f;

    enum enemyTypes
    {
        Crawler = 1,
        Mosquito = 2,
        Mantis = 3
    }
    [SerializeField]
    enemyTypes enemyType;

    [SerializeField]
    [Range(0.1f, 60f)]
    private float attackDelay = 1f;
    private float elapsedAttackDelay = 0f;

    public Slider HealthSlider;
    private float enemyfront;
    private float distanceToPlayer;
    private GameObject targetPlayer;
    public GameObject Projectile;

    private bool isValidTarget = false;
    private DamageAbleObject dmgobjct;

    // Use this for initialization
    void Start()
    {
        dmgobjct = GetComponent<DamageAbleObject>();
        dmgobjct.OnDeath += Dmgobjct_OnDeath;
        SetUI();
    }

    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        CheckAlivePlayers();
        SetUI();

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

    private void SetUI()
    {
        HealthSlider.value = dmgobjct.Health;
    }

    //TODO Fix Variable Attacks with new attack instantiation
    private void TryShoot()
    {
        //Todo: Ersetze code durch waffensystem und erstelle für alle gegnertypen Prefabs mit waffen, dann benötigst du keine unterscheidung mehr zwische klassen
        elapsedAttackDelay += Time.deltaTime;
        if (elapsedAttackDelay > attackDelay && enemyType == enemyTypes.Crawler)
        {
            elapsedAttackDelay = 0f;
            GameObject gobj = Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Projectile projectile = gobj.GetComponent<Projectile>();
            projectile.AttackerTag = tag;
        }
        else if (elapsedAttackDelay > attackDelay && enemyType == enemyTypes.Mosquito)
        {
            elapsedAttackDelay = 0f;
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront + 27.5f, 0f));
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront - 27.5f, 0f));
            //print("I'm a Mosquito");
        }
        else if (elapsedAttackDelay > attackDelay && enemyType == enemyTypes.Mantis)
        {
            elapsedAttackDelay = 0f;
            GameObject gobj = Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
            Projectile projectile = gobj.GetComponent<Projectile>();
            projectile.AttackerTag = transform.tag;
            //print("I'm a Mantis");
        }
    }


    private void CheckAlivePlayers()
    {
        if (targetPlayer != null && distanceToPlayer <= viewRange)
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
}

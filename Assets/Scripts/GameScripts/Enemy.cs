/* 
 * Purpose: Defining the Enemies
 * Author: Fabian Subat
 * Date: 10.01.2016 - TBA
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    private bool isValidTarget = false;
    private DamageAbleObject dmgobjct;
    public GameObject PrimaryWeapon;
    private Weapon primaryWeapon;

    public UnityEvent onEnemyDeath;

    // Use this for initialization
    void Start()
    {
        dmgobjct = GetComponent<DamageAbleObject>();
        dmgobjct.OnDeath += Dmgobjct_OnDeath;
        primaryWeapon = Instantiate(PrimaryWeapon, transform).GetComponent<Weapon>();
        SetUI();
    }

    /// <summary>
    /// Called as the Entity gets destroyed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
        onEnemyDeath.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAlivePlayers();
        SetUI();

        if (isValidTarget) //On player found
        {
            //Calculate Distance between Enemyinstance and Player
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
            else if (distanceToPlayer < viewRange)
            {
                targetPlayer = null;
                isValidTarget = false;
            }
            if (distanceToPlayer < attackRange)
            {
                TryShoot();
                primaryWeapon.PrimaryAttack(transform.position, transform.forward, enemyfront);
            }

            enemyfront = transform.eulerAngles.y;
        }
    }

    private void SetUI()
    {
        HealthSlider.value = dmgobjct.Health;
    }

    /// <summary>
    /// Called as soon as the targetplayer is in attack range.
    /// </summary>
    private void TryShoot()
    {
        //Maybe TODO : Create Specific Enemy Weapon Prefab
        elapsedAttackDelay += Time.deltaTime;
        if (elapsedAttackDelay > attackDelay)
        {
            elapsedAttackDelay = 0f;
            primaryWeapon.PrimaryAttack(transform.position, transform.forward, enemyfront);
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

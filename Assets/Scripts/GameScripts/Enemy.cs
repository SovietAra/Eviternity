/* 
 * Purpose: Defining the Enemies
 * Author: Fabian Subat
 * Date: 10.01.2016 - TBA
 */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using Boo.Lang;

public class Enemy : MonoBehaviour
{
    #region PropertySliders
    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float rotationSpeed = 3.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float slowedSpeed = 3.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float defaultSpeed = 8.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float attackRange = 6.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float viewRange = 15.0f;

    [SerializeField]
    [Range(0, 30f)]
    private float deathDelay = 0f;

    private enum enemyTypes
    {
        Crawler = 1,
        Mosquito = 2,
        Mantis = 3
    }
    [SerializeField]
    enemyTypes enemyType;

    [SerializeField]
    [Range(0.1f, 100f)]
    private float switchDelay = 3f;

    [SerializeField]
    [Range(0, 30)]
    private int squadID;
    #endregion Propertysliders

    #region Privates
    private NavMeshAgent navAgent;

    private Animator anim;
    private bool attack;

    private GameObject currentTarget;
    private GameObject possibleTarget;
    private GameObject targetPlayer;
    private GameObject nearestTarget;

    private float enemyfront;
    private float distanceToPlayer;
    private float elapsedSwitchDelay = 0f;
    private float elapsedDeathTime = 0f;
    private bool delayDeath = false;

    public Slider HealthSlider;

    public bool freeze = false;
    private bool isValidTarget = false;
    private bool triggeredBySquad = false;
    private DamageAbleObject dmgobjct;
    public GameObject PrimaryWeapon;
    private Weapon primaryWeapon;
    private MoveScript moveScript;

    public UnityEvent OnEnemyDeath;
    private Vector3 movement;

    private List<Enemy> squadList = new List<Enemy>();
    #endregion Privates

    // Use this for initialization
    private void Start()
    {
        anim = transform.FindChild("Crawler_Animation").GetComponent<Animator>();

        dmgobjct = GetComponent<DamageAbleObject>();
        if (dmgobjct != null)
        {
            dmgobjct.OnDeath += Dmgobjct_OnDeath;
            dmgobjct.OnNewStatusEffect += Dmgobjct_OnNewStatusEffect;
        }

        if(PrimaryWeapon != null)
            primaryWeapon = Instantiate(PrimaryWeapon, transform).GetComponent<Weapon>();
        
        moveScript = GetComponent<MoveScript>();
        if(moveScript != null)
            moveScript.AddGravity = false;

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
            navAgent.stoppingDistance = attackRange;

        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyList.Length; i++)
        {
            Enemy enemy = enemyList[i].GetComponent<Enemy>();

            if (enemy != null)
            {
                if (enemy.squadID == squadID)
                {
                    squadList.Add(enemy);
                }
            }
        }

        SetUI();
    }

    private void Dmgobjct_OnNewStatusEffect(object sender, Assets.Scripts.StatusEffectArgs e)
    {
        e.StatusScript.OnActivate += StatusScript_OnActivate;
        e.StatusScript.OnDeactivate += StatusScript_OnDeactivate;
    }

    private void StatusScript_OnDeactivate(object sender, EventArgs e)
    {
        navAgent.speed = defaultSpeed;
    }

    private void StatusScript_OnActivate(object sender, EventArgs e)
    {
        StatusEffect script = sender as StatusEffect;
        if (script != null)
        {
            if (script.name.Contains("Slow"))
            {
                navAgent.speed = slowedSpeed;
            }
            else if (script.name.Contains("Stun"))
            {
                navAgent.speed = 0;

            }
        }
    }

    private void Update()
    {
        if (delayDeath)
        {
            elapsedDeathTime += Time.deltaTime;
            if(elapsedDeathTime > deathDelay)
            {
                Destroy(gameObject);
                delayDeath = false;
                OnEnemyDeath.Invoke();
            }
        }
        else
        {
            CheckAlivePlayers();
            SetUI();

            if (isValidTarget && !freeze) //On player found
            {
                //Calculate Distance between Enemyinstance and Player and switch target if delay allows
                nearestTarget = FindClosestPlayer();
                if (currentTarget == null)
                {
                    currentTarget = nearestTarget;
                }
                else if (currentTarget != nearestTarget && possibleTarget != nearestTarget)
                {
                    possibleTarget = nearestTarget;
                    elapsedSwitchDelay = 0f;
                }
                else if (nearestTarget == possibleTarget)
                {
                    elapsedSwitchDelay += Time.deltaTime;
                    if (elapsedSwitchDelay >= switchDelay)
                    {
                        currentTarget = possibleTarget;
                    }
                }

                //Calculate Distance to target
                if (currentTarget != null)
                    distanceToPlayer = Vector3.Distance(currentTarget.transform.position, transform.position);

                UpdateRotation();

                if (triggeredBySquad && distanceToPlayer < viewRange)
                {
                    triggeredBySquad = false;
                }

                //Follow Target
                if (attackRange < distanceToPlayer && distanceToPlayer < viewRange)
                {
                    bool walking = distanceToPlayer < viewRange;
                    anim.SetBool("IsWalking", walking);

                    if (currentTarget != null && navAgent != null)
                    {
                        for (int i = 0; i < squadList.Count; i++)
                        {
                            squadList[i].triggeredBySquad = true;
                        }

                        if (navAgent.isOnNavMesh)
                        {
                            navAgent.SetDestination(currentTarget.transform.position);
                        }
                    }
                }
                else if (distanceToPlayer > viewRange)
                {
                    targetPlayer = null;
                    isValidTarget = false;
                }

                if (triggeredBySquad)
                {
                    if (currentTarget != null && navAgent != null)
                    {
                        if (navAgent.isOnNavMesh)
                        {
                            navAgent.SetDestination(currentTarget.transform.position);
                        }
                    }
                }

                triggeredBySquad = false;
                bool attackTmp = distanceToPlayer < attackRange;
                anim.SetBool("IsMeleeAttack", attackTmp);


                if (distanceToPlayer <= navAgent.stoppingDistance)
                {
                    if (primaryWeapon != null)
                        primaryWeapon.PrimaryAttack(transform.position, transform.forward, enemyfront);
                }

                enemyfront = transform.eulerAngles.y;
            }
        }
    }

    private void UpdateRotation()
    {
        //Look at Player
        if (nearestTarget != null)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation
                                                 , Quaternion.LookRotation(nearestTarget.transform.position - transform.position)
                                                 , rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Called as the Entity gets destroyed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        if (deathDelay <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            delayDeath = true;
            elapsedDeathTime = 0f;
        }
        OnEnemyDeath.Invoke();

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
        if (primaryWeapon != null)
        {
            primaryWeapon.PrimaryAttack(transform.position, transform.forward, enemyfront);
        }
    }


    private GameObject FindClosestPlayer()
    {
        GameObject[] availablePlayers;
        availablePlayers = GameObject.FindGameObjectsWithTag("Player");
        float distance = Mathf.Infinity;

        foreach (GameObject checkedTargetPlayer in availablePlayers)
        {
            //Vector3 diff = checkedTargetPlayer.transform.position - transform.position;
            //float curDistance = diff.sqrMagnitude;

            float curDistance = Vector3.Distance(checkedTargetPlayer.transform.position, transform.position);
            if (curDistance < distance)
            {
                targetPlayer = checkedTargetPlayer;
                distance = curDistance;
            }
        }

        return targetPlayer;
    }

    private void CheckAlivePlayers()
    {
        if (targetPlayer != null)
            return;

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players != null)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Player checkedAlivePlayer = Players[i].GetComponent<Player>();
                if (!checkedAlivePlayer.IsDead)
                {
                    isValidTarget = true;
                    return;
                }
                else
                {
                    isValidTarget = false;
                }

            }
        }
    }
}

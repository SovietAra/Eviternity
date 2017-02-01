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

public class Enemy : MonoBehaviour
{
    #region PropertySliders
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
    #endregion Propertysliders

    #region Privates
    private NavMeshAgent navAgent;

    private GameObject currentTarget;
    private GameObject possibleTarget;
    private GameObject targetPlayer;
    private GameObject nearestTarget;

    private float enemyfront;
    private float distanceToPlayer;
    private float elapsedSwitchDelay = 0f;

    public Slider HealthSlider;

    public bool Freeze = false;
    private bool isValidTarget = false;
    private DamageAbleObject dmgobjct;
    public GameObject PrimaryWeapon;
    private Weapon primaryWeapon;
    private MoveScript moveScript;

    public UnityEvent OnEnemyDeath;
    private Vector3 movement;
    #endregion Privates

    // Use this for initialization
    private void Start()
    {
        dmgobjct = GetComponent<DamageAbleObject>();
        if(dmgobjct != null)
            dmgobjct.OnDeath += Dmgobjct_OnDeath;

        if(PrimaryWeapon != null)
            primaryWeapon = Instantiate(PrimaryWeapon, transform).GetComponent<Weapon>();
        
        moveScript = GetComponent<MoveScript>();
        if(moveScript != null)
            moveScript.AddGravity = false;

        navAgent = GetComponent<NavMeshAgent>();
        SetUI();
    }

    private void Update()
    {
        CheckAlivePlayers();
        SetUI();

        if (isValidTarget && !Freeze) //On player found
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
            else if(nearestTarget == possibleTarget)
            {
                elapsedSwitchDelay += Time.deltaTime;
                if(elapsedSwitchDelay >= switchDelay)
                {
                    currentTarget = possibleTarget;
                }
            }

            //Calculate Distance to target
            distanceToPlayer = Vector3.Distance(currentTarget.transform.position, transform.position);

            UpdateRotation();

            //Follow Target
            if (attackRange < distanceToPlayer && distanceToPlayer < viewRange)
            {
                navAgent.SetDestination(nearestTarget.transform.position);
            }
            else if (distanceToPlayer < viewRange)
            {
                targetPlayer = null;
                isValidTarget = false;
            }
            if (distanceToPlayer < attackRange)
            {
                if(primaryWeapon != null)
                    primaryWeapon.PrimaryAttack(transform.position, transform.forward, enemyfront);
            }

            enemyfront = transform.eulerAngles.y;
        }
    }

    private void UpdateRotation()
    {
        //Look at Player
        transform.rotation = Quaternion.Slerp(transform.rotation
                                             , Quaternion.LookRotation(nearestTarget.transform.position - transform.position)
                                             , rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Called as the Entity gets destroyed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
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
        Vector3 position = transform.position;
        foreach (GameObject checkedTargetPlayer in availablePlayers)
        {
            Vector3 diff = checkedTargetPlayer.transform.position - position;
            float curDistance = diff.sqrMagnitude;
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

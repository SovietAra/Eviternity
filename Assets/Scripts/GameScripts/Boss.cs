using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XInputDotNetPure;

public class Boss : MonoBehaviour
{
    public GameObject AOEWeapon;
    public GameObject IceWaveWeapon;
    public GameObject IcicleWeapon;
    public GameObject StormAbility;

    private Weapon aoeWeapon;
    private Weapon iceWaveWeapon;
    private Weapon icicleWeapon;
    private Ability stormAbility;

    private MoveScript moveScript;
    private DamageAbleObject healthContainer;
    private Player currentTarget = null;
    private float[] damageDone;
    private List<Player> playerScripts;
    private float animationDuration = 0f;
    private float angle;
    private Quaternion targetRotation;
    private bool updatePlayers = true;

    public bool Freeze = false;

    [SerializeField]
    [Range(0, 1000)]
    private float viewRange = 20;

    [SerializeField]
    [Range(0, 5)]
    private float aggroMultiplicator = 2f;

    [SerializeField]
    [Range(0, 20)]
    private float minimumDistance = 2f;

    private NavMeshAgent agent;

    private void Start()
    {
        damageDone = new float[4];
        if (AOEWeapon != null)
        {
            GameObject gobj = Instantiate(AOEWeapon, transform);
            if(gobj != null)
                aoeWeapon = gobj.GetComponent<Weapon>();
        }

        if (IceWaveWeapon != null)
        {
            GameObject gobj = Instantiate(IceWaveWeapon, transform);
            if(gobj != null)
                iceWaveWeapon = gobj.GetComponent<Weapon>();
        }

        if(IcicleWeapon != null)
        {
            GameObject gobj = Instantiate(IcicleWeapon, transform);
            if (gobj != null)
                icicleWeapon = gobj.GetComponent<Weapon>();
        }

        if (StormAbility != null)
        {
            GameObject gobj = Instantiate(StormAbility, transform);
            if(gobj != null)
                stormAbility = gobj.GetComponent<Ability>();
        }

        healthContainer = GetComponent<DamageAbleObject>();
        moveScript = GetComponent<MoveScript>();
        agent = GetComponent<NavMeshAgent>();
        playerScripts = new List<Player>();

        if(healthContainer != null)
        {
            healthContainer.OnReceiveDamage += HealthContainer_OnReceiveDamage;
        }

        GetPlayers();
        GamePadManager.OnPlayerCountChanged += GamePadManager_OnPlayerCountChanged;
    }

    private void GamePadManager_OnPlayerCountChanged(object sender, EventArgs e)
    {
        updatePlayers = true;
    }

    private void GetPlayers()
    {
        playerScripts.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Player playerScript = player.GetComponent<Player>();
            if (!playerScript.IsDead)
                playerScripts.Add(playerScript);
        }

        updatePlayers = playerScripts.Count == 0;
    }
    
    private void HealthContainer_OnReceiveDamage(object sender, Assets.Scripts.OnHealthChangedArgs e)
    {
        if(e.ResponsibleObject != null && e.ChangeValue > 0 && e.ResponsibleObject.CompareTag("Player"))
        {
            Player playerScript = e.ResponsibleObject.GetComponent<Player>();
            if(playerScript != null)
            {
                damageDone[(int)playerScript.Index] += e.ChangeValue;
            }
        }
    }

    private void Update()
    {
        if (updatePlayers)
            GetPlayers();

        animationDuration -= Time.deltaTime;
        if(!Freeze)
        {
            CheckCurrentTarget();

            if (currentTarget == null)
                SearchPlayer();

            if (currentTarget != null)
            {
                Hunt();
                DoRotation();
                AttackPlayer();
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 14);
            angle = transform.eulerAngles.y;
        }
    }

    private void DoRotation()
    {
        targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
    }

    private void Hunt()
    {
        agent.SetDestination(currentTarget.transform.position);
        agent.stoppingDistance = minimumDistance;
    }

    private void CheckCurrentTarget()
    {
        if (currentTarget == null)
            return;

        if (Vector3.Distance(currentTarget.transform.position, transform.position) > viewRange)
        {
            currentTarget = null;
        }
        else
        {
            CheckAggro();
        }
    }

    private void CheckAggro()
    {
        float damage = damageDone[(int)currentTarget.Index];
        for (int i = 0; i < damageDone.Length; i++)
        {
            if (damageDone[i] > damage * aggroMultiplicator)
            {
                damage = damageDone[i];
                currentTarget = GetPlayerByIndex((PlayerIndex)i);

                if(i + 1 < damageDone.Length)
                    CheckAggro();

                return;
            }
        }
    }

    private Player GetPlayerByIndex(PlayerIndex index)
    {
        for (int i = 0; i < playerScripts.Count; i++)
        {
            if (playerScripts[i].Index == index)
                return playerScripts[i];
        }
        return null;
    }

    private void SearchPlayer()
    {
        int index = -1;
        float damage = -1;
        for (int i = 0; i < playerScripts.Count; i++)
        {
            if(Vector3.Distance(playerScripts[i].transform.position, transform.position) < viewRange)
            {
                float tempDamage = damageDone[(int)playerScripts[i].Index];
                if(tempDamage > damage)
                {
                    index = i;
                    damage = tempDamage;
                }
            }
        }

        if(index >= 0)
            currentTarget = playerScripts[index];
    }

    private void AttackPlayer()
    {
        if (animationDuration < 0 && Mathf.Approximately(targetRotation.eulerAngles.y, transform.rotation.eulerAngles.y))
        {
            float distance = Vector3.Distance(currentTarget.transform.position, transform.position);
            WeaponDecider(currentTarget.transform.position, distance, angle);
        }
    }

    private Weapon WeaponDecider(Vector3 targetPosition, float distance, float angle)
    {
        bool done = false;
        if(aoeWeapon != null)
        {
            done = Shoot(aoeWeapon, transform.position, Vector3.zero, 0);
        }

        if (iceWaveWeapon != null && !done)
        {
            done = Shoot(iceWaveWeapon, transform.position, transform.forward, angle);
        }

        if (icicleWeapon != null && !done)
        {
            done = Shoot(iceWaveWeapon, targetPosition, Vector3.zero, 0);
        }

        if (stormAbility != null && !done)
        {
            done = stormAbility.Use();
        }

        return null;
    }

    private bool Shoot(Weapon weapon, Vector3 spawn, Vector3 forward, float angle)
    {
        if(weapon.PrimaryAttack(spawn, forward, angle))
        {
            animationDuration = weapon.AnimationDuration;
            return true;
        }
        return false;
    }
}


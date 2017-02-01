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
    private Vector3 spawnPosition;
    private NavMeshAgent agent;

    public bool Freeze = false;
    
    private float receivedDamagePerSecond = 0f;
    private float elapsedDamageTime = 0f;
    private float elapsedHitTime = 0f;
    private float damageReceived = 0f;

    [SerializeField]
    [Range(0, 1000)]
    private float viewRange = 20;

    [SerializeField]
    [Range(0, 5)]
    private float aggroMultiplicator = 2f;

    [SerializeField]
    [Range(0, 20)]
    private float minimumDistance = 2f;

    [SerializeField]
    [Range(0, 100)]
    private float resetTime = 10f;
    
    private void Start()
    {
        damageDone = new float[4];
        if (AOEWeapon != null)
        {
            GameObject gobj = Instantiate(AOEWeapon, transform);
            if (gobj != null)
            {
                aoeWeapon = gobj.GetComponent<Weapon>();
                aoeWeapon.OnPrimaryAttack += OnPrimaryAttack;
            }
        }

        if (IceWaveWeapon != null)
        {
            GameObject gobj = Instantiate(IceWaveWeapon, transform);
            if (gobj != null)
            {
                iceWaveWeapon = gobj.GetComponent<Weapon>();
                iceWaveWeapon.OnPrimaryAttack += OnPrimaryAttack;
            }
        }

        if (IcicleWeapon != null)
        {
            GameObject gobj = Instantiate(IcicleWeapon, transform);
            if (gobj != null)
            {
                icicleWeapon = gobj.GetComponent<Weapon>();
                icicleWeapon.OnPrimaryAttack += OnPrimaryAttack;
            }
        }

        if (StormAbility != null)
        {
            GameObject gobj = Instantiate(StormAbility, transform);
            if (gobj != null)
            {
                stormAbility = gobj.GetComponent<Ability>();
                stormAbility.OnActivated += OnAbilityActivated;
            }
        }

        healthContainer = GetComponent<DamageAbleObject>();
        if (healthContainer != null)
        {
            healthContainer.OnReceiveDamage += HealthContainer_OnReceiveDamage;
        }

        moveScript = GetComponent<MoveScript>();
        agent = GetComponent<NavMeshAgent>();
        playerScripts = new List<Player>();
        spawnPosition = transform.position;

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
        if (e.ResponsibleObject != null && e.ChangeValue > 0 && e.ResponsibleObject.CompareTag("Player"))
        {
            if (Mathf.Approximately(healthContainer.Health, healthContainer.MaxHealth))
                elapsedDamageTime = 0f;

            elapsedHitTime = 0f;
            damageReceived += e.ChangeValue;

            Player playerScript = e.ResponsibleObject.GetComponent<Player>();
            if (playerScript != null)
            {
                damageDone[(int)playerScript.Index] += e.ChangeValue;
            }
        }
    }

    private void Update()
    {
        if (updatePlayers)
            GetPlayers();

        UpdateTimers();

        if (!Freeze)
        {
            CheckCurrentTarget();

            if (currentTarget == null)
                SearchPlayer();

            if (currentTarget != null)
            {
                SetTargetPosition(currentTarget.transform.position, minimumDistance);
                DoRotation();
                AttackPlayer();
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 14);
            angle = transform.eulerAngles.y;
        }
    }

    private void UpdateTimers()
    {
        if (animationDuration >= 0)
            animationDuration -= Time.deltaTime;

        if (damageReceived > 0)
        {
            elapsedDamageTime += Time.deltaTime;
            receivedDamagePerSecond = damageReceived / elapsedDamageTime;
        }

        elapsedHitTime += Time.deltaTime;
        if (elapsedHitTime > resetTime)
        {
            ResetBoss();
        }
    }

    private void DoRotation()
    {
        targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
    }

    private void SetTargetPosition(Vector3 targetPosition, float stoppingDistance)
    {
        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(targetPosition);
                agent.stoppingDistance = stoppingDistance;
            }
            else
            {
                transform.position = spawnPosition;
                Debug.Log("Position reset required!");
            }
        }
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

                if (i + 1 < damageDone.Length)
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
            if (Vector3.Distance(playerScripts[i].transform.position, transform.position) < viewRange)
            {
                float tempDamage = damageDone[(int)playerScripts[i].Index];
                if (tempDamage > damage)
                {
                    index = i;
                    damage = tempDamage;
                }
            }
        }

        if (index >= 0)
            currentTarget = playerScripts[index];
    }

    private void AttackPlayer()
    {
        if (animationDuration < 0 && Mathf.Approximately(targetRotation.eulerAngles.y, transform.rotation.eulerAngles.y))
        {
            float distance = Vector3.Distance(currentTarget.transform.position, transform.position);
            WeaponDecider(currentTarget.transform.position, distance);
        }
    }

    private void WeaponDecider(Vector3 targetPosition, float distance)
    {
        bool done = false;
        if (aoeWeapon != null && false)
        {
            done = aoeWeapon.PrimaryAttack(transform.position, Vector3.zero, 0);
        }

        if (iceWaveWeapon != null && !done)
        {
            done = iceWaveWeapon.PrimaryAttack(transform.position, transform.forward, angle);
        }

        if (icicleWeapon != null && !done)
        {
            done = icicleWeapon.PrimaryAttack(targetPosition, Vector3.zero, 0);
        }

        if (stormAbility != null && !done)
        {
            stormAbility.Use();
        }
    }

    private void OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        animationDuration = ((Weapon)sender).AnimationDuration;
    }

    private void OnAbilityActivated(object sender, EventArgs e)
    {
        animationDuration = ((Ability)sender).AnimationDuration;
    }

    private void ResetBoss()
    {
        healthContainer.Heal(gameObject, healthContainer.MaxHealth);
        elapsedHitTime = 0f;
        elapsedDamageTime = 0f;
        damageReceived = 0f;
        animationDuration = 0f;
        SetTargetPosition(spawnPosition, 0);
    }
}

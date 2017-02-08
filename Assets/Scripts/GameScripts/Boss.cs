using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XInputDotNetPure;

public class Boss : MonoBehaviour
{
    private static System.Random rand = new System.Random();
    public GameObject AOEWeapon;
    public GameObject IceWaveWeapon;
    public GameObject IcicleAbility;
    public Animator animator;
    public bool Freeze = false;

    private AudioSource[] BossAudioSources;
    private Weapon aoeWeapon;
    private Weapon iceWaveWeapon;
    private Ability icicleAbility;
    private MoveScript moveScript;
    private DamageAbleObject healthContainer;
    private Player currentTarget = null;
    private float[] damageDone;
    private List<Player> playerScripts;
    private float animationDuration = 0f;
    private float angle;
    private Quaternion targetRotation;
    private bool updatePlayers = true;
    private bool isDead = false;
    private bool triggeredByProjectile = false;
    private Vector3 spawnPosition;
    private NavMeshAgent agent;
    private UIScript uiScript;
    private GameObject mainGameObject;
    private float receivedDamagePerSecond = 0f;
    private float elapsedDamageTime = 0f;
    private float elapsedHitTime = 0f;
    private float damageReceived = 0f;
    private float elapsedDeathTime = 0f;
    private float elapsedAggroTime = 0f;
    private float aggroTimeByProjectile = 3f;

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

    [SerializeField]
    [Range(0, 10)]
    private float deathDelay = 0;

    public AudioClip[] BossAudioClips = new AudioClip[10];

    private void Start()
    {
        //create as many audiosources as we want  = needed for playing as many sounds simultaniously as we want, same as in player
        //for example walk and hit sound, or walk and some type of attack
        for (var tmp = 0; tmp < BossAudioClips.Length; tmp++)
        {
            gameObject.AddComponent<AudioSource>();
        }

        BossAudioSources = GetComponents<AudioSource>();

        for (var tmp = 0; tmp < BossAudioClips.Length; tmp++)
        {
            BossAudioSources[tmp].clip = BossAudioClips[tmp];
        }
        //

        mainGameObject = GameObject.FindGameObjectWithTag("GameObject");
        uiScript = mainGameObject.GetComponent<UIScript>();

        damageDone = new float[4];
        if (AOEWeapon != null)
        {
            GameObject gobj = Instantiate(AOEWeapon, transform);
            if (gobj != null)
            {
                aoeWeapon = gobj.GetComponent<Weapon>();
                aoeWeapon.OnPrimaryAttack += OnPrimaryAttack;
                aoeWeapon.OnDelayedPrimaryAttack += OnPrimaryAttack;
            }
        }

        if (IceWaveWeapon != null)
        {
            GameObject gobj = Instantiate(IceWaveWeapon, transform);
            if (gobj != null)
            {
                iceWaveWeapon = gobj.GetComponent<Weapon>();
                iceWaveWeapon.OnPrimaryAttack += OnPrimaryAttack;
                iceWaveWeapon.OnDelayedPrimaryAttack += OnPrimaryAttack;
            }
        }

        if (IcicleAbility != null)
        {
            GameObject gobj = Instantiate(IcicleAbility, transform);
            if (gobj != null)
            {
                icicleAbility = gobj.GetComponent<Ability>();
                icicleAbility.OnActivated += IcicleAbility_OnActivated;
            }
        }

        healthContainer = GetComponent<DamageAbleObject>();
        if (healthContainer != null)
        {
            healthContainer.OnDeath += HealthContainer_OnDeath;
            healthContainer.OnReceiveDamage += HealthContainer_OnReceiveDamage;
        }

        moveScript = GetComponent<MoveScript>();
        agent = GetComponent<NavMeshAgent>();
        playerScripts = new List<Player>();
        spawnPosition = transform.position;

        GetPlayers();
        GamePadManager.OnPlayerCountChanged += GamePadManager_OnPlayerCountChanged;
    }
    
    private void HealthContainer_OnDeath(object sender, EventArgs e)
    {
        //TODO: Umbauen, dass Boss erst nach Delay/Animationsende stirbt
        //Destroy(gameObject);
        if (!isDead)
            animator.SetTrigger("OnDeath");
        isDead = true;
        uiScript.HideBossHealth(true);
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
            if (player != null)
            {
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    if (!playerScript.IsDead)
                        playerScripts.Add(playerScript);
                }
            }
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
                if(currentTarget == null)
                {
                    currentTarget = playerScript;
                    triggeredByProjectile = true;
                    elapsedAggroTime = 0f;
                }
            }
        }
    }

    private void Update()
    {
        if (!isDead)
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
                    if (animationDuration <= 0)
                    {
                        SetTargetPosition(currentTarget.transform.position, minimumDistance);
                        if (agent.velocity != Vector3.zero)
                        {
                            animator.SetBool("Walking", true);
                            if (BossAudioSources[5] != null && !BossAudioSources[5].isPlaying) BossAudioSources[5].Play();//play walk sound, walk is index 5
                        }
                        else
                            animator.SetBool("Walking", false);
                        DoRotation();
                        AttackPlayer();
                    }
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 14);
                angle = transform.eulerAngles.y;
            }
            else
            {
                if (agent != null)
                    agent.Stop();
            }
        }
        else
        {
            UpdateDeath();
        }
    }

    private void UpdateDeath()
    {
        elapsedDeathTime += Time.deltaTime;
        if (elapsedDeathTime >= deathDelay)
        {
            Destroy(gameObject);
            GameInspector.Win = true;
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

        if (currentTarget != null)
            elapsedHitTime = 0f;

        if(triggeredByProjectile)
        {
            elapsedAggroTime += Time.deltaTime;
            if(elapsedAggroTime > aggroTimeByProjectile)
            {
                elapsedAggroTime = 0f;
                triggeredByProjectile = false;
            }
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
                agent.Resume();
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
        {
            uiScript.HideBossHealth(false);
            return;
        }

        if (Vector3.Distance(currentTarget.transform.position, transform.position) > viewRange && !triggeredByProjectile)
        {
            currentTarget = null;
        }
        else
        {
            uiScript.ShowBossHealth();
            CheckAggro();
        }
    }

    private void CheckAggro()
    {
        if (currentTarget != null)
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
            if (playerScripts[i] == null)
            {
                playerScripts.RemoveAt(i);
                i--;
                continue;
            }

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
        if (animationDuration <= 0)
        {
            float distance = Vector3.Distance(currentTarget.transform.position, transform.position);
            WeaponDecider(currentTarget.transform.position, distance);
        }
    }

    private void WeaponDecider(Vector3 targetPosition, float distance)
    {
        bool done = false;
        if (distance < 7f && aoeWeapon.IsReady() && iceWaveWeapon.IsReady())
        {
            if (rand.Next(0, 101) < 50)
            {
                done = aoeWeapon.PrimaryAttack(transform.position + (transform.forward * 2), transform.forward, 0);
                if (done)
                    animator.SetTrigger("MultiHit");
                if (BossAudioSources[7] != null && !BossAudioSources[7].isPlaying) BossAudioSources[7].Play(); //hammer hit
            }
            else
            {
                done = iceWaveWeapon.PrimaryAttack(transform.position + (transform.forward * 2), transform.forward, angle);
                if (done)
                    animator.SetTrigger("IceWave");
                if (BossAudioSources[5] != null && !BossAudioSources[3].isPlaying) BossAudioSources[3].Play(); //ice wave sound, index 3
            }
        }
        else
        {
            if (aoeWeapon != null && distance < 7f && !done)
            {
                done = aoeWeapon.PrimaryAttack(transform.position + (transform.forward * 2), transform.forward, 0);
                if (done)
                    animator.SetTrigger("MultiHit");
                if (BossAudioSources[7] != null && !BossAudioSources[7].isPlaying) BossAudioSources[7].Play(); //hammer hit
            }
            else if (iceWaveWeapon != null && !done && distance < 7f
                && MathUtil.Between(targetRotation.eulerAngles.y, transform.rotation.eulerAngles.y - 5f, transform.rotation.eulerAngles.y + 5f))
            {
                done = iceWaveWeapon.PrimaryAttack(transform.position + (transform.forward * 2), transform.forward, angle);
                if (done)
                    animator.SetTrigger("IceWave");
                if (BossAudioSources[5] != null && !BossAudioSources[3].isPlaying) BossAudioSources[3].Play(); //ice wave sound, index 3
            }
            else if (icicleAbility != null && !done && distance >= 7f && distance <= viewRange)
            {
                Vector3 translation = targetPosition - transform.position;
                icicleAbility.SpawnTranslation = translation;
                if (icicleAbility.Use())
                {
                    animator.SetTrigger("SingleHit");
                    done = icicleAbility.Use();
                    if (BossAudioSources[0] != null && !BossAudioSources[0].isPlaying) BossAudioSources[0].Play();
                }
            }
        }
    }

    private void OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        agent.Stop();
        animationDuration = e.AnimationDuration;
    }

    private void IcicleAbility_OnActivated(object sender, EventArgs e)
    {
        agent.Stop();
        animationDuration = icicleAbility.AnimationDuration;
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
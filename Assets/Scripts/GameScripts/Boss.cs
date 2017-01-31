using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XInputDotNetPure;

public class Boss : MonoBehaviour
{
    private Weapon aoeWeapon;
    private Weapon iceWaveWeapon;
    private Weapon blub;
    private Ability ability;
    private MoveScript moveScript;
    private DamageAbleObject healthContainer;
    private Player currentTarget = null;
    private float[] damageDone;
    private List<Player> playerScripts;

    public bool Freeze = false;

    [SerializeField]
    [Range(0, 1000)]
    private float viewRange = 20;

    [SerializeField]
    [Range(0, 5)]
    private float aggroMultiplicator = 2f;

    private void Start()
    {
        damageDone = new float[4];
        aoeWeapon = GetComponent<Weapon>();
        iceWaveWeapon = GetComponent<Weapon>();
        ability = GetComponent<Ability>();
        healthContainer = GetComponent<DamageAbleObject>();
        moveScript = GetComponent<MoveScript>();
        playerScripts = new List<Player>();

        if(healthContainer != null)
        {
            healthContainer.OnReceiveDamage += HealthContainer_OnReceiveDamage;
        }

        GamePadManager.OnPlayerCountChanged += GamePadManager_OnPlayerCountChanged;
    }

    private void GamePadManager_OnPlayerCountChanged(object sender, EventArgs e)
    {
        playerScripts.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Player playerScript = player.GetComponent<Player>();
            if(playerScript.IsDead)
            playerScripts.Add(playerScript);
        }
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
        if(!Freeze)
        {
            CheckCurrentTarget();
            if (currentTarget == null)
                SearchPlayer();

            if (currentTarget != null)
                AttackPlayer();
            else
                TryHeal();
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

                if(i +1 < damageDone.Length)
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

       // if(index >= 0 && )
       //     currentTarget = playerScripts[i];
    }

    private void AttackPlayer()
    {
        
    }

    private void TryHeal()
    {

    }
}


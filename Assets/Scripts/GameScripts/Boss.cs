using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Weapon aoeWeapon;
    private Weapon iceWaveWeapon;
    private Weapon blub;
    private Ability ability;
    private MoveScript moveScript;
    private DamageAbleObject healthContainer;
    private GameObject currentTarget = null;
    private float[] damageDone;
    private List<Player> playerScripts;

    public bool Freeze = false;



    private void Start()
    {
        damageDone = new float[4];
        aoeWeapon = GetComponent<Weapon>();
        iceWaveWeapon = GetComponent<Weapon>();
        ability = GetComponent<Ability>();
        healthContainer = GetComponent<DamageAbleObject>();
        moveScript = GetComponent<MoveScript>();

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

    }

    private void SearchPlayer()
    {

    }

    private void AttackPlayer()
    {
        
    }

    private void TryHeal()
    {

    }
}


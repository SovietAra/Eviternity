using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAbleObject : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 100000f)]
    private float health = 10;

    [SerializeField]
    [Range(0.1f, 100000f)]
    private float maxHealth = 10;

    public event EventHandler OnDeath;

    public float Health
    {
        get { return health; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
    }

	// Use this for initialization
	private void Start ()
    {
        if(health > maxHealth)
            health = maxHealth;
	}
	
	// Update is called once per frame
	private void Update ()
    {
		
	}

    public virtual void DoDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            if (OnDeath != null)
                OnDeath(this, EventArgs.Empty);
        }
    }

    public virtual void Heal(float addHealth)
    {
        health += addHealth;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
}

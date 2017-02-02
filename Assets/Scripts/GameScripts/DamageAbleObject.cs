using System;
using UnityEngine;
using Assets.Scripts;

public class DamageAbleObject : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 100000f)]
    private float health = 10;

    [SerializeField]
    [Range(0.1f, 100000f)]
    private float maxHealth = 10;

    public bool isImmortal = false;

    public event EventHandler OnDeath;
    public event EventHandler<OnHealthChangedArgs> OnReceiveDamage;
    public event EventHandler<OnHealthChangedArgs> OnReceiveHealth;
    public event EventHandler<StatusEffectArgs> OnNewStatusEffect;

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

    public virtual void DoDamage(GameObject attacker, float damage, GameObject statusEffect)
    {
        if (!isImmortal)
        {
            OnHealthChangedArgs args = new OnHealthChangedArgs(attacker, damage);
            if (OnReceiveDamage != null)
                OnReceiveDamage(this, args);

            if (!args.Cancel && args.ChangeValue != 0)
            {
                if (statusEffect != null && gameObject != null)
                {
                    GameObject tempStatusEffect = Instantiate(statusEffect, transform);
                    if (tempStatusEffect != null)
                    {
                        StatusEffect statusScript = tempStatusEffect.GetComponent<StatusEffect>();
                        if (statusScript != null)
                        {
                            StatusEffectArgs statusArgs = new StatusEffectArgs(statusScript, tempStatusEffect);
                            if (OnNewStatusEffect != null)
                                OnNewStatusEffect(this, statusArgs);

                            if(!statusArgs.Cancel)
                                statusScript.Activate(gameObject);
                        }
                    }
                }

                health -= args.ChangeValue;
                if (health <= 0)
                {
                    health = 0;
                    if (OnDeath != null)
                        OnDeath(this, EventArgs.Empty);
                }
            }
        }
    }

    public virtual void DoDamage(GameObject attacker, float damage)
    {
        DoDamage(attacker, damage, null);
    }

    public virtual void Heal(GameObject healer, float addHealth, GameObject statusEffect)
    {
        OnHealthChangedArgs args = new OnHealthChangedArgs(healer, addHealth);
        if (OnReceiveHealth != null)
            OnReceiveHealth(this, args);

        if (!args.Cancel)
        {
            health += args.ChangeValue;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    public virtual void Heal(GameObject healer, float addHealth)
    {
        Heal(healer, addHealth, null);
    }
}

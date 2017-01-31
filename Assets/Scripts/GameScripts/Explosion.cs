using Assets.Scripts;
using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{  
    [SerializeField]
    [Range(0, 1000)]
    private float radius = 5;
       
    [SerializeField]
    [Range(0, 10)]
    private float damageReduction = 1;

    [SerializeField]
    [Range(0, 2)]
    private float teamDamageMultiplicator = 1;

    [SerializeField]
    [Range(-1000, 1000)]
    private float damage = 1;

    public GameObject StatusEffect;

    private GameObject owner;
    private SphereCollider sphereExplosion;
    private bool exploded;
    private bool playOnce;

    private ParticleSystem effect;
    private float removeDelay = 0.1f;
    private float elapsedTime = 0f;
    private bool overrideValues = true;

    public event EventHandler<HitEventArgs> OnHit;

    public void Init(float damage, float radius, GameObject owner, float damageReduction, float teamDamageMultiplicator)
    {
        this.damage = damage;
        this.damageReduction = damageReduction;
        this.owner = owner;
        this.teamDamageMultiplicator = teamDamageMultiplicator;
        this.radius = radius;
        if (sphereExplosion == null)
            sphereExplosion = GetComponent<SphereCollider>();

        sphereExplosion.radius = radius;
        overrideValues = false;
    }

    public void Init(float damage, float radius, GameObject owner, float teamDamageMultiplicator)
    {
        Init(damage, radius, owner, damageReduction, teamDamageMultiplicator);
    }

    public void Init(float damage, float radius, GameObject owner, bool doTeamDamage)
    {
        Init(damage, radius, owner, damageReduction, doTeamDamage ? 1 : 0);
    }

    private void Start()
    {
        if (overrideValues)
        {
            if (gameObject.tag != null)
            {
                owner = gameObject;
            }
        }

        exploded = false;
        playOnce = false;
        if(sphereExplosion == null)
        {
            sphereExplosion = GetComponent<SphereCollider>();
            sphereExplosion.radius = radius;
        }
        effect = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if(exploded)
        {
            Destroy(sphereExplosion);
            if (!playOnce)
            {
                effect.Play();
                playOnce = true;
            }
            else
            {
                if(!effect.isPlaying)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > removeDelay)
                exploded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isTeam = false;
        if (owner != null)
            isTeam = other.transform.CompareTag(owner.tag);

        DamageAbleObject healthContainer = other.gameObject.GetComponent<DamageAbleObject>();
        if (healthContainer != null)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            float rangeDamage = (damageReduction - (distance / radius)) * damage;
            if (isTeam)
                rangeDamage *= teamDamageMultiplicator;

            HitEventArgs hitArgs = new HitEventArgs(rangeDamage, owner, other.gameObject, isTeam, true);
            if (OnHit != null)
                OnHit(this, hitArgs);

            if (!hitArgs.Cancel && hitArgs.FinalDamage > 0)
            {
                if (StatusEffect != null)
                {
                    GameObject tempStatusEffect = Instantiate(StatusEffect, other.transform);
                    if (tempStatusEffect != null)
                    {
                        StatusEffect statusScript = tempStatusEffect.GetComponent<StatusEffect>();
                        if(statusScript != null)
                        {
                            statusScript.Activate(other.gameObject);
                        }
                    }
                }
                healthContainer.DoDamage(owner, rangeDamage);
            }

            exploded = true;
        }

    }
}


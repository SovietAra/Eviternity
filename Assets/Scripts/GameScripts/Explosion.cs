using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    private string ownerTag;
    private SphereCollider sphereExplosion;
    private bool exploded;
    private bool playOnce;

    private ParticleSystem effect;
    private float removeDelay = 0.1f;
    private float elapsedTime = 0f;
    private bool overrideValues = true;

    public event EventHandler<HitEventArgs> OnHit;

    public void Init(float damage, float radius, string ownerTag, float damageReduction, float teamDamageMultiplicator)
    {
        this.damage = damage;
        this.damageReduction = damageReduction;
        this.ownerTag = ownerTag;
        this.teamDamageMultiplicator = teamDamageMultiplicator;
        this.radius = radius;
        if (sphereExplosion == null)
            sphereExplosion = GetComponent<SphereCollider>();

        sphereExplosion.radius = radius;
        overrideValues = false;
    }

    public void Init(float damage, float radius, string ownerTag, float teamDamageMultiplicator)
    {
        Init(damage, radius, ownerTag, damageReduction, teamDamageMultiplicator);
    }

    public void Init(float damage, float radius, string ownerTag, bool doTeamDamage)
    {
        Init(damage, radius, ownerTag, damageReduction, doTeamDamage ? 1 : 0);
    }

    private void Start()
    {
        if (overrideValues)
        {
            if (gameObject.tag != null)
            {
                ownerTag = gameObject.tag;
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
        if (ownerTag != null)
            isTeam = other.transform.CompareTag(ownerTag);

        DamageAbleObject healthContainer = other.gameObject.GetComponent<DamageAbleObject>();
        if (healthContainer != null)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            float rangeDamage = (damageReduction - (distance / radius)) * damage;
            if (isTeam)
                rangeDamage *= teamDamageMultiplicator;

            HitEventArgs hitArgs = new HitEventArgs(rangeDamage, ownerTag, other.gameObject, isTeam, true);
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
                healthContainer.DoDamage(rangeDamage);
            }

            exploded = true;
        }

    }
}


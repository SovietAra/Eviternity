using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Explosion : MonoBehaviour
{  
    private float radius;   
    private float damageReduction;
    private float teamDamageMultiplicator;
    private float damage;
    private string ownerTag;
    private SphereCollider sphereExplosion;
    private bool exploded;
    private bool playOnce;

    private ParticleSystem effect;

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

            if (rangeDamage > 0)
                healthContainer.DoDamage(rangeDamage);

            exploded = true; ;
        }

    }
}


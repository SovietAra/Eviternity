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

    public bool UseDamageReduction = true;

    [SerializeField]
    [Range(0, 2)]
    private float teamDamageMultiplicator = 1;

    [SerializeField]
    [Range(-1000, 1000)]
    private float damage = 1;

    public GameObject StatusEffect;
    public AudioClip ExplosionSound;

    private AudioSource audioSource;
    private GameObject owner;
    private SphereCollider sphereExplosion;
    private bool exploded;
    private bool playOnce;

    private ParticleSystem effect;
    private float removeDelay = 0.1f;
    private float elapsedTime = 0f;
    private bool overrideValues = true;

    public float Radius
    {
        get { return radius; }
    }

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

        if(sphereExplosion != null)
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

        audioSource = GetComponent<AudioSource>();
        exploded = false;
        playOnce = false;
        if(sphereExplosion == null)
        {
            sphereExplosion = GetComponent<SphereCollider>();
            if(sphereExplosion != null)
                sphereExplosion.radius = radius;
        }
        effect = GetComponent<ParticleSystem>();
        if (effect == null)
            effect = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if(exploded)
        {
            if(sphereExplosion != null)
                Destroy(sphereExplosion);

            if (!playOnce)
            {
                if(effect != null)
                    effect.Play();
                playOnce = true;

                if (audioSource != null && ExplosionSound != null)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = ExplosionSound;
                        audioSource.Play();
                    }
                }
            }
            else
            {
                if (effect != null && !effect.isPlaying)
                {
                    Destroy(gameObject);
                }
                else if (effect == null)
                    Destroy(gameObject);
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

            float rangeDamage = damage;
            if (UseDamageReduction)
            {
                rangeDamage = (damageReduction - (distance / radius)) * damage;
                if (rangeDamage <= 0)
                    return;
            }

            if (isTeam)
                rangeDamage *= teamDamageMultiplicator;

            HitEventArgs hitArgs = new HitEventArgs(rangeDamage, owner, other.gameObject, isTeam, true);
            if (OnHit != null)
                OnHit(this, hitArgs);

            if (!hitArgs.Cancel && hitArgs.FinalDamage > 0)
            {
                healthContainer.DoDamage(owner, rangeDamage, StatusEffect);
            }

            exploded = true;
        }
    }
}


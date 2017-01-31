using Assets.Scripts;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 100f)]
    private float speed = 5f;

    [SerializeField]
    [Range(0.1f, 10000f)]
    private float damage = 1f;

    [SerializeField]
    [Range(0f, 1000f)]
    private float damageRange = 0f;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float detonationTimer = 2f;

    [SerializeField]
    [Range(0.01f, 5f)]
    private float invertGravityFactor = 0.5f;

    [SerializeField]
    [Range(0, 60)]
    private float lifeTime = 10f;

    public bool DestroyOnCollision = true;
    public bool CollideWithOtherProjectiles = false;

    public bool DoTeamDamage = false;
    public bool DoAOETeamDamage = false;
    public GameObject Explosion;
    public GameObject StatusEffect;

    [SerializeField]
    [Range(0.0f, 10f)]
    public float TeamDamageMultiplicator = 0.75f;
    public bool InvertGravity;
    public event EventHandler<HitEventArgs> OnHit;

    public float Damage
    {
        get { return damage; }
    }

    private Rigidbody attachedBody;
    private float elapsedTime = 0f;
    private float elapsedLifeTime;
    private bool startTimer = false;
    private GameObject attacker;

    public GameObject Attacker
    {
        get { return attacker; }
        set
        {
            attacker = value;
            if (!DoTeamDamage && !attacker.CompareTag("Untagged"))
            {
                if (attacker.CompareTag("Player"))
                {
                    gameObject.layer = 10;
                    Physics.IgnoreLayerCollision(gameObject.layer, 8);
                }
                else if (attacker.CompareTag("Enemy"))
                {
                    gameObject.layer = 11;
                    Physics.IgnoreLayerCollision(gameObject.layer, 9);
                }
            }
        }
    }

    private void Start()
    {  
        attachedBody = GetComponent<Rigidbody>();
        if(attachedBody != null)
            attachedBody.velocity = (transform.forward * speed) + (InvertGravity ? new Vector3(0, invertGravityFactor, 0) : Vector3.zero);

        if (CollideWithOtherProjectiles)
        {
            Physics.IgnoreLayerCollision(10, 10);
        }
    }

    private void Update()
    {
        elapsedLifeTime += Time.deltaTime;
        if (elapsedLifeTime > lifeTime)
        {
            Destroy(gameObject);
        }

        if (startTimer)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= detonationTimer)
            {
                Detonate(null);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageAbleObject damageObject = other.gameObject.GetComponent<DamageAbleObject>();
        if (damageObject != null)
        {
            Detonate(damageObject);
        }
        else
        {
            if (!startTimer)
            {
                startTimer = true;
            }
        }
    }

    private void Detonate(DamageAbleObject collisionObject)
    {
        if (DestroyOnCollision)
            Destroy(gameObject);

        if (damageRange == 0)
        {
            if (collisionObject != null)
            {
                bool isTeam = attacker != null && collisionObject.transform.CompareTag(Attacker.tag);
                DoDamage(collisionObject.gameObject, collisionObject, damage, isTeam);
            }
        }
        else
        {
            GameObject explosionObj = Instantiate(Explosion, transform.position, transform.rotation);
            Explosion explosionScript = explosionObj.GetComponent<Explosion>();

            explosionScript.Init(damage, damageRange, attacker, 1.2f, DoAOETeamDamage ? TeamDamageMultiplicator : 0);
        }
    }

    private void DoDamage(GameObject victim, DamageAbleObject damageAbleObject, float damage, bool teamDamage)
    {
        HitEventArgs hitArgs = new HitEventArgs(damage * (DoTeamDamage ? TeamDamageMultiplicator : 1), attacker, victim, teamDamage, false);
        if (OnHit != null)
            OnHit(this, hitArgs);

        if(!hitArgs.Cancel && hitArgs.FinalDamage > 0)
        {
            damageAbleObject.DoDamage(attacker, hitArgs.FinalDamage, StatusEffect);
        }
    }
}


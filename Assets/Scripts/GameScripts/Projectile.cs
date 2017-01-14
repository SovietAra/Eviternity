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

    public bool DestroyOnCollision = true;
    public bool CollideWithOtherProjectiles = false;

    public bool DoTeamDamage = false;
    public bool DoAOETeamDamage = false;

    [SerializeField]
    [Range(0.0f, 10f)]
    public float TeamDamageMultiplicator = 0.75f;

    public bool InvertGravity;
    
    private Collider projectileCollider;
    private Rigidbody attachedBody;
    private float elapsedTime = 0f;
    
    private bool startTimer = false;
    private string attackerTag;

    public float Damage
    {
        get { return damage; }
    }

    public string AttackerTag
    {
        get
        {
            return attackerTag;
        }

        set
        {
            attackerTag = value;
        }
    }

    private void Start()
    {
        projectileCollider = GetComponent<Collider>();      
        attachedBody = GetComponent<Rigidbody>();
        attachedBody.velocity = (transform.forward * speed) + (InvertGravity ? new Vector3(0, invertGravityFactor, 0) : Vector3.zero);
    }

    private void Update()
    {
        if (startTimer)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= detonationTimer)
            {
                Detonate(null);
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if ((!CollideWithOtherProjectiles && collision.gameObject.CompareTag("Projectile")) ||
            (AttackerTag != null && !DoTeamDamage && collision.gameObject.CompareTag(AttackerTag)))
        {
            Physics.IgnoreCollision(collision.collider, projectileCollider);
        }
        else
        {
            DamageAbleObject damageObject = collision.gameObject.GetComponent<DamageAbleObject>();
            if(damageObject != null)
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
    }

    private void Detonate(DamageAbleObject collisionObject)
    {
        if (DestroyOnCollision)
            Destroy(gameObject);

        if (damageRange == 0)
        {
            if (collisionObject != null)
            {
                bool isTeam = AttackerTag != null && collisionObject.transform.CompareTag(AttackerTag);
                DoDamage(collisionObject, damage, isTeam);
            }
        }
        else
        {
            DamageAbleObject[] damageAbleObjects = GameObject.FindObjectsOfType<DamageAbleObject>();
            foreach (DamageAbleObject item in damageAbleObjects)
            {
                float distance = Vector3.Distance(item.transform.position, transform.position);                
                if (distance <= damageRange)
                {
                    bool isTeam = AttackerTag != null && item.transform.CompareTag(AttackerTag);
                    //TODO: Damage reduction on per distance
                    if (!isTeam || DoAOETeamDamage)
                    {
                        DoDamage(item, damage, isTeam);
                    }
                }
            }
        }
    }

    private void DoDamage(DamageAbleObject damageAbleObject, float damage, bool teamDamage)
    {
        if (teamDamage)
        {
            damageAbleObject.DoDamage(damage * TeamDamageMultiplicator);
        }
        else
        {
            damageAbleObject.DoDamage(damage);
        }
    }
}


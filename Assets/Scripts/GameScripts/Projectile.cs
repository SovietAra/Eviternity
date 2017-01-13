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
    public bool CollideWithPlayers = false;
    public bool CollideWithEnemy = true;
    public bool CollideWithOtherProjectiles = false;

    public bool DoAOEDamageOnPlayer = false;
    public bool DoAOEDamageOnEnemy = false;

    public bool InvertGravity;
    
    private Collider projectileCollider;
    private Rigidbody attachedBody;
    private float elapsedTime = 0f;
    private float elapsedInvertGravityTime = 0f;
    
    private bool startTimer = false;

    public float Damage
    {
        get { return damage; }
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
            (!CollideWithPlayers && collision.gameObject.CompareTag("Player")) ||
            (!CollideWithEnemy && collision.gameObject.CompareTag("Enemy")))
        {
            Physics.IgnoreCollision(collision.collider, projectileCollider);
        }
        else
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
            {
                Detonate(collision.gameObject);
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

    private void Detonate(GameObject collisionObject)
    {
        if (DestroyOnCollision)
            Destroy(gameObject);

        if (damageRange == 0)
        {
            if (collisionObject != null)
            {
                if (collisionObject.CompareTag("Enemy"))
                {
                    Enemy enemy = collisionObject.GetComponent<Enemy>();
                    //TODO: damage
                }
                else if (collisionObject.CompareTag("Player"))
                {
                    Player player = collisionObject.GetComponent<Player>();
                    //TODO: damage
                }
            }
        }
        else
        {
            if (DoAOEDamageOnEnemy)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies != null)
                {
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        Enemy enemy = enemies[i].GetComponent<Enemy>();
                        float distance = Vector3.Distance(enemy.transform.position, transform.position);
                        if (distance <= damageRange)
                        {
                            //TODO: damage
                        }
                    }
                }
            }

            if (DoAOEDamageOnPlayer)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                if (players != null)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        Player player = players[i].GetComponent<Player>();
                        float distance = Vector3.Distance(player.transform.position, transform.position);
                        if (distance <= damageRange)
                        {
                            //TODO: damage
                        }
                    }
                }
            }
        }
    }
}


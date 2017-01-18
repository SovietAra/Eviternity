using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{
    private float distanceToObject;
    private DamageAbleObject dmgobjct;

    [SerializeField]
    private bool triggerByCollision, triggerByRange;

    [Range(0.1f, 10)]
    public float range;

    [Range(1, 1000)]
    public int damage;

    [SerializeField]
    [Range(0, 100f)]
    private float damageRange = 5;

    [SerializeField]
    private GameObject plane;

    [SerializeField]
    private GameObject explosion;

    // Use this for initialization
    void Start()
    {
        dmgobjct = GetComponent<DamageAbleObject>();
        dmgobjct.OnDeath += Dmgobjct_OnDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerByRange)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < playerList.Length; i++)
            {
                distanceToObject = Vector3.Distance(playerList[i].transform.position, transform.position);
                if (distanceToObject < range)
                {
                    Detonate();
                    break;
                }
            }
        }
    }

    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        Detonate();
    }

    private void Detonate()
    {
        if (explosion != null)
        {
            Explosion explosionScript = Instantiate(explosion, transform.position, transform.rotation).GetComponent<Explosion>();
            explosionScript.Init(damage, damageRange, null, true);
        }

        if(plane != null)
            Instantiate(plane, new Vector3(transform.position.x, transform.position.y - transform.localScale.y + 0.01f, transform.position.z), Quaternion.identity);

        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (triggerByCollision)
        {
            if (collision.gameObject.CompareTag("Projectile"))
            {
                Detonate();
            }
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnvironmentPillar : MonoBehaviour
{
    private BoxCollider spawnarea;

    [SerializeField]
    [Range(1, 5)]
    private int healthPoints;

    [SerializeField]
    [Range(1, 10)]
    private int debrisCount;

    [SerializeField]
    private GameObject[] debris;

    // Use this for initialization
    private void Start ()
    {
        spawnarea = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
        if (healthPoints == 0)
        {
            dropDebris();

            Destroy(gameObject);
        }
	}

    private void dropDebris()
    {
        if (debris.Length == 0)
        {
            return;
        }

        for (int i = 0; i < debrisCount; i++)
        {
            GameObject drop = debris[UnityEngine.Random.Range(0, debris.Length - 1)];
            Bounds b = spawnarea.bounds;

            Instantiate(drop, new Vector3(UnityEngine.Random.Range(b.min.x,b.max.x), UnityEngine.Random.Range(b.min.y, b.max.y), UnityEngine.Random.Range(b.min.z, b.max.z)), Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            healthPoints--;
        }
    }
}

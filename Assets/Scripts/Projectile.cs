using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    [SerializeField]
    [Range(0f, 1000f)]
    private float speed = 3f;

    [SerializeField]
    [Range(0f, 1000f)]
    private float damage = 1f;

    public bool DestroyOnCollision = true;
    
    public float Damage
    {
        get { return damage; }
    }


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(DestroyOnCollision)
            Destroy(gameObject);
    }
}

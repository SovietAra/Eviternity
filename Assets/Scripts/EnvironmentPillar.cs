using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPillar : MonoBehaviour {

    private bool collide = false;
    public GameObject projectile;

    [Range(1, 5)]
    public int healthPoints;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (collide)
        {
            healthPoints--;
        }

        if (healthPoints == 0)
        {
            Destroy(gameObject);

            Instantiate(projectile, transform.position - (transform.up), Quaternion.Euler(0, 2, 0));
            Instantiate(projectile, transform.position - (transform.up), Quaternion.Euler(0, -1, 0));
        }

        collide = false;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            collide = true;
        }
    }
}

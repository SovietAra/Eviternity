using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShield : MonoBehaviour
{
    private Collider currentCollider;
    private Ability ability;
    private DamageAbleObject damageAbleObject;

	// Use this for initialization
	private void Start ()
    {
        currentCollider = GetComponent<Collider>();
        ability = GetComponentInParent<Ability>();
        damageAbleObject = GetComponentInChildren<DamageAbleObject>();
        damageAbleObject.OnDeath += DamageAbleObject_OnDeath;
    }

    private void DamageAbleObject_OnDeath(object sender, System.EventArgs e)
    {
        ability.Abort();
    }

    // Update is called once per frame
    private void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Projectile"))
        {
            Physics.IgnoreCollision(currentCollider, collision.collider);
        }
    }
}

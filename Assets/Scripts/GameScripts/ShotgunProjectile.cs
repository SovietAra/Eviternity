using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunProjectile : MonoBehaviour {

    private Projectile proj;
    private Projectile[] childProj;

	// Use this for initialization
	void Start ()
    {
        proj = GetComponent<Projectile>();
        childProj = GetComponentsInChildren<Projectile>();

        if (proj != null)
        {
            for (int i = 0; i < childProj.Length; i++)
            {
                childProj[i].Attacker = proj.Attacker;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}

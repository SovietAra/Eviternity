using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Use this for initialization
    public float Speed = 3f;

	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += transform.forward * Time.deltaTime * Speed;
	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Console.WriteLine("collision");
        Debug.Log("Collision");
    }
}

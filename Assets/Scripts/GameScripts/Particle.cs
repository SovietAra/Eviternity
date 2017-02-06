using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestruction : MonoBehaviour {

    private ParticleSystem[] particleSystems;
	// Use this for initialization
	void Start () {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

    private List<ParticleSystem> particleSystems;
	// Use this for initialization
	void Start () {
        particleSystems = new List<ParticleSystem>();
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
            particleSystems.Add(particle);
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < particleSystems.Count; i++)
            if (!particleSystems[i].isPlaying)
            {
                particleSystems.RemoveAt(i);
                if (particleSystems.Count == 0)
                    Destroy(gameObject);
            }
    }
}

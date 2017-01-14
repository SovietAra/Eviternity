using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField]
    private bool activeOnKeyPressed;

    [SerializeField]
    private bool abortAble;
    
    [SerializeField]
    [Range(1f, 1000f)]
    private float maxEnergy;

    [SerializeField]
    [Range(1f, 1000f)]
    private float energy;

    [SerializeField]
    [Range(0.0f, 1000f)]
    private float energyRequiredPerSeconds;

    [SerializeField]
    [Range(0.0f, 1000f)]
    private float energyRegenerationPerSecond;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float regnerationDelay;

    [SerializeField]
    private bool allowActivationWithPartialEnergy;

    [SerializeField]
    private bool spawnObjectOnActivation;

    [SerializeField]
    private bool spawnOncePerActivation;

    [SerializeField]
    private bool destroyOnAbort;

    [SerializeField]
    private GameObject spawnObject;

    [SerializeField]
    [Range(0.0f, 1000f)]
    private float spawnForwardDistance;

    [SerializeField]
    private Vector3 spawnTranslation;

    [SerializeField]
    private bool allowMultiplyObjects;

    [SerializeField]
    [Range(1, 100)]
    private int ObjectsAtTheSameTime;


    private List<GameObject> spawnedObjects;
    private bool active;
    private bool keyPressed;
    private bool regenerating;
    private bool allowSpawning;
    private float elapsedRegenerationDelay;

    public event EventHandler OnActivated;
    public event EventHandler OnAbort;
    public event EventHandler OnRegenerating;
    public event EventHandler OnAbortRegeneration;
    public event EventHandler OnReachedMaxEnergy;
    public event EventHandler OnObjectSpawned;

	// Use this for initialization
	private void Start ()
    {
        allowSpawning = true;
        spawnedObjects = new List<GameObject>();
    }
	
	// Update is called once per frame
	private void Update ()
    { 
        if (active && (!activeOnKeyPressed || (activeOnKeyPressed && keyPressed)))
        {
            keyPressed = false;
            energy -= energyRequiredPerSeconds * Time.deltaTime;
            if (energy <= 0)
            {
                Abort();
            }
        }
        else if(active && activeOnKeyPressed && !keyPressed)
        {
            Abort();
        }
        else if(!active)
        {
            if(regenerating)
            {
                elapsedRegenerationDelay += Time.deltaTime;
                if (elapsedRegenerationDelay > regnerationDelay)
                {
                    Regenerate(energyRegenerationPerSecond * Time.deltaTime);
                }
            }
        }
    }

    private void Activate()
    {
        active = true;
        keyPressed = true;
        AbortRegeneration();

        if(spawnObjectOnActivation && spawnObject != null)
        {
            if(spawnOncePerActivation)
            {
                if (allowSpawning)
                {
                    allowSpawning = !Spawn();
                }
            }          
            else
            {
                Spawn();
            }
        }

        if (OnActivated != null)
            OnActivated(this, null);
    }

    private void Deactivate()
    {
        regenerating = true;
        active = false;
        keyPressed = false;
        allowSpawning = true;

        if(destroyOnAbort)
        {
            Despawn();
        }

        if (OnAbort != null)
            OnAbort(this, null);
    }

    private bool Spawn()
    {
        if (spawnedObjects.Count == 0 || (allowMultiplyObjects && ObjectsAtTheSameTime < spawnedObjects.Count))
        {
            GameObject gobj = Instantiate(spawnObject);
            gobj.transform.position = transform.parent.position + (transform.parent.forward * spawnForwardDistance) + spawnTranslation;
            gobj.transform.localRotation *= transform.parent.rotation;
            gobj.transform.parent = transform;
            
            spawnedObjects.Add(gobj);
            if (OnObjectSpawned != null)
                OnObjectSpawned(this, null);

            return true;
        }
        return false;
    }

    private void Despawn()
    {
        foreach (GameObject item in spawnedObjects)
        {
            Destroy(item);
        }
        spawnedObjects.Clear();
    }

    public virtual bool Use()
    {
        if (!active || (activeOnKeyPressed && active))
        {
            if (allowActivationWithPartialEnergy && energy >= energyRequiredPerSeconds)
            {
                Activate();
                return true;
            }
            else if(!allowActivationWithPartialEnergy && energy == maxEnergy)
            {
                Activate();
                return true;
            }
        }
        return false;
    }

    public virtual void Abort()
    {
        if(abortAble)
        {
            Deactivate();
        }
    }
    
    public virtual void AbortRegeneration()
    {
        regenerating = false;
        elapsedRegenerationDelay = 0f;

        if (OnAbortRegeneration != null)
            OnAbortRegeneration(this, null);
    }

    public virtual void Regenerate(float value)
    {
        energy += value;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;

            if (OnReachedMaxEnergy != null)
                OnReachedMaxEnergy(this, null);
        }
    }
}

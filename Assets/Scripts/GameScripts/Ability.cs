using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField]
    private bool activeOnKeyPressed = false;

    [SerializeField]
    private bool abortAble = true;
    
    [SerializeField]
    [Range(1f, 1000f)]
    private float maxEnergy = 100f;

    [SerializeField]
    [Range(1f, 1000f)]
    private float energy = 100f;

    [SerializeField]
    [Range(0.0f, 1000f)]
    private float energyRequiredPerSeconds = 10f;

    [SerializeField]
    [Range(0.0f, 1000f)]
    private float energyRegenerationPerSecond = 10f;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float regnerationDelay = 2f;

    [SerializeField]
    private bool allowActivationWithPartialEnergy = false;

    [SerializeField]
    private bool spawnObjectOnActivation = true;

    [SerializeField]
    private bool spawnOncePerActivation = true;

    [SerializeField]
    private bool destroyOnAbort = true;

    [SerializeField]
    private GameObject spawnObject = null;

    [SerializeField]
    [Range(0.0f, 1000f)]
    private float spawnForwardDistance = 1;

    [SerializeField]
    private Vector3 spawnTranslation = Vector3.zero;

    [SerializeField]
    private bool allowMultiplyObjects = false;

    [SerializeField]
    [Range(1, 100)]
    private int ObjectsAtTheSameTime = 1;


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
        if (OnRegenerating != null)
            OnRegenerating(this, null);

        if (energy > maxEnergy)
        {
            energy = maxEnergy;

            if (OnReachedMaxEnergy != null)
                OnReachedMaxEnergy(this, null);
        }
    }
}

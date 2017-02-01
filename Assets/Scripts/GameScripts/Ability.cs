using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public float AnimationDuration = 0;

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
    [Range(0.0f, 1000f)]
    private float energyRequiredOnce = 0;

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
    [Range(-100.0f, 100f)]
    private float spawnForwardDistance = 1;

    [SerializeField]
    private Vector3 spawnTranslation = Vector3.zero;

    [SerializeField]
    private bool allowMultiplyObjects = false;

    [SerializeField]
    [Range(1, 100)]
    private int objectsAtTheSameTime = 1;
    
    [Range(-10000f, 10000f)]
    public float AbilityValue = 0;

    [SerializeField]
    private bool parentObject = false;

    [SerializeField]
    private bool MultiplyRotation = true;

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
    public event EventHandler OnUsing;

    public float MaxEnergy
    {
        get { return maxEnergy; }
    }

    public float Energy
    {
        get { return energy; }
    }

    public bool IsActive
    {
        get { return active; }
    }

	// Use this for initialization
	private void Start ()
    {
        allowSpawning = true;
        spawnedObjects = new List<GameObject>();
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if (active && !activeOnKeyPressed)
        {
            keyPressed = false;
            if (energyRequiredOnce > 0)
            {
                energy -= energyRequiredOnce;
                Deactivate();
            }
            else
            {
                energy -= energyRequiredPerSeconds * Time.deltaTime;
                if (energy <= 0)
                {
                    Deactivate();
                }
            }

            if (OnUsing != null)
                OnUsing(this, EventArgs.Empty);
        }
        else if (active && activeOnKeyPressed && keyPressed)
        {
            keyPressed = false;
            energy -= energyRequiredPerSeconds * Time.deltaTime;
            if (energy <= 0)
            {
                Deactivate();
            }

            if (OnUsing != null)
                OnUsing(this, EventArgs.Empty);
        }
        else if (active && activeOnKeyPressed && !keyPressed)
        {
            Deactivate();
        }
        else if (!active)
        {
            if (regenerating)
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

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                i--;
            }
        }

        if (spawnObjectOnActivation && spawnObject != null)
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
        if (spawnedObjects.Count == 0 || (allowMultiplyObjects && objectsAtTheSameTime < spawnedObjects.Count))
        {
            GameObject gobj = Instantiate(spawnObject);
            gobj.transform.position = transform.parent.position + (transform.parent.forward * spawnForwardDistance) + spawnTranslation;
            if(MultiplyRotation)
                gobj.transform.localRotation *= transform.parent.rotation;

            if(parentObject)
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

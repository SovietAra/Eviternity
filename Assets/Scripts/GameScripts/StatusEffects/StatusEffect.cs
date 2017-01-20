using System;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    [Range(0f, 30f)]
    public float Duration;
    public bool DestroyOnDeactivation = true;
    public bool ResetSameStatus = false;
    public bool StackAble = false;

    protected float elapsedTime = 0;
    protected bool active = false;
    protected GameObject characterGameObject;

    public event EventHandler OnActivate;
    public event EventHandler OnDeactivate;

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        if (active)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= Duration)
            {
                Deactivate();
            }
            else
            {
                Do();
            }
        }
    }

    public virtual void ResetTImer()
    {
        elapsedTime = 0f;
    }

    public virtual void Activate(GameObject gameObject)
    {
        if (!active)
        {
            characterGameObject = gameObject;
            CheckEffects();
            active = true;

            if (OnActivate != null)
                OnActivate(this, EventArgs.Empty);
        }
    }

    private void CheckEffects()
    {
        if (characterGameObject != null)
        {
            StatusEffect[] effects = characterGameObject.GetComponentsInChildren<StatusEffect>();
            for (int i = 0; i < effects.Length; i++)
            {
                if(effects[i] != this && effects[i].GetType() == this.GetType())
                {
                    if(ResetSameStatus)
                    {
                        effects[i].ResetTImer();
                    }

                    if(!StackAble)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
    }

    public virtual void Deactivate()
    {
        active = false;
        ResetTImer();   
        if (OnDeactivate != null)
            OnDeactivate(this, EventArgs.Empty);

        if (DestroyOnDeactivation)
            Destroy(gameObject);
    }

    public abstract void Do();
}




using System;
using UnityEngine;

public class Poison : StatusEffect
{
    [SerializeField]
    [Range(0, 10000)]
    private float damagePerSecond = 10;

    private DamageAbleObject damageScript;

    public override void Start()
    {
        base.Start();
        OnActivate += Poison_OnActivate;
    }

    private void Poison_OnActivate(object sender, EventArgs e)
    {
        if(characterGameObject != null)
        {
            GetDamageScript();
        }
        else
        {
            Deactivate();
        }
    }

    public override void Do()
    {
        if (damageScript == null)
            GetDamageScript();

        if (damageScript != null)
            damageScript.DoDamage(damagePerSecond * Time.deltaTime);
    }

    private void GetDamageScript()
    {
        if (characterGameObject != null)
        {
            damageScript = characterGameObject.GetComponent<DamageAbleObject>();
            if (damageScript == null)
            {
                Deactivate();
            }
        }
        else
        {
            Deactivate();

        }
    }
}


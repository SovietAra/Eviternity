using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{

    private float distanceToObject;
    public bool triggerByCollision, triggerByRange;

    [Range(0.1f, 10)]
    public float range;

    [Range(1, 1000)]
    public int damage;

    private DamageAbleObject dmgobjct;

    // Use this for initialization
    void Start()
    {
        dmgobjct = GetComponent<DamageAbleObject>();
        dmgobjct.OnDeath += Dmgobjct_OnDeath;
    }

    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        Detonate();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        if (triggerByRange)
        {
            for (int i = 0; i < playerList.Length; i++)
            {
                distanceToObject = Vector3.Distance(playerList[i].transform.position, transform.position);
                if (distanceToObject < range)
                {
                    Detonate();
                    break;
                }
            }
        }
    }

    private void Detonate()
    {
        DamageAbleObject[] damageAbleObjects = GameObject.FindObjectsOfType<DamageAbleObject>();
        foreach (DamageAbleObject item in damageAbleObjects)
        {
            distanceToObject = Vector3.Distance(item.transform.position, transform.position);
            if (distanceToObject < range)
            {
                item.DoDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}

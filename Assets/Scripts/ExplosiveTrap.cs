using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{

    private float distanceToObject;

    [SerializeField]
    private bool triggerByCollision, triggerByRange;

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

    // Update is called once per frame
    void Update()
    {
        if (triggerByRange)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

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

    private void Dmgobjct_OnDeath(object sender, System.EventArgs e)
    {
        Detonate();
    }

    private void Detonate()
    {
        DamageAbleObject[] damageAbleObjects = FindObjectsOfType<DamageAbleObject>();
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

    public void OnCollisionEnter(Collision collision)
    {
        if (triggerByCollision)
        {
            if (collision.gameObject.CompareTag("Projectile"))
            {
                Detonate();
            }
        }
    }
}

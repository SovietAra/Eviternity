using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Transform tr_Player;
    [SerializeField]
    [Range(0.1f, 100.0f)]
    float RotationSpeed = 3.0f;
    [SerializeField]
    [Range(0.1f, 100.0f)]
    float MoveSpeed = 3.0f;
    float distanceToPlayer;
    [SerializeField]
    [Range(0.1f, 100.0f)]
    float AttackRange = 2.0f;
    [SerializeField]
    [Range(0.1f, 100.0f)]
    float ViewRange = 15.0f;
    private float elapsedAttackDelay = 0f;
    [SerializeField]
    [Range(0.1f, 60f)]
    float attackDelay = 1f;
    float enemyfront;

    public GameObject Projectile;
    float AimDirection;

    private bool AimsAtPlayer = false;
    RaycastHit hit;


    // Use this for initialization
    void Start()
    {
        tr_Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        distanceToPlayer = Vector3.Distance(tr_Player.position, transform.position);
        //Look at Player
        transform.rotation = Quaternion.Slerp(transform.rotation
                                             , Quaternion.LookRotation(tr_Player.position - transform.position)
                                             , RotationSpeed * Time.deltaTime);

        //Follow Player
        if (AttackRange < distanceToPlayer && distanceToPlayer < ViewRange)
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
        if (distanceToPlayer < AttackRange)
        {
            TryShoot();
        }

        enemyfront = transform.eulerAngles.y;
        Debug.DrawLine(transform.position, hit.point);
    }

    private void TryShoot()
    {
        elapsedAttackDelay += Time.deltaTime;
        if (elapsedAttackDelay > attackDelay)
        {

            elapsedAttackDelay = 0f;
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0f, enemyfront, 0f));
        }
    }

}

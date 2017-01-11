using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    Transform tr_Player;
    [Range(0.1f, 100.0f)]
    public float RotationSpeed = 3.0f;
    [Range(0.1f, 100.0f)]
    public float MoveSpeed = 3.0f;
    float distanceToPlayer;
    [Range(0.1f, 100.0f)]
    public float AttackRange = 2.0f;
    [Range(0.1f, 100.0f)]
    public float ViewRange = 15.0f;
    private float elapsedAttackDelay = 0f;
    [Range(0.1f, 60f)]
    public float attackDelay = 1f;


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
    }

}

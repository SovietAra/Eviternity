using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

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
    private bool AimsAtPlayer = false;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        tr_Player = GameObject.FindGameObjectWithTag("Player").transform;
        if (tr_Player)
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

    //Shoots raycast and checks if it collides. If yes and the colliding object is a player start shooting in tryshoot method
    //public bool Aiming()
    //{
    //    Physics.Raycast(transform.position, transform.forward, 10000);


    //    return AimsAtPlayer;
    //}

}

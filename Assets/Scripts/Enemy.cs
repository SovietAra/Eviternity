using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    Transform tr_Player;
    public float f_RotSpeed = 3.0f;
    public float f_MoveSpeed = 3.0f;
    float distanceToPlayer;
    public float attackRange = 2.0f;
    public float viewRange = 15.0f;

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
                                             , f_RotSpeed * Time.deltaTime);
        //Follow Player
        if (attackRange < distanceToPlayer && distanceToPlayer < viewRange)
        {
            transform.position += transform.forward * f_MoveSpeed * Time.deltaTime;
        }
    }
}

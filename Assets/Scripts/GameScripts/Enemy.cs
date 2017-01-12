using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //TODO Change fields to private and check everything for styleguide(pascalCase)
    [SerializeField]
    [Range(0.1f, 100.0f)]
    float RotationSpeed = 3.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    float MoveSpeed = 3.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    float attackRange = 6.0f;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    float ViewRange = 15.0f;

    [SerializeField]
    [Range(0.1f, 10000f)]
    float EnemyHealth = 10.0f;

    [SerializeField]
    [Range(0.1f, 60f)]
    float attackDelay = 1f;
    private float elapsedAttackDelay = 0f;

    float enemyfront;
    float distanceToPlayer;
    float AimDirection;
    private GameObject TargetPlayer;
    public GameObject Projectile;

    private bool isValidTarget = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckAlivePlayers();

        //TODO change tr_player to isValidTarget and maybe consider viewrange
        if (isValidTarget) //On player found
        {
            distanceToPlayer = Vector3.Distance(TargetPlayer.transform.position, transform.position);
            //Look at Player
            transform.rotation = Quaternion.Slerp(transform.rotation
                                                 , Quaternion.LookRotation(TargetPlayer.transform.position - transform.position)
                                                 , RotationSpeed * Time.deltaTime);

            //Follow Player
            if (attackRange < distanceToPlayer && distanceToPlayer < ViewRange)
            {
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
            }
            else if(distanceToPlayer < ViewRange)
            {
                TargetPlayer = null;
                isValidTarget = false;
            }

            if (distanceToPlayer < attackRange)
            {
                TryShoot();
            }

            enemyfront = transform.eulerAngles.y;
        }
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


    private void CheckAlivePlayers()
    {
        if (TargetPlayer != null) //TODO und in range
            return;

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players != null)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Player checkedPlayer = Players[i].GetComponent<Player>();
                if (!checkedPlayer.IsDead)
                {
                    isValidTarget = true;
                    TargetPlayer = Players[i];
                    return;
                }
                else
                {
                    isValidTarget = false;
                }
            }
        }
    }
}

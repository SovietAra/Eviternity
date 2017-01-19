using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts;
public class MoveScript : MonoBehaviour
{
    private Rigidbody physics;
    private Vector3 movement;
    private GameObject statusEffect;

    public event EventHandler<OnMovingArgs> OnMoving;

    private float movementMultiplicator;

    private void Start()
    {
        physics = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //movementMultiplicator = 
    }

    private void FixedUpdate()
    {
        OnMovingArgs args = new OnMovingArgs(movement);

        if (OnMoving != null)
            OnMoving(this, args);

        if (!args.Cancel)
        {
            physics.velocity = args.Velocity;
            movement = Vector3.zero;
        }
    }

    public void Move(Vector3 movement, GameObject statusEffect)
    {
        this.movement = movement;
        this.statusEffect = statusEffect;
    }
}



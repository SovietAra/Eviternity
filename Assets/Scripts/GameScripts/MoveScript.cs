using System;
using UnityEngine;
using Assets.Scripts;

public class MoveScript : MonoBehaviour
{
    private Rigidbody physics;
    private Vector3 movement;
    private float defaultMovementMultiplicator;
    
    [Range(0, 2)]
    public float MovementMultiplicator = 1f;
    public bool AddGravity = true;

    public event EventHandler<OnMovingArgs> OnMoving;
 
    public float DefaultMovementMultiplicator
    {
        get
        {
            return defaultMovementMultiplicator;
        }
    }

    private void Start()
    {
        defaultMovementMultiplicator = MovementMultiplicator;
        physics = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        OnMovingArgs args = new OnMovingArgs(movement + (AddGravity ? Physics.gravity : Vector3.zero));
        
        if (OnMoving != null)
            OnMoving.Invoke(this, args);

        if (!args.Cancel)
        {
            physics.velocity = (args.Velocity * MovementMultiplicator);
            movement = Vector3.zero;
        }
    }

    public void Move(Vector3 movement)
    {
        this.movement = movement;
    }

    public void ResetMultiplicator()
    {
        MovementMultiplicator = defaultMovementMultiplicator;
    }
}



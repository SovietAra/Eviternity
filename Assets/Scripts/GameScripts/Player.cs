﻿using Assets.Scripts;
using System;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    private PlayerIndex index;
    private bool hasPlayerIndex;
    private bool lostConnection;
    private bool isDead;
    private Vector3 moveVector;
    private Vector3 velocity;

    private float elapsedAttackDelay = 0f;
    private float elapsedDashTime = 0f;
    
    private float angle;
    private Quaternion targetRotation;
    private GamePadState prevState;

    [SerializeField]
    [Range(1f, 10000f)]
    private float health = 10;

    [SerializeField]
    [Range(1f, 100f)]
    private float speed = 1f;

    [SerializeField]
    [Range(1f, 100f)]
    private float dashSpeed = 5f;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float dashTime = 1.25f;

    [SerializeField]
    [Range(1, 100)]
    private float regenerationPerSecond = 5f;

    [SerializeField]
    [Range(0.1f, 100)]
    private float regenerationDuration = 5f;

    [SerializeField]
    [Range(0.1f, 100)]
    private float maxRegenerationDuration = 5f;

    [SerializeField]
    [Range(0.001f, 5f)]
    private float regenerationRefill = 0.01f;

    [SerializeField]
    [Range(0.1f, 60f)]
    public float attackDelay = 1f;

   



    public bool Freeze = false;
    public bool TwoStickMovement = false;
    public GameObject Projectile;

    [HideInInspector]
    public event EventHandler<PlayerEventArgs> OnPlayerExit;

    [HideInInspector]
    public PlayerIndex Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
            hasPlayerIndex = true;
        }
    }

    public bool IsDead
    {
        get { return isDead; }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (hasPlayerIndex)
        {
            GamePadState state = GamePad.GetState(Index);
            if (state.IsConnected)
            {
                /*if(lostConnection)
                {
                    lostConnection = false;
                    GamePadManager.Connect((int)index);
                }*/

                if (!isDead)
                {
                    UpdateTimers();
                    CheckHealth();
                    Input(state);
                    UpdatePosition();
                    UpdateRotation();
                }
            }
            else
            {
                GamePadManager.Disconnect(Index);
                lostConnection = true;
                GlobalReferences.CurrentGameState = GlobalReferences.GameState.ConnectionLost;
            }
        }
	}

    private void UpdateTimers()
    {
        elapsedAttackDelay += Time.deltaTime;
        elapsedDashTime += Time.deltaTime;
    }

    private void Input(GamePadState state)
    {
        if(state.Buttons.Start == ButtonState.Pressed)
        {
            TryPause();
        }

        if(state.Buttons.Back == ButtonState.Pressed)
        {
            TryExit();
        }

        if(state.Buttons.A == ButtonState.Pressed)
        {
            TryDash();
        }

        if(state.Buttons.B == ButtonState.Pressed)
        {
            //unused
        }

        if(state.Buttons.X == ButtonState.Pressed)
        {
            TryHeal();
        }
        else
        {
            TryFillRegeneration();
        }

        if(state.Buttons.Y == ButtonState.Pressed)
        {
            TryAbillity();
        }

        if(state.Triggers.Right > 0)
        {
            TryPrimaryAttack();
        }

        if(state.Triggers.Left > 0)
        {
            TrySecondaryAttack();
        }

        Vector2 leftStick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        Vector2 rightStick = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        TryMove(leftStick, rightStick);
    }
    
    private void TryMove(Vector2 leftStick, Vector2 rightStick)
    {
        if (!Freeze)
        {
            moveVector = Vector3.forward * leftStick.y * Time.deltaTime * speed;
            moveVector += Vector3.right * leftStick.x * Time.deltaTime * speed;

            float leftAngle = FixAngle(CalculateAngle(new Vector2(leftStick.x * -1, leftStick.y), Vector2.zero) - 90);
            if (leftStick != Vector2.zero)
            {
                DoRotation(leftAngle);
            }
            else
            {
                float rightAngle = FixAngle(CalculateAngle(new Vector2(rightStick.x * -1, rightStick.y), Vector2.zero) - 90);
                if (rightStick != Vector2.zero)
                {
                    DoRotation(rightAngle);
                }
            }
        }
    }

    private void DoRotation(float angle)
    {
        this.angle = angle;
        targetRotation = Quaternion.Euler(0, angle, 0);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 14);
    }

    private void UpdatePosition()
    {
        transform.position += moveVector + velocity;
        velocity -= (velocity * 0.1f);
    }

    private float CalculateAngle(Vector2 target, Vector2 source)
    {
        return ((float)Math.Atan2(target.y - source.y, target.x - source.x)) * (180f / (float)Math.PI);
    }

    private float FixAngle(float value)
    {
        if (value < 0)
            return 360 + value;

        return value;
    }

    private void TryHeal()
    {
        if (regenerationDuration > 0)
        {
            regenerationDuration -= Time.deltaTime;
            health += regenerationPerSecond * Time.deltaTime;
        }
    }

    private void TryFillRegeneration()
    {
        regenerationDuration += regenerationRefill * Time.deltaTime;
        if (regenerationDuration > maxRegenerationDuration)
            regenerationDuration = maxRegenerationDuration;
    }

    private void TryDash()
    {
        if (elapsedDashTime >= dashTime)
        {
            velocity = transform.forward * Time.deltaTime * dashSpeed;
            elapsedDashTime = 0f;
        }
    }

    private void TryAbillity()
    {
    }

    private void TryPrimaryAttack()
    {
        if (elapsedAttackDelay > attackDelay)
        {
            elapsedAttackDelay = 0f;
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0.0f, (angle), 0));
        }
    }

    private void TrySecondaryAttack()
    {
        if (elapsedAttackDelay > attackDelay)
        {
            elapsedAttackDelay = 0f;
            Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0.0f, (angle), 0));
        }
    }

    private void TryExit()
    {
        if (OnPlayerExit != null)
            OnPlayerExit(this, new PlayerEventArgs(gameObject, this));
    }

    private void TryPause()
    {
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Pause;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            DoDamage(collision.gameObject);
        }
    }

    private void DoDamage(GameObject projectile)
    {
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        health -= projectileScript.Damage;
        if(health <= 0)
        {
            isDead = true;
            Destroy(gameObject);          
        }
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            DestroyImmediate(this);
        }
    }
}

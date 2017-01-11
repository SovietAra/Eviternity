using Assets.Scripts;
using System;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    private PlayerIndex index;
    private bool hasPlayerIndex;
    private bool lostConnection;
    private bool isDead;
    private float elapsedAttackDelay = 0f;
    private float angle;
    private GamePadState prevState;

    [SerializeField]
    [Range(1f, 10000f)]
    private float health = 10;
    [SerializeField]
    [Range(0.1f, 60f)]
    public float attackDelay = 1f;

    public bool Freeze = false;
    public bool TwoStickMovement = false;
    public GameObject Projectile;
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!hasPlayerIndex || lostConnection)
        {
            index = GamePadManager.GetPlayerIndex();
            hasPlayerIndex = true;
            lostConnection = false;
        }
        else
        {
            GamePadState state = GamePad.GetState(index);
            if (state.IsConnected)
            {
                if (!isDead)
                {
                    CheckHealth();
                    TryMove(state);
                    TryShoot(state);
                }
            }
            else
            {
                GamePadManager.ConnectionLost(index);
                lostConnection = true;
                //Pause
            }
        }
	}
    
    private void TryMove(GamePadState state)
    {
        if(!Freeze)
        {
            if (TwoStickMovement)
            {
                transform.position += transform.forward * state.ThumbSticks.Left.Y * Time.deltaTime;
                transform.position += transform.right * state.ThumbSticks.Left.X * Time.deltaTime;

                transform.localRotation *= Quaternion.Euler(0.0f, state.ThumbSticks.Right.X * 25.0f * Time.deltaTime, 0.0f);
            }
            else
            {
                transform.position += Vector3.forward * state.ThumbSticks.Left.Y * Time.deltaTime;
                transform.position += Vector3.right * state.ThumbSticks.Left.X * Time.deltaTime;

                float leftAngle = FixAngle(CalculateAngle(new Vector2(state.ThumbSticks.Left.X * -1, state.ThumbSticks.Left.Y), Vector2.zero) - 90);              
                if (state.ThumbSticks.Left.X != 0 || state.ThumbSticks.Left.Y != 0)
                {
                    DoRotation(leftAngle);
                }
                else
                {
                    float rightAngle = FixAngle(CalculateAngle(new Vector2(state.ThumbSticks.Right.X * -1, state.ThumbSticks.Right.Y), Vector2.zero) - 90);
                    if (state.ThumbSticks.Right.X != 0 || state.ThumbSticks.Right.Y != 0)
                    {
                        DoRotation(rightAngle);
                    }
                }
            }
        }
    }

    private void DoRotation(float angle)
    {
        this.angle = angle;
        transform.localRotation = Quaternion.Euler(0.0f, (angle), 0);
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

    private void CheckHealth()
    {
        if(health <= 0)
        {
            DestroyImmediate(this);
        }
    }

    private void TryShoot(GamePadState state)
    {
        elapsedAttackDelay += Time.deltaTime;
        if (elapsedAttackDelay > attackDelay)
        {
            if (state.Triggers.Right > 0)
            {
                elapsedAttackDelay = 0f;
                Instantiate(Projectile, transform.position + (transform.forward), Quaternion.Euler(0.0f, (angle), 0));
             }
        }
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
}

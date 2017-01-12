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
    private Vector3 moveVector;
    private Vector3 velocity;

    private float elapsedAttackDelay = 0f;
    private float elapsedDashTime = 0f;
    
    private float angle;
    private Quaternion targetRotation;
    private GamePadState prevState;

    [Range(1f, 10000f)]
    public float TeamHealth = 10;
    
    [Range(1f, 10000f)]
    public float maxTeamHealth = 10;

    [SerializeField]
    [Range(1f, 10000f)]
    private float maxHealth = 10;

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
    [Range(0.1f, 60f)]
    public float attackDelay = 1f;

    public bool Freeze = false;
    public bool TwoStickMovement = false;
    public GameObject Projectile;

    private Camera _camera;
    private float _xMin, _xMax, _zMin, _zMax, clampedX, clampedZ;
    private Rigidbody _physics;

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
        _physics = GetComponent<Rigidbody>();
        _camera = Camera.main;
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
    void FixedUpdate()
    {
        Borders();
        _physics.MovePosition(new Vector3(clampedX, 0, clampedZ));
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
        if (health < maxHealth)
        {
            float addHealth = regenerationPerSecond * Time.deltaTime;
            if (TeamHealth < addHealth)
                addHealth = TeamHealth;

            if (addHealth + health > maxHealth)
            {
                addHealth -= (addHealth + health) - maxHealth;
            }
            health += addHealth;
            UpdateTeamHealth(-addHealth);
        }
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

    private void UpdateTeamHealth(float addHealth)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            Player playerScript = players[i].GetComponent<Player>();
            playerScript.TeamHealth += addHealth;
            if (playerScript.TeamHealth > playerScript.maxHealth)
                playerScript.TeamHealth = playerScript.maxHealth;
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

    private void CheckHealth()
    {
        if (health <= 0)
        {
            DestroyImmediate(this);
        }
    }
    public void Borders()
    {

        _xMax = (_camera.ViewportPointToRay(new Vector3(1, 1)).origin +
                _camera.ViewportPointToRay(new Vector3(1, 1)).direction *
                (-_camera.ViewportPointToRay(new Vector3(1, 1)).origin.y /
                 _camera.ViewportPointToRay(new Vector3(1, 1)).direction.y)).x;
        _zMax = (_camera.ViewportPointToRay(new Vector3(1, 1)).origin +
                _camera.ViewportPointToRay(new Vector3(1, 1)).direction *
                (-_camera.ViewportPointToRay(new Vector3(1, 1)).origin.y /
                 _camera.ViewportPointToRay(new Vector3(1, 1)).direction.y)).z;
        _xMin = (_camera.ViewportPointToRay(new Vector3(0, 0)).origin +
                _camera.ViewportPointToRay(new Vector3(0, 0)).direction *
                (-_camera.ViewportPointToRay(new Vector3(0, 0)).origin.y /
                 _camera.ViewportPointToRay(new Vector3(0, 0)).direction.y)).x;
        _zMin = (_camera.ViewportPointToRay(new Vector3(0, 0)).origin +
                _camera.ViewportPointToRay(new Vector3(0, 0)).direction *
                (-_camera.ViewportPointToRay(new Vector3(0, 0)).origin.y /
                 _camera.ViewportPointToRay(new Vector3(0, 0)).direction.y)).z;

        clampedX = Mathf.Clamp(transform.position.x, _xMin, _xMax);
        clampedZ = Mathf.Clamp(transform.position.z, _zMin, _zMax);
    }
}

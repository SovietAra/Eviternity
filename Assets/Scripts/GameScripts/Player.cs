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
    public float health = 10;

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

    public bool Freeze = false;
    public bool RotateOnMove = false;
    public GameObject PrimaryWeapon;
    public GameObject SecondaryWeapon;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;

    private Camera camera;
    private float xMin, xMax, zMin, zMax, clampedX, clampedZ;
    private Rigidbody physics;

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
        elapsedDashTime = dashTime;  
           
        physics = GetComponent<Rigidbody>();
        primaryWeapon = Instantiate(PrimaryWeapon, transform).GetComponent<Weapon>();
        secondaryWeapon = Instantiate(SecondaryWeapon, transform).GetComponent<Weapon>();
        camera = Camera.main;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (hasPlayerIndex)
        {
            GamePadState state = GamePad.GetState(Index);
            if (state.IsConnected)
            {
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
        physics.MovePosition(new Vector3(clampedX, 0, clampedZ));
    }

    private void UpdateTimers()
    {
        elapsedDashTime += Time.deltaTime;
    }

    private void Input(GamePadState state)
    {
        Vector2 leftStick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        Vector2 rightStick = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        TryMove(leftStick, rightStick);

        if (state.Buttons.Start == ButtonState.Pressed)
        {
            TryPause();
        }

        if(state.Buttons.Back == ButtonState.Pressed)
        {
            TryExit();
        }

        if(state.Buttons.B == ButtonState.Pressed)
        {
            //unused
        }

        bool executed = false;
        if (state.Buttons.X == ButtonState.Pressed)
        {
            executed = TryHeal();
        }

        if (state.Buttons.Y == ButtonState.Pressed && !executed)
        {
            executed = TryAbillity();
        }

        if (state.Buttons.A == ButtonState.Pressed && !executed)
        {
            executed = TryDash();
        }

        if (state.Triggers.Right > 0 && !executed)
        {
            executed = primaryWeapon.Fire(transform.position, transform.forward, angle);
        }

        if (state.Triggers.Left > 0 && !executed)
        {
            executed = secondaryWeapon.Fire(transform.position, transform.forward, angle);
        }
        
    }
    
    private void TryMove(Vector2 leftStick, Vector2 rightStick)
    {
        if (!Freeze)
        {
            moveVector = Vector3.forward * leftStick.y * Time.deltaTime * speed;
            moveVector += Vector3.right * leftStick.x * Time.deltaTime * speed;

            if (RotateOnMove)
            {
                float leftAngle = FixAngle(CalculateAngle(new Vector2(leftStick.x * -1, leftStick.y), Vector2.zero) - 90);
                if (leftStick != Vector2.zero)
                {
                    DoRotation(leftAngle);
                }
            }

            float rightAngle = FixAngle(CalculateAngle(new Vector2(rightStick.x * -1, rightStick.y), Vector2.zero) - 90);
            if (rightStick != Vector2.zero)
            {
                DoRotation(rightAngle);
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

    private bool TryHeal()
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

            if (addHealth != 0)
            {
                health += addHealth;
                UpdateTeamHealth(-addHealth);
                return true;
            }
        }
        return false;
    }

    private bool TryDash()
    {
        if (elapsedDashTime >= dashTime)
        {
            velocity = transform.forward * Time.deltaTime * dashSpeed;
            elapsedDashTime = 0f;
            return true;
        }

        return false;
    }

    private bool TryAbillity()
    {
        return true;
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
            Projectile projectileScript = collision.gameObject.GetComponent<Projectile>();
            DealDamage(projectileScript.Damage);
        }
    }

    public void DealDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
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
        xMax = (camera.ViewportPointToRay(new Vector3(1, 1)).origin +
                camera.ViewportPointToRay(new Vector3(1, 1)).direction *
                (-camera.ViewportPointToRay(new Vector3(1, 1)).origin.y /
                 camera.ViewportPointToRay(new Vector3(1, 1)).direction.y)).x;
        zMax = (camera.ViewportPointToRay(new Vector3(1, 1)).origin +
                camera.ViewportPointToRay(new Vector3(1, 1)).direction *
                (-camera.ViewportPointToRay(new Vector3(1, 1)).origin.y /
                 camera.ViewportPointToRay(new Vector3(1, 1)).direction.y)).z;
        xMin = (camera.ViewportPointToRay(new Vector3(0, 0)).origin +
                camera.ViewportPointToRay(new Vector3(0, 0)).direction *
                (-camera.ViewportPointToRay(new Vector3(0, 0)).origin.y /
                 camera.ViewportPointToRay(new Vector3(0, 0)).direction.y)).x;
        zMin = (camera.ViewportPointToRay(new Vector3(0, 0)).origin +
                camera.ViewportPointToRay(new Vector3(0, 0)).direction *
                (-camera.ViewportPointToRay(new Vector3(0, 0)).origin.y /
                 camera.ViewportPointToRay(new Vector3(0, 0)).direction.y)).z;

        clampedX = Mathf.Clamp(transform.position.x, xMin, xMax);
        clampedZ = Mathf.Clamp(transform.position.z, zMin, zMax);
    }
}

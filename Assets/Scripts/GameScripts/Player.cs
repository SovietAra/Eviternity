using Assets.Scripts;
using System;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    public static float TeamHealth = 10f;

    private PlayerIndex index;
    private bool hasPlayerIndex;
    private bool isDead;
    private Vector3 moveVector;
    private Vector3 velocity;
    
    private float elapsedDashTime = 0f;
    
    private float angle;
    private Quaternion targetRotation;
    
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
    public GameObject Ability;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    private Ability ability;
    private DamageAbleObject healthContainer;

    private Camera mainCamera;
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

    private Vector3 newForce;

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
        ability = Instantiate(Ability, transform).GetComponent<Ability>();
        healthContainer = GetComponent<DamageAbleObject>();
        healthContainer.OnDeath += HealthContainer_OnDeath;
        mainCamera = Camera.main;
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
                    Input(state);
                    UpdatePosition();
                    UpdateRotation();
                }
            }
            else
            {
                GamePadManager.Disconnect(Index);
                GlobalReferences.CurrentGameState = GlobalReferences.GameState.ConnectionLost;
            }
        }
	}

    void FixedUpdate()
    {
        //physics.velocity = newForce; //für physik movement einfügen!
        //newForce = Vector3.zero; //für physik movement einfügen!
        Borders();
        physics.MovePosition(new Vector3(clampedX, 0, clampedZ)); //<- Buggy: bei physik movement entfernen da man sich sonst nicht bewegen kann
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
        newForce = (moveVector + velocity) * 100;
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
            if(primaryWeapon != null)
                executed = primaryWeapon.PrimaryAttack(transform.position, transform.forward, angle);
        }

        if (state.Triggers.Left > 0 && !executed)
        {
            if(secondaryWeapon != null)
                executed = secondaryWeapon.PrimaryAttack(transform.position, transform.forward, angle);
        }

        if(state.Buttons.RightShoulder == ButtonState.Pressed && !executed)
        {
            if (primaryWeapon != null)
                executed = primaryWeapon.SecondaryAttack(transform.position, transform.forward, angle);
        }

        if (state.Buttons.RightShoulder == ButtonState.Pressed && !executed)
        {
            if (primaryWeapon != null)
                executed = secondaryWeapon.SecondaryAttack(transform.position, transform.forward, angle);
        }
    }
    
    private void TryMove(Vector2 leftStick, Vector2 rightStick)
    {
        if (!Freeze)
        {
            if(leftStick.y > 0.1f || leftStick.y < 0.1f)
                moveVector = Vector3.forward * leftStick.y * Time.deltaTime * speed;

            if (leftStick.x > 0.1f || leftStick.x < 0.1f)
                moveVector += Vector3.right * leftStick.x * Time.deltaTime * speed;
           

            if (RotateOnMove && moveVector != Vector3.zero)
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
        transform.position += moveVector + velocity; //Für Phyisk movement entfernen
        //velocity -= (velocity * 0.1f);
        velocity *= 0.8f;
        if (velocity.x < 0.1 && velocity.y > -0.1f && velocity.y < 0.1 && velocity.y > -0.1f && velocity.z < 0.1 && velocity.z > -0.1f)
            velocity = Vector3.zero;
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
        if (healthContainer.Health < healthContainer.MaxHealth)
        {
            float addHealth = regenerationPerSecond * Time.deltaTime;
            if (TeamHealth < addHealth)
                addHealth = TeamHealth;

            if (addHealth + healthContainer.Health > healthContainer.MaxHealth)
            {
                addHealth -= (addHealth + healthContainer.Health) - healthContainer.MaxHealth;
            }

            if (addHealth != 0)
            {
                healthContainer.Heal(addHealth);
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
            if (moveVector == Vector3.zero)
            {
                velocity = transform.forward * Time.deltaTime * dashSpeed;
            }
            else
            {
                velocity = ScaleVactorUp(moveVector) * Time.deltaTime * dashSpeed;
            }
            elapsedDashTime = 0f;
            return true;
        }

        return false;
    }

    private Vector3 ScaleVactorUp(Vector3 vec)
    {
        float x = Math.Abs(vec.x);
        float y = Math.Abs(vec.y);
        float z = Math.Abs(vec.z);
        float diff = 0f;
        if(x > y && x > z)
            diff = 1 - x;
        else if(y > z)
            diff = 1 - y;
        else
            diff = 1 - z;
        
        if(x != 0)
            x = vec.x + (vec.x > 0 ? diff : -diff);
        if(y != 0)
            y = vec.y + (vec.y > 0 ? diff : -diff);
        if(z != 0)
            z = vec.z + (vec.z > 0 ? diff : -diff);
        return new Vector3(x, y, z);
    }

    private bool TryAbillity()
    {
        if(ability != null)
        {
            return ability.Use();
        }
        return false;
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
        TeamHealth += addHealth;
    }

    private void HealthContainer_OnDeath(object sender, EventArgs e)
    {
        if (TeamHealth == 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
        else
        {
            float addHealth = healthContainer.MaxHealth * 2;
            if(TeamHealth < addHealth)
            {
                addHealth = TeamHealth / 2;
            }

            UpdateTeamHealth(-addHealth);
            healthContainer.Heal(addHealth);
        }
    }

    private void Borders()
    {
        xMax = (mainCamera.ViewportPointToRay(new Vector3(1, 1)).origin +
                mainCamera.ViewportPointToRay(new Vector3(1, 1)).direction *
                (-mainCamera.ViewportPointToRay(new Vector3(1, 1)).origin.y /
                 mainCamera.ViewportPointToRay(new Vector3(1, 1)).direction.y)).x;
        zMax = (mainCamera.ViewportPointToRay(new Vector3(1, 1)).origin +
                mainCamera.ViewportPointToRay(new Vector3(1, 1)).direction *
                (-mainCamera.ViewportPointToRay(new Vector3(1, 1)).origin.y /
                 mainCamera.ViewportPointToRay(new Vector3(1, 1)).direction.y)).z;
        xMin = (mainCamera.ViewportPointToRay(new Vector3(0, 0)).origin +
                mainCamera.ViewportPointToRay(new Vector3(0, 0)).direction *
                (-mainCamera.ViewportPointToRay(new Vector3(0, 0)).origin.y /
                 mainCamera.ViewportPointToRay(new Vector3(0, 0)).direction.y)).x;
        zMin = (mainCamera.ViewportPointToRay(new Vector3(0, 0)).origin +
                mainCamera.ViewportPointToRay(new Vector3(0, 0)).direction *
                (-mainCamera.ViewportPointToRay(new Vector3(0, 0)).origin.y /
                 mainCamera.ViewportPointToRay(new Vector3(0, 0)).direction.y)).z;
      
        clampedX = Mathf.Clamp(transform.position.x, xMin, xMax);
        clampedZ = Mathf.Clamp(transform.position.z, zMin, zMax);
    }
}

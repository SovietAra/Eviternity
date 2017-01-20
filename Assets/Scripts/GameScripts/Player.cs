using Assets.Scripts;
using System;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    #region statics
    public static float TeamHealth = 10f;
    public static float HealthRegenerationMultiplicator = 1f;
    public static float HealthRegenerationMulitplicatorOnDeath = 2f;
    #endregion

    #region privats
    private PlayerIndex index;
    private bool hasPlayerIndex;
    private bool isDead;
    private Vector3 moveVector;
    private Vector3 velocity;
    private Vector3 finalVelocity;

    private float elapsedDashTime = 0f;
    private float elapsedReviveDelay = 0f;
    private float attackInProgressTimer = 0f;

    private float angle;
    private Quaternion targetRotation;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    private Ability ability;
    private Ability secondaryAbility;
    private DamageAbleObject healthContainer;
    private MoveScript moveScript;

    private Camera mainCamera;
    private float xMin, xMax, zMin, zMax, clampedX, clampedZ;
    private Rigidbody physics;
    private GameObject transparentObject;

    private GamePadState prevState;
    #endregion

    #region InspectorFields
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
    [Range(0.1f, 30f)]
    private float reviveDelay = 2f;

    public bool Freeze = false;
    public bool RotateOnMove = false;
    public GameObject PrimaryWeapon;
    public GameObject SecondaryWeapon;
    public GameObject Ability;
    public GameObject SecondaryAbility;

    public bool OnIce;
    public static Vector3 Checkpos;
    #endregion

    #region EventHandlers
    [HideInInspector]
    public event EventHandler<PlayerEventArgs> OnPlayerExit;
    #endregion

    #region Properties
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
    #endregion

    #region UnityMethodes
    // Use this for initialization
    private void Start ()
    {
        mainCamera = Camera.main;
        mainCamera.GetComponentInParent<NewFollowingCamera>().AddToCamera(transform);
        elapsedDashTime = dashTime;  
           
        physics = GetComponent<Rigidbody>();
        if (PrimaryWeapon != null)
        {
            primaryWeapon = Instantiate(PrimaryWeapon, transform).GetComponent<Weapon>();
            primaryWeapon.OnPrimaryAttack += PrimaryWeapon_OnPrimaryAttack;
            primaryWeapon.OnSecondaryAttack += PrimaryWeapon_OnSecondaryAttack;
        }
        if (SecondaryWeapon != null)
        {
            secondaryWeapon = Instantiate(SecondaryWeapon, transform).GetComponent<Weapon>();
            secondaryWeapon.OnPrimaryAttack += SecondaryWeapon_OnPrimaryAttack;
            secondaryWeapon.OnSecondaryAttack += SecondaryWeapon_OnSecondaryAttack;
        }

        if (Ability != null)
        {
            ability = Instantiate(Ability, transform).GetComponent<Ability>();
            ability.OnActivated += Ability_OnActivated;
            ability.OnAbort += Ability_OnAbort;
        }
        if (SecondaryAbility != null)
        {
            secondaryAbility = Instantiate(SecondaryAbility, transform).GetComponent<Ability>();
            secondaryAbility.OnActivated += SecondaryAbility_OnActivated;
            secondaryAbility.OnAbort += SecondaryAbility_OnAbort;
        }

        healthContainer = GetComponent<DamageAbleObject>();
        if (healthContainer != null)
        {
            healthContainer.OnDeath += HealthContainer_OnDeath;
            healthContainer.OnReceiveDamage += HealthContainer_OnReceiveDamage;
            healthContainer.OnReceiveHealth += HealthContainer_OnReceiveHealth;
        }

        moveScript = GetComponent<MoveScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (hasPlayerIndex)
        {
            GamePadState state = GamePad.GetState(Index);
            if (state.IsConnected)
            {
                UpdateTimers();
                if (isDead)
                {
                    RevivePlayer();
                }
                else
                {
                    Input(state);
                    UpdateVelocity();
                    UpdateRotation();
                }
                prevState = state;
            }
            else
            {
                GamePadManager.Disconnect(Index);
                GlobalReferences.CurrentGameState = GlobalReferences.GameState.ConnectionLost;
            }
        }
    }

    private void FixedUpdate()
    {
        if(OnIce)
        {
            InputOnIce();
        }
        CheckOverlappingObjects();
    }
    #endregion
    
    #region UpdateMethodes
    private void UpdateTimers()
    {
        elapsedDashTime += Time.deltaTime;
        if(isDead)
            elapsedReviveDelay += Time.deltaTime;

        if (attackInProgressTimer > 0)
            attackInProgressTimer -= Time.deltaTime;

    }

    private void Input(GamePadState state)
    {
        Vector2 leftStick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        Vector2 rightStick = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        TryMove(leftStick, rightStick);

        finalVelocity = (moveVector + velocity) * 100;
        if(moveScript !=null)
        {
            moveScript.Move(finalVelocity);
        }

        bool executed = false;

        if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released)
        {
            GlobalReferences.CurrentGameState = GlobalReferences.GameState.Pause;
        }

        if(state.Buttons.Back == ButtonState.Pressed)
        {
            if (OnPlayerExit != null)
                OnPlayerExit(this, new PlayerEventArgs(gameObject, this));
        }

        if (state.Buttons.Y == ButtonState.Pressed)
        {
            executed = TryHeal();
        }

        if (state.Buttons.B == ButtonState.Pressed && !executed)
        {
            executed = TrySecondaryAbility();
        }
        
        if (state.Buttons.X == ButtonState.Pressed && !executed)
        {
            executed = TryAbillity();
        }
        
        if (state.Buttons.A == ButtonState.Pressed && !executed)
        {
            executed = TryDash();
        }

        if (attackInProgressTimer <= 0)
        {
            if (state.Triggers.Right > 0 && !executed)
            {
                if (primaryWeapon != null)
                    executed = primaryWeapon.PrimaryAttack(transform.position, transform.forward, angle);
            }

            if (state.Triggers.Left > 0 && !executed)
            {
                if (secondaryWeapon != null)
                    executed = secondaryWeapon.PrimaryAttack(transform.position, transform.forward, angle);
            }

            if (state.Buttons.RightShoulder == ButtonState.Pressed && !executed)
            {
                if (primaryWeapon != null)
                    executed = primaryWeapon.SecondaryAttack(transform.position, transform.forward, angle);
            }

            if (state.Buttons.LeftShoulder == ButtonState.Pressed && !executed)
            {
                if (secondaryWeapon != null)
                    executed = secondaryWeapon.SecondaryAttack(transform.position, transform.forward, angle);
            }
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

    private void CheckOverlappingObjects()
    {
        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Untagged"))
            {
                if (ChangeColor(hit.transform.gameObject, 0.1f))
                {
                    if (transparentObject != null && transparentObject != hit.transform.gameObject)
                        ChangeColor(transparentObject, 1);

                    transparentObject = hit.transform.gameObject;
                }
                else
                {
                    if (transparentObject != null)
                        ChangeColor(transparentObject, 1f);
                }
            }
            else
            {
                if (transparentObject != null)
                    ChangeColor(transparentObject, 1f);
            }
        }
        else
        {
            if (transparentObject != null)
                ChangeColor(transparentObject, 1f);
        }
    }

    private bool ChangeColor(GameObject gameObject, float alpha)
    {
        Renderer prevRenderer = gameObject.GetComponent<Renderer>();
        if (prevRenderer != null)
        {
            prevRenderer.material.color = new Color(prevRenderer.material.color.r, prevRenderer.material.color.g, prevRenderer.material.color.b, alpha);
            return true;
        }

        return false;
    }

    #region Movement
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

    private void UpdateVelocity()
    {
        velocity = velocity * 0.8f;
        if (velocity.x < 0.1 && velocity.x > -0.1f && velocity.y < 0.1 && velocity.y > -0.1f && velocity.z < 0.1 && velocity.z > -0.1f)
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
    #endregion
    #endregion

    #region Abilities
    private bool TryHeal()
    {
        if (healthContainer.Health < healthContainer.MaxHealth)
        {
            return TakeTeamHealth(regenerationPerSecond * Time.deltaTime, HealthRegenerationMultiplicator);
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

    private void CheckLifeSteal(Ability ability, float damage)
    {
        if (ability.IsActive && ability.name.Contains("LifeSteal"))
        {
            healthContainer.Heal(damage * ability.abilityValue);
        }
    }

    private Vector3 ScaleVactorUp(Vector3 vec)
    {
        float x = Math.Abs(vec.x);
        float y = Math.Abs(vec.y);
        float z = Math.Abs(vec.z);
        float diff = 0f;
        if (x > y && x > z)
            diff = 1 - x;
        else if (y > z)
            diff = 1 - y;
        else
            diff = 1 - z;

        if (x != 0)
            x = vec.x + (vec.x > 0 ? diff : -diff);
        if (y != 0)
            y = vec.y + (vec.y > 0 ? diff : -diff);
        if (z != 0)
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
    
    private bool TrySecondaryAbility()
    {
        if (secondaryAbility != null)
        {
            return secondaryAbility.Use();
        }
        return false;
    }
    #endregion

    #region AbilityEvents
    private void SecondaryAbility_OnAbort(object sender, EventArgs e)
    {
        Ability ability = (Ability)sender;
    }

    private void SecondaryAbility_OnActivated(object sender, EventArgs e)
    {
        Ability ability = (Ability)sender;
    }

    private void Ability_OnAbort(object sender, EventArgs e)
    {
        Ability ability = (Ability)sender;
    }

    private void Ability_OnActivated(object sender, EventArgs e)
    {
        Ability ability = (Ability)sender;

    }
    #endregion

    #region WeaponEvents
    private void SecondaryWeapon_OnSecondaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
    }

    private void SecondaryWeapon_OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
    }

    private void PrimaryWeapon_OnSecondaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
    }

    private void PrimaryWeapon_OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
    }

    private void ProjectileScript_OnHit(object sender, HitEventArgs e)
    {
        CheckLifeSteal(ability, e.FinalDamage);
        CheckLifeSteal(secondaryAbility, e.FinalDamage);
    }
    #endregion

    #region PlayerHealth
    private bool TakeTeamHealth(float addHealth, float teamHealthMultiplicator)
    {
        if(healthContainer.Health + addHealth > healthContainer.MaxHealth)
        {
            addHealth = healthContainer.MaxHealth - healthContainer.Health;
        }

        float subHealth = addHealth * teamHealthMultiplicator;
        if (TeamHealth < subHealth)
        {
            addHealth = TeamHealth / teamHealthMultiplicator;
            subHealth = addHealth * teamHealthMultiplicator;
        }

        if (addHealth > 0)
        {
            TeamHealth -= subHealth;
            healthContainer.Heal(addHealth);
            return true;
        }
        return false;
    }

    private void HealthContainer_OnDeath(object sender, EventArgs e)
    {
        isDead = true;
        if (TeamHealth == 0)
        {
            Destroy(gameObject);
        }
    }

    private void RevivePlayer()
    {
        if (elapsedReviveDelay >= reviveDelay)
        {
            if (!TakeTeamHealth(healthContainer.MaxHealth, HealthRegenerationMulitplicatorOnDeath))
            {
                Destroy(gameObject);
            }
            else
            {
                elapsedReviveDelay = 0f;
                isDead = false;
            }
        }
    }
    
    private void HealthContainer_OnReceiveHealth(object sender, OnHealthChangedArgs e)
    {
        //Stun, slow, gift
    }

    private void HealthContainer_OnReceiveDamage(object sender, OnHealthChangedArgs e)
    {

    }
    #endregion

    public void InputOnIce()
    {
        physics.AddForce(finalVelocity);
    }

    public void PutOnIce()
    {
        OnIce = true;
        moveScript.MovementMultiplicator = 0f;
    }
    public void PutOffIce()
    {
        OnIce = false;
        moveScript.ResetMultiplicator();
    }
}

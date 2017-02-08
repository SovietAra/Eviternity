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
    public static Vector3 LastCheckpointPosition;
    #endregion

    #region privats

    private AudioSource[] audioSources;
    private PlayerIndex index;
    private bool hasPlayerIndex;
    private bool isDead;
    private Vector3 moveVector;
    private Vector3 velocity;
    private Vector3 finalVelocity;
    
    private float elapsedDashRegenerationTime = 0f;
    private float elapsedReviveDelay = 0f;
    private float elapsedImmortal;
    private float attackInProgressTimer = 0f;

    private float angle;
    private Quaternion targetRotation;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    private Weapon grenadeWeapon;
    private Ability ability;
    private Ability secondaryAbility;
    private Ability dashAbility;
    private DamageAbleObject healthContainer;
    private MoveScript moveScript;
    private ParticleSystem[] dashParticles;
    private GameObject transparentObject = null;
    private GameObject leftSpawn, rightSpawn;

    private Camera mainCamera;
    private float xMin, xMax, zMin, zMax;
    private Rigidbody physics;
    
    private GamePadState prevState;

    private GameObject mainGameObject;
    private UIScript uiScript;

    public Animator healingAnim;
    public Animator animator;

    private Vector3 meshBounds;

    private float stepTimer;
    private AudioSource deniedSource;
    

    private bool isReloading;

    // private bool[] PlayMusicTheme = new bool[50];
    #endregion

    #region InspectorFields

    [SerializeField]
    private GameObject dashTrail = null;

    [SerializeField]
    [Range(1f, 100f)]
    private float speed = 1f;
    
    [SerializeField]
    [Range(1, 500)]
    private float regenerationPerSecond = 5f;

    [SerializeField]
    [Range(0.1f, 30f)]
    private float reviveDelay = 2f;

    [SerializeField]
    [Range(0.1f, 5)]
    private float maxImmortality = 2f;

    public bool Freeze = false;
    public bool RotateOnMove = false;
    public GameObject PrimaryWeapon;
    public GameObject SecondaryWeapon;
    public GameObject GrenadeWeapon;
    public GameObject Ability;
    public GameObject SecondaryAbility;
    public GameObject DashAbility;
    public AudioClip HealDeniedSound;
    [HideInInspector]
    public bool OnIce;
    

    public AudioClip[] AudioClips = new AudioClip[50];

    [SerializeField]
    private float StepVolume = 0.4f;

    [SerializeField]
    private float stepCooldown = 0.5f;
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

    public bool HasPlayerIndex
    {
        get
        {
            return hasPlayerIndex;
        }
    }
    #endregion

    #region UnityMethodes

    // Use this for initialization
    private void Start()
    {
        foreach (Transform item in transform)
        {
            if(item.CompareTag("ProjectileSpawn"))
            {
                if(item.name == "Left")
                {
                    leftSpawn = item.gameObject;
                }
                else if(item.name == "Right")
                {
                    rightSpawn = item.gameObject;
                }
            }       
        }
        //create as many audiosources as we want  = needed for playing as many sounds simultaniously as we want
        for (var tmp = 0; tmp < AudioClips.Length; tmp++)
        {
            gameObject.AddComponent<AudioSource>();
        }

        audioSources = GetComponents<AudioSource>();

        for (var tmp = 0; tmp < AudioClips.Length; tmp++)
        {
            audioSources[tmp].clip = AudioClips[tmp];
        }

        deniedSource = gameObject.AddComponent<AudioSource>();
        //define names for sounds
        var Spawn_Sound = audioSources[0];

        //var Despawn_Sound = audioSources[1];
        //var Walk_Ice_1_Sound = audioSources[3];   3-7 Ice_Walk
        //var Walk_Ice_2_Sound = audioSources[4];   8-12 Metal_Walk
        //var Walk_Ice_3_Sound = audioSources[5];   13-17 Oil_Walk
        //var Walk_Ice_4_Sound = audioSources[6];   18-22 Snow_Walk
        //var Walk_Ice_5_Sound = audioSources[7]; 
        //var Hit1_Sound = audioSources[7];         23-26 Hit
        
        //for(int i = 3; i < 23; i++)
        //{
        //    audioSources[i].volume = StepVolume;
        //}
        

        //TODO call sounds in correct places/functions

        //play sound by its name defined above
        Spawn_Sound.Play();

        mainCamera = Camera.main;
        mainCamera.GetComponentInParent<FollowingCamera>().AddToCamera(transform);
        
        mainGameObject = GameObject.FindGameObjectWithTag("GameObject");
        uiScript = mainGameObject.GetComponent<UIScript>();

        if (DashAbility != null)
        {
            GameObject dash = Instantiate(DashAbility, gameObject.transform);
            if (dash != null)
            {
                dashAbility = dash.GetComponent<Ability>();
                dashAbility.OnActivated += DashAbility_OnActivated;
                dashAbility.OnAbort += DashAbility_OnAbort;
                dashAbility.OnUsing += DashAbility_OnUsing;
            }
        }          

        physics = GetComponent<Rigidbody>();
        if (PrimaryWeapon != null)
        {
            primaryWeapon = Instantiate(PrimaryWeapon, transform).GetComponent<Weapon>();
            primaryWeapon.OnPrimaryAttack += PrimaryWeapon_OnPrimaryAttack;
            primaryWeapon.OnSecondaryAttack += PrimaryWeapon_OnSecondaryAttack;
            primaryWeapon.OnDelayedPrimaryAttack += PrimaryWeapon_OnDelayedPrimaryAttack;
            primaryWeapon.OnDelayedSecondaryAttack += PrimaryWeapon_OnDelayedSecondaryAttack;
            primaryWeapon.OnSpawning += PrimaryWeapon_OnSpawning;
        }
        if (SecondaryWeapon != null)
        {
            secondaryWeapon = Instantiate(SecondaryWeapon, transform).GetComponent<Weapon>();
            secondaryWeapon.OnPrimaryAttack += SecondaryWeapon_OnPrimaryAttack;
            secondaryWeapon.OnSecondaryAttack += SecondaryWeapon_OnSecondaryAttack;
            secondaryWeapon.OnDelayedPrimaryAttack += SecondaryWeapon_OnDelayedPrimaryAttack;
            secondaryWeapon.OnDelayedSecondaryAttack += SecondaryWeapon_OnDelayedSecondaryAttack;
            secondaryWeapon.OnSpawning += SecondaryWeapon_OnSpawning;
        }
        if(GrenadeWeapon != null)
        {
            grenadeWeapon = Instantiate(GrenadeWeapon, transform).GetComponent<Weapon>();
            grenadeWeapon.OnPrimaryAttack += GrenadeWeapon_OnPrimaryAttack;
        }

        if (Ability != null)
        {
            ability = Instantiate(Ability, transform).GetComponent<Ability>();
            if(ability.name.Contains("Dash"))
            {
                ability.OnActivated += DashAbility_OnActivated;
                ability.OnAbort += DashAbility_OnAbort;
                ability.OnUsing += DashAbility_OnUsing;
            }
        }
        if (SecondaryAbility != null)
        {
            secondaryAbility = Instantiate(SecondaryAbility, transform).GetComponent<Ability>();
            if (secondaryAbility.name.Contains("Dash"))
            {
                secondaryAbility.OnActivated += DashAbility_OnActivated;
                secondaryAbility.OnAbort += DashAbility_OnAbort;
                secondaryAbility.OnUsing += DashAbility_OnUsing;
            }
        }

        healthContainer = GetComponent<DamageAbleObject>();
        if (healthContainer != null)
        {
            healthContainer.OnDeath += HealthContainer_OnDeath;
            healthContainer.OnReceiveDamage += HealthContainer_OnReceiveDamage;
            healthContainer.OnReceiveHealth += HealthContainer_OnReceiveHealth;
        }

        moveScript = GetComponent<MoveScript>();
        if (moveScript != null)
            moveScript.OnMoving += MoveScript_OnMoving;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            if (mesh != null)
            {
                meshBounds = mesh.bounds.size;
            }
        }

        //Set Immortality Time Value
        elapsedImmortal = maxImmortality;
        if (dashTrail != null)
        {
            dashParticles = dashTrail.GetComponentsInChildren<ParticleSystem>();
            SetDashParticles(false);
        }
    }

    private void SecondaryWeapon_OnSpawning(object sender, SpawnEventArgs e)
    {
        e.Angle = angle;
        e.SpawnPosition = rightSpawn.transform.position;
        e.Forward = transform.forward;
    }

    private void PrimaryWeapon_OnSpawning(object sender, SpawnEventArgs e)
    {
        e.Angle = angle;
        e.SpawnPosition = leftSpawn.transform.position;
        e.Forward = transform.forward;
    }

    private void MoveScript_OnMoving(object sender, OnMovingArgs e)
    { 
        e.Cancel = OnIce;
        if(!OnIce && e.Velocity != Physics.gravity)
        {
            if(stepTimer >= stepCooldown)
            {
                audioSources[UnityEngine.Random.Range(3,17)].Play();
                stepTimer = 0;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!hasPlayerIndex)
        {
            PlayerIndex[] freeControllers = GamePadManager.GetFreeControllers();
            for (int i = 0; i < freeControllers.Length; i++)
            {
                if (freeControllers[i] != index)
                {
                    for (int j = 0; j < GlobalReferences.PlayerStates.Count; j++)
                    {
                        if (GlobalReferences.PlayerStates[i].Index == freeControllers[i])
                            return;
                    }

                    for (int l = 0; l < GlobalReferences.PlayerStates.Count; l++)
                    {
                        if(GlobalReferences.PlayerStates[l].Index == index)
                        {
                            index = freeControllers[i];
                            GlobalReferences.PlayerStates[l] = new PlayerState(index, GlobalReferences.PlayerStates[i]);
                            hasPlayerIndex = true;
                            GamePadManager.Connect((int)index);
                            uiScript.CreateUI(index, uiScript.UICanvas.transform);
                            return;
                        }
                    }
                }
                else
                {
                    hasPlayerIndex = true;
                    GamePadManager.Connect((int)index);
                    uiScript.CreateUI(index, uiScript.UICanvas.transform);

                }
            }
        }

        if (hasPlayerIndex)
        {
            GamePadState state = GamePad.GetState(Index);
            if (state.IsConnected)
            {
                if (elapsedImmortal < maxImmortality)
                    healthContainer.isImmortal = true;
                else
                    healthContainer.isImmortal = false;

                UpdateTimers();
                if (isDead)
                {
                    RevivePlayer();
                }
                else
                {
                    if (!Freeze)
                    {
                        Input(state);
                        UpdateRotation();
                    }
                }
                prevState = state;
            }
            else
            { 
                uiScript.RemoveUI(index);
                hasPlayerIndex = false;
                GamePadManager.Disconnect(Index);
                GlobalReferences.CurrentGameState = GlobalReferences.GameState.ConnectionLost;
            }
        }
        
        stepTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (OnIce)
        {
            InputOnIce();
        }

        //for (var i = 45; i < 50; i++)//tracks from 45 to 49 are music themes, always.
        //{       
        //    if (!audioSources[i].isPlaying && PlayMusicTheme[i]) 
        //        audioSources[i].Play(); 

        //    if (audioSources[i].isPlaying && !PlayMusicTheme[i])
        //        audioSources[i].Stop();
        //}
       
        CheckOverlappingObjects();
    }
    #endregion
    
    #region UpdateMethodes

    private void UpdateTimers()
    {
        elapsedImmortal += Time.deltaTime;
        elapsedDashRegenerationTime += Time.deltaTime;
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

        bool executed = false;
        if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released)
        {
            GlobalReferences.CurrentGameState = GlobalReferences.GameState.Pause;
        }

        if (state.Buttons.Back == ButtonState.Pressed)
        {
            if (OnPlayerExit != null)
                OnPlayerExit(this, new PlayerEventArgs(gameObject, this));
        }

        if (state.Buttons.Y == ButtonState.Pressed)
        {
            executed = TryHeal();
        }
        else
            healingAnim.SetBool("heal", false);

        if (state.Buttons.Y == ButtonState.Released)
        {
            if (audioSources[23].isPlaying)
                audioSources[23].Stop(); //stop healing sound
        }

        if (state.Buttons.B == ButtonState.Pressed && !executed)
        {
            executed = TrySecondaryAbility();
        }

        if (state.Buttons.X == ButtonState.Pressed && !executed)
        {
            executed = TryAbillity();
        }

        if (((state.Buttons.LeftStick == ButtonState.Pressed || (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released))) && !executed)
        {
            executed = TryDash();
        }

        #region AnimationHandler

        if (!(state.Triggers.Left > 0 && !executed) || attackInProgressTimer > 0)
        {
            animator.SetBool("RightAttack", false);
        }

        if (!(state.Triggers.Right > 0 && !executed) || attackInProgressTimer > 0)
        {
            animator.SetBool("LeftAttack", false);
        }

        if (!(state.Buttons.LeftShoulder == ButtonState.Pressed && !executed) || attackInProgressTimer > 0)
        {
            animator.SetBool("RightAttack2", false);
        }

        if (!(state.Buttons.RightShoulder == ButtonState.Pressed && !executed) || attackInProgressTimer > 0)
        {
            animator.SetBool("LeftAttack2", false);
        }
        
        #endregion

        if (attackInProgressTimer <= 0)
        {
            if (state.Triggers.Left > 0 && !executed)
            {
                if (primaryWeapon != null)
                    executed = primaryWeapon.PrimaryAttack(leftSpawn.transform.position, transform.forward, angle);
            }

            if (state.Triggers.Right > 0 && !executed)
            {
                if (secondaryWeapon != null)
                    executed = secondaryWeapon.PrimaryAttack(rightSpawn.transform.position, transform.forward, angle);
            }

            if (state.Buttons.LeftShoulder == ButtonState.Pressed && !executed)
            {
                if (primaryWeapon != null)
                    executed = primaryWeapon.SecondaryAttack(transform.position, transform.forward, angle);
            }

            if (state.Buttons.RightShoulder == ButtonState.Pressed && !executed)
            {
                if (secondaryWeapon != null)
                    executed = secondaryWeapon.SecondaryAttack(transform.position, transform.forward, angle);
            }

            if(state.Buttons.RightStick == ButtonState.Pressed && !executed)
            {
                if (grenadeWeapon != null)
                    executed = grenadeWeapon.PrimaryAttack(transform.position - (transform.forward / 1.25f), transform.forward, angle);
            }
        }

        finalVelocity = (moveVector * 100) + velocity;
        if (moveScript != null && !OnIce)
        {
            Borders();
            moveScript.Move(finalVelocity);
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

        if (transform.position.x < xMin + meshBounds.x && finalVelocity.x < 0)
            finalVelocity.x = 0;

        if (transform.position.x > xMax - meshBounds.x && finalVelocity.x > 0)
            finalVelocity.x = 0;

        if (transform.position.z < zMin + meshBounds.z && finalVelocity.z < 0)
            finalVelocity.z = 0;

        if (transform.position.z > zMax - meshBounds.z && finalVelocity.z > 0)
            finalVelocity.z = 0;
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
                if(ChangeColor(hit.transform.gameObject, 0.1f))
                    transparentObject = hit.transform.gameObject;
            }
            else
            {
                ChangeColor(transparentObject, 1f);
            }
        }
        else
        {
            ChangeColor(transparentObject, 1f);
        }
    }

    private bool ChangeColor(GameObject gameObject, float alpha)
    {
        if (gameObject != null)
        {
            Renderer prevRenderer = gameObject.GetComponent<Renderer>();
            if (prevRenderer != null)
            {
                prevRenderer.material.color = new Color(prevRenderer.material.color.r, prevRenderer.material.color.g, prevRenderer.material.color.b, alpha);
                return true;
            }
        }

        return false;
    }

    #region Movement

    private void TryMove(Vector2 leftStick, Vector2 rightStick)
    {
        moveVector = Vector3.zero;
        if (!Freeze)
        {
            animator.SetBool("Walking", false);
            if (leftStick != Vector2.zero)
                animator.SetBool("Walking", true);

            if (leftStick.magnitude > 0.25f)
            {//if (leftStick.y > 0.1f || leftStick.y < 0.1f)
                moveVector = Vector3.forward * leftStick.y * Time.deltaTime * speed;

             //if (leftStick.x > 0.1f || leftStick.x < 0.1f)
                    moveVector += Vector3.right * leftStick.x * Time.deltaTime * speed;

                if (RotateOnMove && moveVector != Vector3.zero)
                {
                    float leftAngle = MathUtil.FixAngle(MathUtil.CalculateAngle(new Vector2(leftStick.x * -1, leftStick.y), Vector2.zero) - 90);
                    if (leftStick != Vector2.zero)
                    {
                        DoRotation(leftAngle);
                        animator.SetInteger("WalkAnim", 0);
                    }
                }
            }

            if (rightStick.magnitude > 0.25f)
            {
                float rightAngle = MathUtil.FixAngle(MathUtil.CalculateAngle(new Vector2(rightStick.x * -1, rightStick.y), Vector2.zero) - 90);
                if (rightStick != Vector2.zero)
                {
                    DoRotation(rightAngle);
                    FindWalkAnimation(leftStick, rightStick);
                }
            }
        }
    }

    private void FindWalkAnimation(Vector2 leftStick, Vector2 rightStick)
    {
        float ang = Vector2.Angle(leftStick, rightStick);
        Vector3 cross = Vector3.Cross(leftStick, rightStick);

        if (cross.z > 0)
            ang = 360 - ang;

        if (ang < 45 || ang > 315)
            animator.SetInteger("WalkAnim", 0);
        if (ang > 45 && ang < 135)
            animator.SetInteger("WalkAnim", 2);
        if (ang > 135 && ang < 225)
            animator.SetInteger("WalkAnim", 3);
        if (ang > 225 && ang < 315)
            animator.SetInteger("WalkAnim", 1);
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
    #endregion Movement

    #endregion UpdateMethodes

    #region Abilities

    private bool TryHeal()
    {
        if(Player.TeamHealth <= 0)
        {
            if(deniedSource != null && HealDeniedSound != null)
            {
                deniedSource.clip = HealDeniedSound;
                deniedSource.Play();
            }
        }

        if (healthContainer.Health < healthContainer.MaxHealth)
        {
            uiScript.ActivateTeamBar();

            if (TakeTeamHealth(regenerationPerSecond * Time.deltaTime, HealthRegenerationMultiplicator))
            {
                if(!isDead)
                    healingAnim.SetBool("heal", true);
                if (!audioSources[23].isPlaying)
                    audioSources[23].Play();
                return true;
            }
            else
                healingAnim.SetBool("heal", false);
        }
        return false;
    }

    private bool TryDash()
    {
        if (dashAbility != null)
        {
            if(dashAbility.Use())
            {
                return true;
            }
        }
        return false;
    }

    private void CheckLifeSteal(Ability ability, float damage)
    {
        if (ability.IsActive && ability.name.Contains("LifeSteal"))
        {
            healthContainer.Heal(null, damage * ability.AbilityValue);
        }
    }
    
    private bool TryAbillity()
    {
        if (ability != null)
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

    #endregion Abilities

    #region AbilityEvents
    private void DashAbility_OnAbort(object sender, EventArgs e)
    {
        velocity = Vector3.zero;
        SetDashParticles(false);
    }

    private void DashAbility_OnActivated(object sender, EventArgs e)
    {
        animator.SetTrigger("Dash");
        if (!audioSources[2].isPlaying)
            audioSources[2].Play();

        SetDashParticles(true);
        Ability abil = sender as Ability;
        if (abil != null)
        {
            if (moveVector != Vector3.zero)
            {
                velocity = (moveVector * 2) * abil.AbilityValue;
            }
            else
            {
                velocity = transform.forward * abil.AbilityValue;
            }
        }
    }

    private void DashAbility_OnUsing(object sender, EventArgs e)
    {
        velocity -= velocity * (Time.deltaTime * 0.1f);
    }
    #endregion

    #region WeaponEvents

    private void SecondaryWeapon_OnSecondaryAttack(object sender, WeaponEventArgs e)
    {
        if (!e.Delayed)
        {
            animator.SetBool("LeftAttack2", true);
            attackInProgressTimer += e.AnimationDuration;
        }
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
        if ((finalVelocity.x != 0 || finalVelocity.z != 0)
            && ((finalVelocity.x <= 0 && transform.forward.x <= 0) || (finalVelocity.x >= 0 && transform.forward.x >= 0) || (transform.forward.x == finalVelocity.x))
            && ((finalVelocity.z <= 0 && transform.forward.z <= 0) || (finalVelocity.z >= 0 && transform.forward.z >= 0) || (finalVelocity.z == transform.forward.z)))
        {
            e.ProjectileScript.IncreaseVelocity(transform.forward * speed);
        }
    }

    private void SecondaryWeapon_OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        if (!e.Delayed)
        {
            animator.SetBool("LeftAttack", true);
            attackInProgressTimer += e.AnimationDuration;
        }
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
        if ((finalVelocity.x != 0 || finalVelocity.z != 0)
            && ((finalVelocity.x <= 0 && transform.forward.x <= 0) || (finalVelocity.x >= 0 && transform.forward.x >= 0) || (transform.forward.x == finalVelocity.x))
            && ((finalVelocity.z <= 0 && transform.forward.z <= 0) || (finalVelocity.z >= 0 && transform.forward.z >= 0) || (finalVelocity.z == transform.forward.z)))
        {
            e.ProjectileScript.IncreaseVelocity(transform.forward * speed);
        }
    }

    private void PrimaryWeapon_OnSecondaryAttack(object sender, WeaponEventArgs e)
    {
        if (!e.Delayed)
        {
            animator.SetBool("RightAttack2", true);
            attackInProgressTimer += e.AnimationDuration;
        }
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;

        if ((finalVelocity.x != 0 || finalVelocity.z != 0)
            && ((finalVelocity.x <= 0 && transform.forward.x <= 0) || (finalVelocity.x >= 0 && transform.forward.x >= 0) || (transform.forward.x == finalVelocity.x))
            && ((finalVelocity.z <= 0 && transform.forward.z <= 0) || (finalVelocity.z >= 0 && transform.forward.z >= 0) || (finalVelocity.z == transform.forward.z)))
        {
            e.ProjectileScript.IncreaseVelocity(transform.forward * speed);
        }
    }

    private void PrimaryWeapon_OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        if (!e.Delayed)
        {
            animator.SetBool("RightAttack", true);
            attackInProgressTimer += e.AnimationDuration;
        }
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
        

        if ((finalVelocity.x != 0 || finalVelocity.z != 0) 
            && ((finalVelocity.x <= 0 && transform.forward.x <= 0) || (finalVelocity.x >= 0 && transform.forward.x >= 0) || (transform.forward.x == finalVelocity.x)) 
            && ((finalVelocity.z <= 0 && transform.forward.z <= 0) || (finalVelocity.z >= 0 && transform.forward.z >= 0) || (finalVelocity.z == transform.forward.z)))
        {
            e.ProjectileScript.IncreaseVelocity(transform.forward * speed);
        }
    }

    private void GrenadeWeapon_OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;

        if ((finalVelocity.x != 0 || finalVelocity.z != 0)
            && ((finalVelocity.x <= 0 && transform.forward.x <= 0) || (finalVelocity.x >= 0 && transform.forward.x >= 0) || (transform.forward.x == finalVelocity.x))
            && ((finalVelocity.z <= 0 && transform.forward.z <= 0) || (finalVelocity.z >= 0 && transform.forward.z >= 0) || (finalVelocity.z == transform.forward.z)))
        {
            e.ProjectileScript.IncreaseVelocity(transform.forward * speed);
        }
    }

    private void SecondaryWeapon_OnDelayedSecondaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        animator.SetBool("LeftAttack2", true);
    }

    private void SecondaryWeapon_OnDelayedPrimaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        animator.SetBool("LeftAttack", true);
    }

    private void PrimaryWeapon_OnDelayedSecondaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        animator.SetBool("RightAttack2", true);
    }

    private void PrimaryWeapon_OnDelayedPrimaryAttack(object sender, WeaponEventArgs e)
    {
        attackInProgressTimer += e.AnimationDuration;
        animator.SetBool("RightAttack", true);
    }

    private void ProjectileScript_OnHit(object sender, HitEventArgs e)
    {
        CheckLifeSteal(ability, e.FinalDamage);
        CheckLifeSteal(secondaryAbility, e.FinalDamage);
    }

    #endregion WeaponEvents

    #region PlayerHealth

    private bool TakeTeamHealth(float addHealth, float teamHealthMultiplicator)
    {
        if (healthContainer.Health + addHealth > healthContainer.MaxHealth)
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
            healthContainer.Heal(null, addHealth);
            return true;
        }
        return false;
    }

    private void HealthContainer_OnDeath(object sender, EventArgs e)
    {
        isDead = true;
        healingAnim.SetBool("heal", false);
        SetDashParticles(false);
        if (!animator.GetBool("IsDead"))
        {
            animator.SetBool("IsDead", true);
            animator.SetTrigger("OnDeath");
        }
        if (TeamHealth == 0 || GlobalReferences.PlayerStates.Count <= 1)
        {
            if (!audioSources[1].isPlaying)
                audioSources[1].Play();
            Destroy(gameObject);
            uiScript.OnExit(index);
        }
    }

    private void RevivePlayer()
    {   
        if (elapsedReviveDelay >= reviveDelay)
        {
            if (!TakeTeamHealth(healthContainer.MaxHealth, HealthRegenerationMulitplicatorOnDeath))
            {
                GamePadManager.Disconnect(index);
                Destroy(gameObject);
            }
            else
            {
                elapsedReviveDelay = 0f;
                isDead = false;
                animator.SetBool("IsDead", false);
                elapsedImmortal = 0;
            }
        }
    }

    private void HealthContainer_OnReceiveHealth(object sender, OnHealthChangedArgs e)
    {
        //Stun, slow, gift
    }

    private void HealthContainer_OnReceiveDamage(object sender, OnHealthChangedArgs e)
    {
        //if (!audioSources[23].isPlaying && !audioSources[24].isPlaying && !audioSources[25].isPlaying && !audioSources[26].isPlaying)
        //{
        //    System.Random rand = new System.Random();
        //        audioSources[rand.Next(23,27)].Play();
        //}
    }

    #endregion PlayerHealth

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

    public bool GetAmmo()
    {
        isReloading = secondaryWeapon.Reloading;
        return isReloading;
    }

    public float GetHeat(int wpnNumber)
    {
        float heat = 0;
        switch (wpnNumber)
        {
            case 1:
                {
                    if (primaryWeapon != null)
                        heat = primaryWeapon.Heat;
                    else heat = 0;
                }
                break;
            case 2:
                {
                    if (secondaryWeapon != null)
                        heat = secondaryWeapon.Heat;
                    else heat = 0;
                }
                break;
            case 3:
                {
                    if (grenadeWeapon != null)
                        heat = grenadeWeapon.Heat;
                    else heat = 0;
                }
                break;
            default:
                break;
        }
        return heat;
    }

    public float GetMaxHeat(int wpnNumber)
    {
        float heat = 0;
        switch (wpnNumber)
        {
            case 1:
                {
                    if (primaryWeapon != null)
                        heat = primaryWeapon.MaxHeat;
                    else heat = 0;
                }
                break;
            case 2:
                {
                    if (secondaryWeapon != null)
                        heat = secondaryWeapon.MaxHeat;
                    else heat = 0;
                }
                break;
            case 3:
                {
                    if (grenadeWeapon != null)
                        heat = grenadeWeapon.MaxHeat;
                    else heat = 0;
                }
                break;
            default:
                break;
        }
        return heat;
    }

    public float AbilityEnergy(int abilityNumber)
    {
        float energyLevel = 0;
        switch (abilityNumber)
        {
            case 1:
                {
                    if (ability != null)
                        energyLevel = ability.Energy;
                    else energyLevel = 0;
                }
                break;
            case 2:
                {
                    if (dashAbility != null)
                        energyLevel = dashAbility.Energy;
                    else energyLevel = 0;
                }
                break;
            case 3:
                {
                    if (secondaryAbility != null)
                        energyLevel = secondaryAbility.Energy;
                    else energyLevel = 0;
                }
                break;
            default: energyLevel = 0;
                break;
        }

        return energyLevel;
    }

    public float MaxEnergy(int abilityNumber)
    {
        float maxEnergyLevel;
        switch (abilityNumber)
        {
            case 1:
                {
                    if (ability != null)
                        maxEnergyLevel = ability.MaxEnergy;
                    else maxEnergyLevel = 0;
                }
                break;
            case 2:
                {
                    if (dashAbility != null)
                        maxEnergyLevel = dashAbility.MaxEnergy;
                    else maxEnergyLevel = 0;
                }
                break;
            case 3:
                {
                    if (secondaryAbility != null)
                        maxEnergyLevel = secondaryAbility.MaxEnergy;
                    else maxEnergyLevel = 0;
                }
                break;
            default:
                maxEnergyLevel = 0;
                break;
        }
        return maxEnergyLevel;
    }

    //public void PlayMusicThemePicker(int tmp, bool state)
    //{
    //    PlayMusicTheme[tmp] = state;
    //}

    private void SetDashParticles(bool active)
    {
        if (dashParticles != null)
        {
            foreach (ParticleSystem particles in dashParticles)
            {
                if (active)
                    particles.Play();
                else
                    particles.Stop();
            }
        }
    }
}
using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.Assertions.Must;
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
    private Ability ability;
    private Ability secondaryAbility;
    private Ability dashAbility;
    private DamageAbleObject healthContainer;
    private MoveScript moveScript;

    private Camera mainCamera;
    private float xMin, xMax, zMin, zMax;
    private Rigidbody physics;
    private GameObject transparentObject;

    private GamePadState prevState;

    private GameObject mainGameObject;
    private UIScript uiScript;

    private Vector3 meshBounds;

    private float stepTimer;

    private bool[] PlayMusicTheme = new bool[50];
    #endregion

    #region InspectorFields

    [SerializeField]
    [Range(1f, 100f)]
    private float speed = 1f;
    
    [SerializeField]
    [Range(1, 100)]
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
    public GameObject Ability;
    public GameObject SecondaryAbility;
    public GameObject DashAbility;

    [HideInInspector]
    public bool OnIce;
    

    public AudioClip[] AudioClips = new AudioClip[50];

    [SerializeField]
    float ShotVolume;

    [SerializeField]
    float StepVolume;

    [SerializeField]
    float stepCooldown;
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
    private void Start()
    {
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

        //define names for sounds
        var Spawn_Sound = audioSources[0];
        var Despawn_Sound = audioSources[1];
        var Dash_Sound = audioSources[2];
        //var Walk_Ice_1_Sound = audioSources[3];   3-7 Ice_Walk
        //var Walk_Ice_2_Sound = audioSources[4];   8-12 Metal_Walk
        //var Walk_Ice_3_Sound = audioSources[5];   13-17 Oil_Walk
        //var Walk_Ice_4_Sound = audioSources[6];   18-22 Snow_Walk
        //var Walk_Ice_5_Sound = audioSources[7]; 
        //var Hit1_Sound = audioSources[7];         23-26 Hit

        audioSources[10].volume = ShotVolume;
        for(int i = 3; i < 23; i++)
        {
            audioSources[i].volume = StepVolume;
        }
        

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
    }

    private void MoveScript_OnMoving(object sender, OnMovingArgs e)
    {
        e.Cancel = OnIce;
        if(!OnIce && e.Velocity != Physics.gravity)
        {
            if(stepTimer >= stepCooldown)
            {
                System.Random rand = new System.Random();
                audioSources[rand.Next(3,23)].Play();
                stepTimer = 0;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
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
                        // UpdateVelocity();
                        UpdateRotation();
                    }
                }
                prevState = state;
            }
            else
            {
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

        for (var i = 45; i < 50; i++)//tracks from 45 to 49 are music themes, always.
        {
         
            if (!audioSources[i].isPlaying && PlayMusicTheme[i]) 

                audioSources[i].Play(); 

            if (audioSources[i].isPlaying && !PlayMusicTheme[i])

                audioSources[i].Stop();



        }
        



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

        finalVelocity = (moveVector * 100) + velocity;
        if(moveScript !=null && !OnIce)
        {
            Borders();
            moveScript.Move(finalVelocity);          
        }

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

        if (state.Buttons.Y == ButtonState.Released)
        {
            if (audioSources[28].isPlaying)
                audioSources[28].Stop(); //stop healing sound
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

    bool preVis = false;
    Vector3 lastVec = Vector3.zero;
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

        /*Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        if(!GeometryUtility.TestPlanesAABB(planes, new Bounds(transform.position + (Vector3.right * meshBounds.x), new Vector3(1, 1, 1))))
        {
            if (finalVelocity.x > 0)
                finalVelocity.x = 0;
        }
        else if (!GeometryUtility.TestPlanesAABB(planes, new Bounds(transform.position + (Vector3.left * meshBounds.x), new Vector3(1, 1, 1))))
        {
            if (finalVelocity.x < 0)
                finalVelocity.x = 0;
        }

        if (!GeometryUtility.TestPlanesAABB(planes, new Bounds(transform.position + (Vector3.forward * meshBounds.z), new Vector3(1, 1, 1))))
        {
            if (finalVelocity.z > 0)
                finalVelocity.z = 0;
        }
        else if (!GeometryUtility.TestPlanesAABB(planes, new Bounds(transform.position + (Vector3.back * meshBounds.z), new Vector3(1, 1, 1))))
        {
            if (finalVelocity.z < 0)
                finalVelocity.z = 0;
        }*/

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
            if (leftStick.y > 0.1f || leftStick.y < 0.1f)
                moveVector = Vector3.forward * leftStick.y * Time.deltaTime * speed;

            if (leftStick.x > 0.1f || leftStick.x < 0.1f)
                moveVector += Vector3.right * leftStick.x * Time.deltaTime * speed;

            if (RotateOnMove && moveVector != Vector3.zero)
            {
                float leftAngle = MathUtil.FixAngle(MathUtil.CalculateAngle(new Vector2(leftStick.x * -1, leftStick.y), Vector2.zero) - 90);
                if (leftStick != Vector2.zero)
                {
                    DoRotation(leftAngle);
                }
            }

            float rightAngle = MathUtil.FixAngle(MathUtil.CalculateAngle(new Vector2(rightStick.x * -1, rightStick.y), Vector2.zero) - 90);
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
    #endregion Movement

    #endregion UpdateMethodes

    #region Abilities

    private bool TryHeal()
    {
        if (healthContainer.Health < healthContainer.MaxHealth)
        {
            uiScript.ActivateTeamBar();
            
            if (TakeTeamHealth(regenerationPerSecond * Time.deltaTime, HealthRegenerationMultiplicator))
            {
                if(!audioSources[28].isPlaying)
                    audioSources[28].Play();
                return true;
            }
        }
        return false;
    }

    private bool TryDash()
    {
        if (dashAbility != null)
        {
            if(dashAbility.Use())
            {
                if (!audioSources[2].isPlaying)
                    audioSources[2].Play();
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
    }

    private void DashAbility_OnActivated(object sender, EventArgs e)
    {
        velocity = transform.forward * dashAbility.AbilityValue;
    }

    private void DashAbility_OnUsing(object sender, EventArgs e)
    {
        velocity -= velocity * (Time.deltaTime * 0.1f);
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
        System.Random rand = new System.Random();
        audioSources[rand.Next(32,35)].Play();
        attackInProgressTimer += e.AnimationDuration;
        e.ProjectileScript.OnHit += ProjectileScript_OnHit;
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
        if (TeamHealth == 0)
        {
            if (!audioSources[1].isPlaying)
                audioSources[1].Play();
            Destroy(gameObject);
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
        if (!audioSources[23].isPlaying && !audioSources[24].isPlaying && !audioSources[25].isPlaying && !audioSources[26].isPlaying)
        {
            System.Random rand = new System.Random();
                audioSources[rand.Next(23,27)].Play();
        }
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

    public float PrimaryHeat
    {
        get
        {
            if (primaryWeapon != null)
                return primaryWeapon.Heat;

            return 0;
        }
    }

    public float PrimaryMaxHeat
    {
        get
        {
            if (primaryWeapon != null)
                return primaryWeapon.MaxHeat;

            return 0;
        }
    }

    public float AbilityEnergy(int abilityNumber)
    {
        float energyLevel = 0;
        switch (abilityNumber)
        {
            case 1:
                {
                    if (dashAbility != null)
                        energyLevel = dashAbility.Energy;
                    else energyLevel = 0;
                }
                break;
            case 2:
                {
                    if (ability != null)
                        energyLevel = ability.Energy;
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

    public float MaxEnergy
    {
        get
        {
            if (ability != null)
                return ability.MaxEnergy;

            return 0;
        }
    }

    public void   PlayMusicThemePicker(int tmp, bool state)

    {
        PlayMusicTheme[tmp] = state;
    }
}
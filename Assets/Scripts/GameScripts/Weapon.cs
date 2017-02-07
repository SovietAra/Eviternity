using Assets.Scripts;
using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 20f)]
    private float fireRate = 1f;

    [SerializeField]
    [Range(0f, 45f)]
    private float maxSprayAngle = 20f;
    
    [SerializeField]
    [Range(0, 1000)]
    private int maxAmmoPerClip = 100;

    [SerializeField]
    [Range(0.01f, 100f)]
    private float reloadTimePerShell = 0.75f;

    [SerializeField]
    [Range(0.01f, 100f)]
    private float reloadTimePerClip = 2f;

    [SerializeField]
    [Range(0, 1000)]
    private float maxHeat = 50;

    [SerializeField]
    [Range(0, 1000)]
    private float heat = 0;

    [SerializeField]
    [Range(0, 1000)]
    private float heatPerShot = 1f;

    [SerializeField]
    [Range(0, 1000)]
    private float heatReductionPerSecond = 0.5f;

    [SerializeField]
    [Range(0, 100)]
    private float heatReductionDelay = 2f;

    [Range(0, 100)]
    public float AnimationDuration = 0;

    [SerializeField]
    private bool smoothSpray = false;

    [SerializeField]
    private float sprayValueSpeed = 2f;

    [SerializeField]
    [Range(0, 30)]
    private float spawnDelay = 0f;

    [SerializeField]
    private HeatDelayType heatDelayType = HeatDelayType.Always;

    public enum HeatDelayType
    {
        Always,
        OnOverheat
    }

    private float sprayAngle = 0f;
    
    public bool AllowShellRelaod = false;
    public bool UseAmmo = true;
    public bool AutoReload = true;
    public bool ProduceHeat = false;
    public bool HeatCooldown = false;
    public bool OverwriteSound = true;
    public AudioClip FireSound;
    public AudioClip ReloadSound;


    public GameObject SecondaryWeapon;
    public GameObject Projectile;
    private Weapon secondaryWeapon;

    private int currentClipAmmo;
    
    private float elapsedAttackDelay;
    private float elapsedReloadTime;
    private float elapsedSpawnDelay;
    private float elapsedHeatReductionDelay;
    private bool reloading;
    private bool overheat = false;

    public event EventHandler<WeaponEventArgs> OnPrimaryAttack;
    public event EventHandler<WeaponEventArgs> OnSecondaryAttack;
    public event EventHandler<WeaponEventArgs> OnDelayedPrimaryAttack;
    public event EventHandler<WeaponEventArgs> OnDelayedSecondaryAttack;
    public event EventHandler<SpawnEventArgs> OnSpawning;
    public event EventHandler OnReloadBegin;
    public event EventHandler OnReloadEnd;
    public event EventHandler OnReloadAbort;

    private AudioSource audioSource;

    private Vector3 delayedSpawnPosition;
    private float delayedSpawnAngle;
    private Vector3 delayedSpawnForward;
    private bool delayedSpawn;
    private bool attacked = false;

    public float MaxHeat
    {
        get { return maxHeat; }
    }

    public float Heat
    {
        get { return heat;  }
    }

    private void Start()
    {
        elapsedAttackDelay = fireRate;
        currentClipAmmo = maxAmmoPerClip;
        if (SecondaryWeapon != null)
        {
            secondaryWeapon = Instantiate(SecondaryWeapon, transform.parent).GetComponent<Weapon>();
            secondaryWeapon.OnPrimaryAttack += SecondaryWeapon_OnPrimaryAttack;
            secondaryWeapon.OnDelayedPrimaryAttack += SecondaryWeapon_OnDelayedPrimaryAttack;
            secondaryWeapon.OnSpawning += SecondaryWeapon_OnSpawning;
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void SecondaryWeapon_OnSpawning(object sender, SpawnEventArgs e)
    {
        if (OnSpawning != null)
            OnSpawning(this, e);
    }

    private void Update()
    {
        elapsedAttackDelay += Time.deltaTime;
        elapsedHeatReductionDelay += Time.deltaTime;

        if (delayedSpawn)
        {
            elapsedSpawnDelay += Time.deltaTime;

            if (elapsedSpawnDelay > spawnDelay)
            {
                SpawnObject(delayedSpawnPosition, delayedSpawnForward, delayedSpawnAngle);
            }
        }

        UpdateReload();
        if (!attacked && elapsedAttackDelay >= fireRate)
        {
            UpdateHeat();
        }
        else
        {
            attacked = false;
        }
    }

    private void UpdateReload()
    {
        if (reloading)
        {
            elapsedReloadTime += Time.deltaTime;
            if (UseAmmo)
            {
                if (AllowShellRelaod)
                {
                    if (elapsedReloadTime >= reloadTimePerShell)
                    {
                        currentClipAmmo++;
                        elapsedReloadTime = 0f;
                        if (currentClipAmmo == maxAmmoPerClip)
                        {
                            reloading = false;
                            if (OnReloadEnd != null)
                                OnReloadEnd(this, EventArgs.Empty);
                        }
                    }
                }
                else
                {
                    if (elapsedReloadTime >= reloadTimePerClip)
                    {
                        currentClipAmmo = maxAmmoPerClip;
                        reloading = false;

                        if (OnReloadEnd != null)
                            OnReloadEnd(this, EventArgs.Empty);
                    }
                }
            }
        }
    }

    private void UpdateHeat()
    {
        if (ProduceHeat)
        {
            if (elapsedHeatReductionDelay >= heatReductionDelay || (heatDelayType == HeatDelayType.OnOverheat && !overheat))
            {
                sprayAngle = 0f;
                heat -= heatReductionPerSecond * Time.deltaTime;
                if (heat <= 0)
                {
                    heat = 0;
                    overheat = false;
                }
            }
        }
    }

    public virtual bool PrimaryAttack(Vector3 spawnPosition, Vector3 forward, float angle)
    {
        if (elapsedAttackDelay >= fireRate && ((UseAmmo && currentClipAmmo > 0) || (ProduceHeat && ((HeatCooldown && !overheat) || !HeatCooldown) && heat < maxHeat) || (!UseAmmo && !ProduceHeat)) && (!reloading || AllowShellRelaod))
        {
            if (AllowShellRelaod && reloading)
            {
                reloading = false;

                if (OnReloadEnd != null)
                    OnReloadEnd(this, EventArgs.Empty);
            }
            
            if(smoothSpray)
            {
                sprayAngle += sprayValueSpeed;
                if(sprayAngle > maxSprayAngle / 2f)
                {
                    sprayAngle = maxSprayAngle / 2f;
                    sprayValueSpeed *= -1f;
                }
                else if(sprayAngle < maxSprayAngle / -2f)
                {
                    sprayAngle = maxSprayAngle / -2f;
                    sprayValueSpeed *= -1f;
                }
            }
            else
            {
                sprayAngle = UnityEngine.Random.Range(-(maxSprayAngle / 2f), (maxSprayAngle / 2f));
            }

            attacked = true;
            if (spawnDelay <= 0)
            {
                SpawnObject(spawnPosition, forward, angle);
            }
            else
            {
                delayedSpawn = true;
                delayedSpawnForward = forward;
                delayedSpawnAngle = angle;
                delayedSpawnPosition = spawnPosition;
                if (OnDelayedPrimaryAttack != null)
                    OnDelayedPrimaryAttack(this, new WeaponEventArgs(null, null, AnimationDuration, true));
            }
            return true;
        }
        else if (currentClipAmmo <= 0 && !reloading)
        {
            if (AutoReload)
            {
                Reload();
            }
        }
        return false;
    }

    private void SpawnObject(Vector3 spawnPosition, Vector3 forward, float angle)
    {
        SpawnEventArgs spawnArgs = new SpawnEventArgs(spawnPosition, forward, angle);
        if (OnSpawning != null)
            OnSpawning(this, spawnArgs);

        GameObject gobj = Instantiate(Projectile, spawnArgs.SpawnPosition + spawnArgs.Forward, Quaternion.Euler(0.0f, (spawnArgs.Angle + sprayAngle), 0));
        Projectile projectile = gobj.GetComponent<Projectile>();
        if (transform.parent != null)
            projectile.Attacker = transform.parent.gameObject;

        if (OnPrimaryAttack != null)
            OnPrimaryAttack(this, new WeaponEventArgs(gobj, projectile, AnimationDuration, delayedSpawn));

        if (audioSource != null && FireSound != null)
        {
            if (!audioSource.isPlaying || OverwriteSound)
            {
                audioSource.clip = FireSound;
                audioSource.Play();
            }
        }

        elapsedAttackDelay = 0f;
        elapsedSpawnDelay = 0f;
        delayedSpawn = false;
        delayedSpawnAngle = 0f;
        delayedSpawnForward = Vector3.zero;
        delayedSpawnPosition = Vector3.zero;

        if (UseAmmo)
        {
            currentClipAmmo--;
        }
        else if (ProduceHeat)
        {
            elapsedHeatReductionDelay = 0f;
            heat += heatPerShot;
            overheat = heat >= maxHeat;

            if (overheat)
            {
                if (audioSource != null && ReloadSound != null)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = ReloadSound;
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public virtual bool SecondaryAttack(Vector3 spawnPosition, Vector3 forward, float angle)
    {
        if (secondaryWeapon != null)
        {
            return secondaryWeapon.PrimaryAttack(spawnPosition, forward, angle);
        }
        return false;
    }

    private void SecondaryWeapon_OnPrimaryAttack(object sender, WeaponEventArgs e)
    {
        if (OnSecondaryAttack != null)
            OnSecondaryAttack(sender, e);
    }

    private void SecondaryWeapon_OnDelayedPrimaryAttack(object sender, WeaponEventArgs e)
    {
        if (OnDelayedSecondaryAttack != null)
            OnDelayedSecondaryAttack(this, e);
    }

    public void Reload()
    {
        if (UseAmmo)
        {
            if (!reloading)
            {
                if (audioSource != null && ReloadSound != null)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = ReloadSound;
                        audioSource.Play();
                    }
                }
            }

            elapsedReloadTime = 0f;
            reloading = true;
            if (OnReloadBegin != null)
                OnReloadBegin(this, EventArgs.Empty);
        }
    }

    public void AbortReload()
    {
        if (UseAmmo)
        {
            elapsedReloadTime = 0f;
            reloading = false;

            if (OnReloadAbort != null)
                OnReloadAbort(this, EventArgs.Empty);
        }
    }
}


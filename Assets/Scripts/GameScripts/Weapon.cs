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
    [Range(0.01f, 10f)]
    private float reloadTimePerShell = 0.75f;

    [SerializeField]
    [Range(0.01f, 10f)]
    private float reloadTimePerClip = 2f;

    [SerializeField]
    [Range(0, 1000)]
    private float maxHeat = 50;

    [SerializeField]
    [Range(0, 1000)]
    private float heat = 0;

    [SerializeField]
    [Range(0.1f, 1000)]
    private float heatPerShot = 1f;

    [SerializeField]
    [Range(0.1f, 1000)]
    private float heatReductionPerSecond = 0.5f;

    [SerializeField]
    [Range(0, 10)]
    private float heatReductionDelay = 2f;

    [Range(0, 10)]
    public float AnimationDuration = 0;

    [SerializeField]
    private bool smoothSpray = false;

    [SerializeField]
    private float sprayValueSpeed = 2f;

    private float sprayAngle = 0f;
    
    public bool AllowShellRelaod = false;
    public bool UseAmmo = true;
    public bool AutoReload = true;
    public bool ProduceHeat = false;
    public bool HeatCooldown = false;
    public GameObject SecondaryWeapon;
    public GameObject Projectile;
    private Weapon secondaryWeapon;

    private int currentClipAmmo;
    
    private float elapsedAttackDelay;
    private float elapsedReloadTime;
    private float elapsedHeatReductionDelay;
    private bool reloading;
    private bool overheat = false;

    public event EventHandler<WeaponEventArgs> OnPrimaryAttack;
    public event EventHandler<WeaponEventArgs> OnSecondaryAttack;
    public event EventHandler OnReloadBegin;
    public event EventHandler OnReloadEnd;
    public event EventHandler OnReloadAbort;

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
        }
    }

    private void Update()
    {
        elapsedAttackDelay += Time.deltaTime;
        elapsedHeatReductionDelay += Time.deltaTime;
        
        UpdateReload();
        UpdateHeat();
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
            if (elapsedHeatReductionDelay >= heatReductionDelay)
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

            GameObject gobj = Instantiate(Projectile, spawnPosition + forward, Quaternion.Euler(0.0f, (angle + sprayAngle), 0));
            Projectile projectile = gobj.GetComponent<Projectile>();
            if(transform.parent != null)
                projectile.Attacker = transform.parent.gameObject;

            if(OnPrimaryAttack != null)
                OnPrimaryAttack(this, new WeaponEventArgs(gobj, projectile, AnimationDuration));

            elapsedAttackDelay = 0f;

            if (UseAmmo)
            {
                currentClipAmmo--;
            }
            else if (ProduceHeat)
            {
                elapsedHeatReductionDelay = 0f;
                heat += heatPerShot;
                overheat = heat >= maxHeat;
            }

            return true;
        }
        else if (currentClipAmmo <= 0)
        {
            if (AutoReload)
            {
                Reload();
            }
        }
        return false;
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

    public void Reload()
    {
        if (UseAmmo)
        {
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


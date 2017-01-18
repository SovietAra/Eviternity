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
    private float sprayAngle = 20f;
    
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

    public bool AllowShellRelaod = false;
    public bool UseAmmo = true;
    public bool AutoReload = true;
    public bool ProduceHeat = false;
    public GameObject SecondaryWeapon;
    public GameObject Projectile;
    private Weapon secondaryWeapon;

    private int currentClipAmmo;
    
    private float elapsedAttackDelay;
    private float elapsedReloadTime;
    private float elapsedHeatReductionDelay;
    private bool reloading;

    public event EventHandler<WeaponEventArgs> OnPrimaryAttack;
    public event EventHandler<WeaponEventArgs> OnSecondaryAttack;
    public event EventHandler OnReloadBegin;
    public event EventHandler OnReloadEnd;
    public event EventHandler OnReloadAbort;

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
                heat -= heatReductionPerSecond * Time.deltaTime;
                if (heat < 0)
                    heat = 0;
            }
        }
    }

    public virtual bool PrimaryAttack(Vector3 spawnPosition, Vector3 forward, float angle)
    {
        if (elapsedAttackDelay >= fireRate && ((UseAmmo && currentClipAmmo > 0) || (ProduceHeat && heat < maxHeat) || (!UseAmmo && !ProduceHeat)) && (!reloading || AllowShellRelaod))
        {
            if (AllowShellRelaod && reloading)
            {
                reloading = false;

                if (OnReloadEnd != null)
                    OnReloadEnd(this, EventArgs.Empty);
            }

            //TODO: smooth spray
            GameObject gobj = Instantiate(Projectile, spawnPosition + forward, Quaternion.Euler(0.0f, (angle + UnityEngine.Random.Range(-(sprayAngle / 2f), (sprayAngle / 2f))), 0));
            Projectile projectile = gobj.GetComponent<Projectile>();
            projectile.AttackerTag = transform.parent.tag;

            if(OnPrimaryAttack != null)
                OnPrimaryAttack(this, new WeaponEventArgs(gobj, projectile));

            elapsedAttackDelay = 0f;

            if (UseAmmo)
            {
                currentClipAmmo--;
            }
            else if (ProduceHeat)
            {
                elapsedHeatReductionDelay = 0f;
                heat += heatPerShot;
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


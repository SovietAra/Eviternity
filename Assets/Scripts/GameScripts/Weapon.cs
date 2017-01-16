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

    public GameObject Projectile;

    private int currentClipAmmo;
    private float currentHeat;
    private float elapsedAttackDelay;
    private float elapsedReloadTime;
    private float elapsedHeatReductionDelay;
    private bool reloading;

    private void Start()
    {
        elapsedAttackDelay = fireRate;
        currentClipAmmo = maxAmmoPerClip;
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
                        }
                    }
                }
                else
                {
                    if (elapsedReloadTime >= reloadTimePerClip)
                    {
                        currentClipAmmo = maxAmmoPerClip;
                        reloading = false;
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
                currentHeat -= heatReductionPerSecond * Time.deltaTime;
            }
        }
    }

    public virtual bool PrimaryAttack(Vector3 spawnPosition, Vector3 forward, float angle)
    {
        if (elapsedAttackDelay >= fireRate && ((UseAmmo && currentClipAmmo > 0) || (ProduceHeat && currentHeat < maxHeat) || (!UseAmmo && !ProduceHeat)) && (!reloading || AllowShellRelaod))
        {
            if (AllowShellRelaod && reloading)
            {
                reloading = false;
            }

            //TODO: smooth spray
            GameObject gobj = Instantiate(Projectile, spawnPosition + forward, Quaternion.Euler(0.0f, (angle + Random.Range(-(sprayAngle / 2f), (sprayAngle / 2f))), 0));
            Projectile projectile = gobj.GetComponent<Projectile>();
            projectile.AttackerTag = transform.parent.tag;

            elapsedAttackDelay = 0f;

            if (UseAmmo)
            {
                currentClipAmmo--;
            }
            else if (ProduceHeat)
            {
                elapsedHeatReductionDelay = 0f;
                currentHeat += heatPerShot;
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
        return false;
    }

    public void Reload()
    {
        if (UseAmmo)
        {
            elapsedReloadTime = 0f;
            reloading = true;
        }
    }

    public void AbortReload()
    {
        if (UseAmmo)
        {
            elapsedReloadTime = 0f;
            reloading = false;
        }
    }
}


using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponEventArgs : EventArgs
    {
        public GameObject ProjectileObject;
        public Projectile ProjectileScript;

        public WeaponEventArgs(GameObject projectileObject, Projectile projectileScript)
        {
            ProjectileObject = projectileObject;
            ProjectileScript = projectileScript;
        }
    }
}

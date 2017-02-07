using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponEventArgs : EventArgs
    {
        public GameObject ProjectileObject;
        public Projectile ProjectileScript;
        public float AnimationDuration;
        public bool Delayed;

        public WeaponEventArgs(GameObject projectileObject, Projectile projectileScript, float animationDuration, bool delayed)
        {
            ProjectileObject = projectileObject;
            ProjectileScript = projectileScript;
            AnimationDuration = animationDuration;
            Delayed = delayed;
        }
    }
}

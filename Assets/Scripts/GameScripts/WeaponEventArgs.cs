using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponEventArgs : EventArgs
    {
        public GameObject ProjectileObject;
        public Projectile ProjectileScript;
        public float AnimationDuration;

        public WeaponEventArgs(GameObject projectileObject, Projectile projectileScript, float animationDuration)
        {
            ProjectileObject = projectileObject;
            ProjectileScript = projectileScript;
            AnimationDuration = animationDuration;
        }
    }
}

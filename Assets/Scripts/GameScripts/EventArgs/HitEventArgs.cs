using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class HitEventArgs : EventArgs
    {
        public float FinalDamage;
        public bool Cancel;
        public readonly GameObject Attacker;
        public readonly GameObject Victim;
        public readonly bool IsTeamDamage;
        public readonly bool IsAOEDamage;

        public HitEventArgs(float finalDamage, GameObject attacker, GameObject victim, bool isTeamDamage, bool isAOEDamage)
        {
            FinalDamage = finalDamage;
            Attacker = attacker;
            Victim = victim;
            IsTeamDamage = isTeamDamage;
            IsAOEDamage = isAOEDamage;
            Cancel = false;
        }
    }
}

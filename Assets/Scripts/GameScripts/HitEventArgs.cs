using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class HitEventArgs : EventArgs
    {
        public float FinalDamage;
        public bool Cancel;
        public readonly string AttackerTag;
        public readonly GameObject Victim;
        public readonly bool IsTeamDamage;
        public readonly bool IsAOEDamage;

        public HitEventArgs(float finalDamage, string attackerTag, GameObject victim, bool isTeamDamage, bool isAOEDamage)
        {
            FinalDamage = finalDamage;
            AttackerTag = attackerTag;
            Victim = victim;
            IsTeamDamage = isTeamDamage;
            IsAOEDamage = isAOEDamage;
            Cancel = false;
        }
    }
}

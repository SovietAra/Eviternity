using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class StatusEffectArgs : EventArgs
    {
        public StatusEffect StatusScript;
        public GameObject StatusObject;
        public bool Cancel;

        public StatusEffectArgs(StatusEffect statusScript, GameObject statusObject)
        {
            StatusScript = statusScript;
            StatusObject = statusObject;
            Cancel = false;
        }
    }
}

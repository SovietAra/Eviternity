using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class OnHealthChangedArgs : EventArgs
    {
        public GameObject ResponsibleObject;
        public float ChangeValue;
        public GameObject StatusEffect;
        public bool Cancel;

        public OnHealthChangedArgs(GameObject responsibleObject, float changeValue, GameObject statusEffect)
        {
            ResponsibleObject = responsibleObject;
            ChangeValue = changeValue;
            StatusEffect = statusEffect;
            Cancel = false;
        }
    }
}

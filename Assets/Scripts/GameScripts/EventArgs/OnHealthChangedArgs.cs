using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class OnHealthChangedArgs : EventArgs
    {
        public GameObject ResponsibleObject;
        public float ChangeValue;
        public bool Cancel;

        public OnHealthChangedArgs(GameObject responsibleObject, float changeValue)
        {
            ResponsibleObject = responsibleObject;
            ChangeValue = changeValue;
            Cancel = false;
        }
    }
}

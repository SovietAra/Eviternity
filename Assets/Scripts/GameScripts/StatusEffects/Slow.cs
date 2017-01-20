using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Slow : StatusEffect
    {
        [SerializeField]
        [Range(0.0f, 10.0f)]
        protected float slowDownPercentage = 0.5f;

        protected MoveScript moveScript;

        public override void Start()
        {
            OnActivate += Slow_OnActivate;
            OnDeactivate += Slow_OnDeactivate;
            base.Start();
        }

        private void Slow_OnDeactivate(object sender, EventArgs e)
        {
            if (moveScript != null)
            {
                moveScript.ResetMultiplicator();
            }
        }

        private void Slow_OnActivate(object sender, EventArgs e)
        {
            GetMoveScript();
        }

        public override void Do()
        {
            if (moveScript == null)
            {
                GetMoveScript();
            }
            else
            {
                moveScript.MovementMultiplicator = moveScript.DefaultMovementMultiplicator - slowDownPercentage;
            }
        }

        private void GetMoveScript()
        {
            if (characterGameObject != null)
            {
                moveScript = characterGameObject.GetComponent<MoveScript>();
                if (moveScript != null)
                {
                    moveScript.MovementMultiplicator = moveScript.DefaultMovementMultiplicator - slowDownPercentage;
                }
                else
                {
                    Deactivate();
                }
            }
            else
            {
                Deactivate();

            }
        }
    }
}

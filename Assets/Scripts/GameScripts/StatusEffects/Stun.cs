
namespace Assets.Scripts
{
    public class Stun : Slow
    {
        public override void Start()
        {
            if(moveScript != null)
                slowDownPercentage = moveScript.DefaultMovementMultiplicator;

            base.Start();
        }

        public override void Update()
        {
            if(moveScript != null)
                slowDownPercentage = moveScript.DefaultMovementMultiplicator;

            base.Update();
        }
    }
}

using System;
using UnityEngine;

namespace Assets.Scripts
{
    public static class MathUtil
    {
        public static float CalculateAngle(Vector2 target, Vector2 source)
        {
            return ((float)Math.Atan2(target.y - source.y, target.x - source.x)) * (180f / (float)Math.PI);
        }

        public static float FixAngle(float value)
        {
            if (value < 0)
                return 360 + value;
            
            return value;
        }
    }
}

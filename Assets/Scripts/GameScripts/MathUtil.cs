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

        public static bool Between(float value, float minValue, float maxValue)
        {
            if (value >= minValue - Mathf.Epsilon && value <= maxValue + Mathf.Epsilon)
                return true;

            return false;
        }

        public static Vector3 ScaleVector(Vector3 vec)
        {
            float[] values = ScaleValues(new float[] { vec.x, vec.y, vec.z });
            return new Vector3(values[0], values[1], values[2]);
        }

        public static Vector3 ScaleVectorXZ(Vector3 vec)
        {
            float[] values = ScaleValues(new float[] { vec.x, vec.z });
            return new Vector3(values[0], vec.y, values[1]);
        }

        public static float[] ScaleValues(float[] values)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max)
                    max = values[i];
                else if (values[i] < min)
                    min = values[i];
            }

            float[] scaledValues = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                scaledValues[i] = ScaleValue(min, max, values[i]);
            }

            return scaledValues;
        }

        public static float ScaleValue(float min, float max, float value)
        {
            return ((value - min) / (max - min));
        }
    }
}

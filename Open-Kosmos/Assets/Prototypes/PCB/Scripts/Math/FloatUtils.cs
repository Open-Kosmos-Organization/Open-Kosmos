using Unity.Mathematics;
using UnityEngine;

namespace Kosmos.Prototypes.PCB.Math
{
    public static class FloatUtils
    {
        public const double DoubleEpsilon = math.EPSILON_DBL * 128;
        public const float FloatEpsilon = math.EPSILON * 128;

        public static bool Approximate(double a, double b, double epsilon = DoubleEpsilon, double absoluteThreshold = double.MinValue)
        {
            if (a == b)
            {
                return true;
            }
            
            double difference = math.abs(a - b);

            double norm = math.min((math.abs(a) + math.abs(b)), double.MaxValue);

            return difference < math.max(absoluteThreshold, epsilon * norm);
        }

        public static bool Approximate(float a, float b, float epsilon = FloatEpsilon, float absoluteThreshold = float.MinValue)
        {            
            if (a == b)
            {
                return true;
            }
            
            float difference = math.abs(a - b);

            float norm = math.min((math.abs(a) + math.abs(b)), float.MaxValue);

            return difference < math.max(absoluteThreshold, epsilon * norm);
        }

        public static Vector3 ToVector3(this double3 value)
        {
            return new Vector3((float)value.x, (float)value.y, (float)value.z);
        }
    }
}
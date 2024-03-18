using System;

namespace Stellamod.Helpers
{
    internal static class Easing
    {
        public static float InExpo(float t, float p = 10) => (float)Math.Pow(2, p * (t - 1));
        public static float OutExpo(float t) => 1 - InExpo(1 - t);
        public static float InOutExpo(float t, float p = 10)
        {
            if (t < 0.5) return InExpo(t * 2, p) / 2;
            return 1 - InExpo((1 - t) * 2, p) / 2;
        }
    }
}

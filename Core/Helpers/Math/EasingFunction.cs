using System;

namespace Stellamod.Core.Helpers.Math
{
    public delegate float Easer(float t);

    public static class EasingFunction
    {
        //REFERENCE:
        //https://easings.net/
     
        public static float Clamp(float t)
        {
            return System.Math.Clamp(t, 0, 1);
        }

        public static readonly Easer None = delegate (float t)
        {
            t = Clamp(t);
            return t;
        };
        public static readonly Easer InSine = delegate (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Cos((t * MathF.PI) / 2);
        };

        public static readonly Easer OutSine = delegate (float t)
        {
            t = Clamp(t);
            return MathF.Sin((t * MathF.PI) / 2);
        };

        public static readonly Easer InOutSine = delegate (float t)
        {
            t = Clamp(t);
            return -(MathF.Cos(MathF.PI * t) - 1) / 2;
        };

        public static readonly Easer InQuad = delegate (float t)
        {
            t = Clamp(t);
            return t * t;
        };

        public static readonly Easer OutQuad = delegate (float t)
        {
            t = Clamp(t);
            return 1 - (1 - t) * (1 - t);
        };

        public static readonly Easer InOutQuad = delegate (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
        };

        public static readonly Easer InCubic = delegate (float t)
        {
            t = Clamp(t);
            return t * t * t;
        };

        public static readonly Easer OutCubic = delegate (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 3);
        };

        public static readonly Easer InOutCubic = delegate (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 4 * t * t * t : 1 - MathF.Pow(-2 * t + 2, 3) / 2;
        };

        public static readonly Easer InQuart = delegate (float t)
        {
            t = Clamp(t);
            return t * t * t * t;
        };

        public static readonly Easer OutQuart = delegate (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 4);
        };

        public static readonly Easer InOutQuart = delegate (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 8 * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 4) / 2;
        };

        public static readonly Easer InQuint = delegate (float t)
        {
            t = Clamp(t);
            return t * t * t * t * t;
        };

        public static readonly Easer OutQuint = delegate (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 5);
        };

        public static readonly Easer InOutQuint = delegate (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 16 * t * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 5) / 2;
        };

        public static readonly Easer InExpo = delegate (float t)
        {
            t = Clamp(t);
            const float p = 10;
            return t == 0 ? 0 : MathF.Pow(2, p * t - p);
        };

        public static readonly Easer OutExpo = delegate (float t)
        {
            const float p = 10;
            t = Clamp(t);
            return t == 0 ? t : 1 - MathF.Pow(2, -p * t);
        };

        public static readonly Easer InOutExpo = delegate (float t)
        {
            const float p = 10;
            t = Clamp(t);
            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f ? MathF.Pow(2, (p * 2) * t - p) / 2
                : (2 - MathF.Pow(2, -(p * 2) * t + p)) / 2;
        };

        public static readonly Easer InOutExpo7 = delegate (float t)
        {
            const float p = 7;
            t = Clamp(t);
            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f ? MathF.Pow(2, (p * 2) * t - p) / 2
                : (2 - MathF.Pow(2, -(p * 2) * t + p)) / 2;
        };

        public static readonly Easer InCirc = delegate (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Sqrt(1 - MathF.Pow(t, 2));
        };

        public static readonly Easer OutCirc = delegate (float t)
        {
            t = Clamp(t);
            return MathF.Sqrt(1 - MathF.Pow(t - 1, 2));
        };


        public static readonly Easer InOutCirc = delegate (float t)
        {
            t = Clamp(t);
            return t < 0.5f
                ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * t, 2))) / 2
                : (MathF.Sqrt(1 - MathF.Pow(-2 * t + 2, 2)) + 1) / 2;
        };


        public static readonly Easer InBack = delegate (float t)
        {
            t = Clamp(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * t * t * t - c1 * t * t;
        };


        public static readonly Easer OutBack = delegate (float t)
        {
            t = Clamp(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(t - 1, 3) + c1 + MathF.Pow(t - 1, 2);
        };


        public static readonly Easer InOutBack = delegate (float t)
        {
            t = Clamp(t);

            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f
                ? (MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
                : (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        };

        public static readonly Easer InElastic = delegate (float t)
        {
            t = Clamp(t);

            const float c4 = (2 * MathF.PI) / 3;
            return t == 0
                ? 0
                : t == 1
                ? 1
                : -MathF.Pow(2, 10 * t - 10) * MathF.Sin((t * 10 - 10.75f) * c4);
        };

        public static readonly Easer OutElastic = delegate (float t)
        {
            t = Clamp(t);
            const float c4 = (2 * MathF.PI) / 3;
            return t == 0
                ? 0
                : t == 1
                ? 1
                : MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * c4) + 1;
        };

        public static readonly Easer InOutElastic = delegate (float t)
        {
            t = Clamp(t);
            const float c5 = (2 * MathF.PI) / 4.5f;

            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f
                ? -(MathF.Pow(2, 20 * t - 10) * MathF.Sin((20 * t - 11.125f) * c5)) / 2
                : (MathF.Pow(2, -20 * t + 10) * MathF.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
        };

        public static readonly Easer InBounce = delegate (float t)
        {
            t = Clamp(t);
            return 1 - OutBounce(1 - t);
        };

        public static readonly Easer OutBounce = delegate (float t)
        {
            t = Clamp(t);
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5f / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        };

        public static readonly Easer InOutBounce = delegate (float t)
        {
            t = Clamp(t);
            return t < 0.5f
                ? (1 - OutBounce(1 - 2 * t)) / 2
                : (1 + OutBounce(2 * t - 1)) / 2;
        };

        public static readonly Easer SinBump = delegate (float t)
        {
            t = Clamp(t);
            const float factor = 2;
            return t * (factor - t * factor);
        };

        public static readonly Easer TriBump = delegate (float t)
        {
            t = Clamp(t);
            const float factor = 3;
            return t * (factor - t * factor);
        };

        public static readonly Easer QuadraticBump = delegate (float t)
        {
            t = Clamp(t);
            const float factor = 4;
            return t * (factor - t * factor);
        };
    }
}

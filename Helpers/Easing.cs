using System;

namespace Stellamod.Helpers
{

    /// <summary>
    /// A class that calculates eased times.
    /// </summary>
    public static class Easing
    {

        private static float Clamp(float t)
        {
            return Math.Clamp(t, 0, 1);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float InSine(float t)
        {
            t = Clamp(t);
            return 1 - MathF.Cos((t * MathF.PI) / 2);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float OutSine(float t)
        {
            t = Clamp(t);
            return MathF.Sin((t * MathF.PI) / 2);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float InOutSine(float t)
        {
            t = Clamp(t);
            return -(MathF.Cos(MathF.PI * t) - 1) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float InQuad(float t)
        {
            t = Clamp(t);
            return t * t;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float OutQuad(float t)
        {
            t = Clamp(t);
            return 1 - (1 - t) * (1 - t);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float InOutQuad(float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float InCubic(float t)
        {
            t = Clamp(t);
            return t * t * t;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float OutCubic(float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 3);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>
        ///</Summary>
        public static float InOutCubic(float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 4 * t * t * t : 1 - MathF.Pow(-2 * t + 2, 3) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InQuart(float t)
        {
            t = Clamp(t);
            return t * t * t * t;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutQuart(float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 4);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutQuart(float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 8 * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 4) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InQuint(float t)
        {
            t = Clamp(t);
            return t * t * t * t * t;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutQuint(float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 5);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutQuint(float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 16 * t * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 5) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InExpo(float t, float p = 10)
        {
            t = Clamp(t);
            return t == 0 ? 0 : MathF.Pow(2, p * t - p);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutExpo(float t, float p = 10)
        {
            t = Clamp(t);
            return t == 0 ? t : 1 - MathF.Pow(2, -p * t);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutExpo(float t, float p = 10)
        {
            t = Clamp(t);
            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f ? MathF.Pow(2, (p * 2) * t - p) / 2
                : (2 - MathF.Pow(2, -(p * 2) * t + p)) / 2;
        }


        public static float SpikeOrb(float t)
        {
            if (t <= .5f)
                return InOutExpo(t / .5f, 8);
            else
            {
                return InOutExpo((1-t) / 0.5f, 8); 
            }
        }

        public static float SpikeCirc(float t)
        {
            if (t <= .5f)
                return InOutCirc(t / .5f);
            else
            {
                return InOutCirc((1 - t) / 0.5f);
            }
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InCirc(float t)
        {
            t = Clamp(t);
            return 1 - MathF.Sqrt(1 - MathF.Pow(t, 2));
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutCirc(float t)
        {
            t = Clamp(t);
            return MathF.Sqrt(1 - MathF.Pow(t - 1, 2));
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutCirc(float t)
        {
            t = Clamp(t);
            return t < 0.5f
                ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * t, 2))) / 2
                : (MathF.Sqrt(1 - MathF.Pow(-2 * t + 2, 2)) + 1) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InBack(float t)
        {
            t = Clamp(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * t * t * t - c1 * t * t;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutBack(float t)
        {
            t = Clamp(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(t - 1, 3) + c1 + MathF.Pow(t - 1, 2);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutBack(float t)
        {
            t = Clamp(t);

            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f
                ? (MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
                : (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InElastic(float t)
        {
            t = Clamp(t);

            const float c4 = (2 * MathF.PI) / 3;
            return t == 0
                ? 0
                : t == 1
                ? 1
                : -MathF.Pow(2, 10 * t - 10) * MathF.Sin((t * 10 - 10.75f) * c4);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutElastic(float t)
        {
            t = Clamp(t);
            const float c4 = (2 * MathF.PI) / 3;
            return t == 0
                ? 0
                : t == 1
                ? 1
                : MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * c4) + 1;
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutElastic(float t)
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
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InBounce(float t)
        {
            t = Clamp(t);
            return 1 - OutBounce(1 - t);
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float OutBounce(float t)
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
        }

        ///<Summary>
        /// Useful easing function for making things smooth! <see href="https://easings.net/">Ease Functions</see>           
        ///</Summary>
        public static float InOutBounce(float t)
        {
            t = Clamp(t);
            return t < 0.5f
                ? (1 - OutBounce(1 - 2 * t)) / 2
                : (1 + OutBounce(2 * t - 1)) / 2;
        }
    }
}

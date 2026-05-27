using Microsoft.Xna.Framework;
using System;

namespace Stellamod.Helpers
{
    public delegate float Easer(float t);
    public static class EasingFunction
    {
        //REFERENCE:
        //https://easings.net/

        public static Vector2 CubicBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            //B(t) = (1-t)^3 * P0 + 3*(1-t)^2 * t * P1 + 3*(1-t) * t^2 * P2 + t^3 * P3

            Vector2 t0 = MathF.Pow(1 - t, 3) * p0;
            Vector2 t1 = 3 * MathF.Pow(1 - t, 2) * t * p1;
            Vector2 t2 = 3 * (1 - t) * t * t * p2;
            Vector2 t3 = MathF.Pow(t, 3) * p3;

            return t0 + t1 + t2 + t3;
        }

        public static float BezierEase(float t, Vector2 control1, Vector2 control2)
        {
            Vector2 point = CubicBezier(t, Vector2.Zero, control1, control2, Vector2.One);
            return point.Y;
        }
        public static  float Anticipation (float t)
        {
            t = Clamp(t);
            Vector2 control1 = new Vector2(0.8f, -0.4f);
            Vector2 control2 = new Vector2(0.1f, 1.3f);
            return EasingFunction.BezierEase(t, control1, control2);
        }
        public static  float Anticipation2 (float t)
        {
            t = Clamp(t);
            Vector2 control1 = new Vector2(0.8f, -0.4f);
            Vector2 control2 = new Vector2(0.5f, 1f);
            return EasingFunction.BezierEase(t, control1, control2);
        }

        public static  float Anticipation3(float t)
        {
            t = Clamp(t);
            Vector2 control1 = new Vector2(0.164f, -0.392f);
            Vector2 control2 = new Vector2(0, 0.92f);
            return EasingFunction.BezierEase(t, control1, control2);
        }

        public static float Clamp(float t)
        {
            return Math.Clamp(t, 0, 1);
        }

        public static  float None (float t)
        {
            t = Clamp(t);
            return t;
        }
        public static  float InSine (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Cos(t * MathF.PI / 2);
        }

        public static  float OutSine (float t)
        {
            t = Clamp(t);
            return MathF.Sin(t * MathF.PI / 2);
        }

        public static  float InOutSine (float t)
        {
            t = Clamp(t);
            return -(MathF.Cos(MathF.PI * t) - 1) / 2;
        }

        public static  float InQuad (float t)
        {
            t = Clamp(t);
            return t * t;
        }

        public static  float OutQuad (float t)
        {
            t = Clamp(t);
            return 1 - (1 - t) * (1 - t);
        }

        public static  float InOutQuad (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
        }

        public static  float InCubic (float t)
        {
            t = Clamp(t);
            return t * t * t;
        }

        public static  float OutCubic (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 3);
        }

        public static  float InOutCubic (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 4 * t * t * t : 1 - MathF.Pow(-2 * t + 2, 3) / 2;
        }

        public static  float InQuart (float t)
        {
            t = Clamp(t);
            return t * t * t * t;
        }

        public static  float OutQuart (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 4);
        }

        public static  float InOutQuart (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 8 * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 4) / 2;
        }

        public static  float InQuint (float t)
        {
            t = Clamp(t);
            return t * t * t * t * t;
        }

        public static  float OutQuint (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Pow(1 - t, 5);
        }

        public static  float InOutQuint (float t)
        {
            t = Clamp(t);
            return t < 0.5f ? 16 * t * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 5) / 2;
        }

        public static  float InExpo (float t)
        {
            t = Clamp(t);
            const float p = 10;
            return t == 0 ? 0 : MathF.Pow(2, p * t - p);
        }

        public static  float OutExpo (float t)
        {
            const float p = 10;
            t = Clamp(t);
            return t == 0 ? t : 1 - MathF.Pow(2, -p * t);
        }

        public static  float InOutExpo (float t)
        {
            const float p = 10;
            t = Clamp(t);
            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f ? MathF.Pow(2, p * 2 * t - p) / 2
                : (2 - MathF.Pow(2, -(p * 2) * t + p)) / 2;
        }

        public static  float InOutExpo7 (float t)
        {
            const float p = 7;
            t = Clamp(t);
            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f ? MathF.Pow(2, p * 2 * t - p) / 2
                : (2 - MathF.Pow(2, -(p * 2) * t + p)) / 2;
        }

        public static  float InCirc (float t)
        {
            t = Clamp(t);
            return 1 - MathF.Sqrt(1 - MathF.Pow(t, 2));
        }

        public static  float OutCirc (float t)
        {
            t = Clamp(t);
            return MathF.Sqrt(1 - MathF.Pow(t - 1, 2));
        }


        public static  float InOutCirc (float t)
        {
            t = Clamp(t);
            return t < 0.5f
                ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * t, 2))) / 2
                : (MathF.Sqrt(1 - MathF.Pow(-2 * t + 2, 2)) + 1) / 2;
        }


        public static  float InBack (float t)
        {
            t = Clamp(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * t * t * t - c1 * t * t;
        }


        public static  float OutBack (float t)
        {
            t = Clamp(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(t - 1, 3) + c1 + MathF.Pow(t - 1, 2);
        }


        public static  float InOutBack (float t)
        {
            t = Clamp(t);

            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f
                ? MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2) / 2
                : (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }

        public static  float InElastic (float t)
        {
            t = Clamp(t);

            const float c4 = 2 * MathF.PI / 3;
            return t == 0
                ? 0
                : t == 1
                ? 1
                : -MathF.Pow(2, 10 * t - 10) * MathF.Sin((t * 10 - 10.75f) * c4);
        }

        public static  float OutElastic (float t)
        {
            t = Clamp(t);
            const float c4 = 2 * MathF.PI / 3;
            return t == 0
                ? 0
                : t == 1
                ? 1
                : MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * c4) + 1;
        }

        public static  float InOutElastic (float t)
        {
            t = Clamp(t);
            const float c5 = 2 * MathF.PI / 4.5f;

            return t == 0
                ? 0
                : t == 1
                ? 1
                : t < 0.5f
                ? -(MathF.Pow(2, 20 * t - 10) * MathF.Sin((20 * t - 11.125f) * c5)) / 2
                : MathF.Pow(2, -20 * t + 10) * MathF.Sin((20 * t - 11.125f) * c5) / 2 + 1;
        }

        public static  float InBounce (float t)
        {
            t = Clamp(t);
            return 1 - OutBounce(1 - t);
        }

        public static  float OutBounce (float t)
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

        public static  float InOutBounce (float t)
        {
            t = Clamp(t);
            return t < 0.5f
                ? (1 - OutBounce(1 - 2 * t)) / 2
                : (1 + OutBounce(2 * t - 1)) / 2;
        }

        public static  float SinBump (float t)
        {
            t = Clamp(t);
            const float factor = 2;
            return t * (factor - t * factor);
        }

        public static  float TriBump (float t)
        {
            t = Clamp(t);
            const float factor = 3;
            return t * (factor - t * factor);
        }

        public static  float QuadraticBump (float t)
        {
            t = Clamp(t);
            const float factor = 4;
            return t * (factor - t * factor);
        }
        public static float GreatswordAnticipation(float t)
        {
            float easeIn = MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(t / 0.5f));
            float easeOut = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(t));
            return MathHelper.Lerp(0f, 1f, easeOut * easeIn);
        }
        public static float GreatswordSpinAnticipation(float t)
        {
            t = Clamp(t);
            Vector2 control1 = new Vector2(0.8f, -0.4f);
            Vector2 control2 = new Vector2(0.5f, 1f);
            float ease = EasingFunction.BezierEase(t, control1, control2);
            return MathHelper.Lerp(0f, 1f, ease * EasingFunction.OutExpo(t));
        }

        public static float QuickOutSlowIn(float t)
        {
            float easeIn = EasingFunction.OutExpo(t);
            float easeOut = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(t));
            return easeIn * easeOut;
        }
        public static float QuickInLinear(float t)
        {
            float easeIn = EasingFunction.InCirc(t / 0.5f);
            float newTime = MathHelper.Lerp(0f, t, easeIn);
            return newTime;
        }

        public static  float QuadraticBumpP05 (float t)
        {
            t = Clamp(t);
            const float factor = 4;
            return MathF.Pow(t * (factor - t * factor), 0.5f);
        }
    }
}

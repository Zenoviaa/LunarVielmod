using Microsoft.Xna.Framework;
using Stellamod.Common.Camera;
using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Helpers
{
    internal static class FXUtil
    {
        public static GlowCircleLongBoomParticle GlowCircleLongBoom(Vector2 position, Color innerColor, Color glowColor, Color outerGlowColor, float duration = 15f, float baseSize = 0.12f)
        {
            GlowCircleLongBoomParticle boomParticle = Particle.NewParticle<GlowCircleLongBoomParticle>(position, Vector2.Zero);
            //The inside color of the circle
            boomParticle.InnerColor = innerColor;

            //The main color it fades to
            boomParticle.GlowColor = glowColor;

            //The final color it fades to
            boomParticle.OuterGlowColor = outerGlowColor;

            //How long to last
            boomParticle.Duration = duration;

            //How big the circle is, don't make it too big or it'll go outside the square
            boomParticle.BaseSize = baseSize;
            boomParticle.Pixelation = 0.0015f;
            return boomParticle;
        }

        public static GlowCircleBoomParticle GlowCircleBoom(Vector2 position, Color innerColor, Color glowColor, Color outerGlowColor, float duration = 15f, float baseSize = 0.12f)
        {
            GlowCircleBoomParticle boomParticle = Particle.NewParticle<GlowCircleBoomParticle>(position, Vector2.Zero);
            //The inside color of the circle
            boomParticle.InnerColor = innerColor;

            //The main color it fades to
            boomParticle.GlowColor = glowColor;

            //The final color it fades to
            boomParticle.OuterGlowColor = outerGlowColor;

            //How long to last
            boomParticle.Duration = duration;

            //How big the circle is, don't make it too big or it'll go outside the square
            boomParticle.BaseSize = baseSize;
            boomParticle.Pixelation = 0.0015f;
            return boomParticle;
        }
        public static void FocusCamera(Vector2 position, float duration, string uniqueIdentity = null)
        {
            FocusCameraModifier focusCameraModifier = new FocusCameraModifier(position, duration, uniqueIdentity);
            Main.instance.CameraModifiers.Add(focusCameraModifier);
        }

        public static void FocusCamera(Entity entity, float duration, string uniqueIdentity = null)
        {
            FocusCameraModifier focusCameraModifier = new FocusCameraModifier(entity, duration, uniqueIdentity);
            Main.instance.CameraModifiers.Add(focusCameraModifier);
        }
        public static void ShakeCamera(Vector2 position, float distance, float strength, string uniqueIdentity = null)
        {
            ScreenshakeCameraModifier screenshakeCameraModifier = new ScreenshakeCameraModifier(
                position,
                distance,
                strength,
                uniqueIdentity);
            Main.instance.CameraModifiers.Add(screenshakeCameraModifier);
        }

        public static void PunchCamera(Vector2 startPosition, Vector2 direction, float strength, float vibrationCyclesPerSecond, int frames, float distanceFallOff = -1, string uniqueIdentity = null)
        {
            PunchCameraModifier punchCameraModifier = new PunchCameraModifier(
                startPosition,
                direction,
                strength,
                vibrationCyclesPerSecond,
                frames,
                distanceFallOff,
                uniqueIdentity);
            Main.instance.CameraModifiers.Add(punchCameraModifier);
        }
    }
}

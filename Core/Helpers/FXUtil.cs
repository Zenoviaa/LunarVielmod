using Microsoft.Xna.Framework;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Visual.Particles;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Core.Helpers
{
    internal static class FXUtil
    {
        public static FogParticle Fog(Vector2 position, Vector2 velocity)
        {
            FogParticle particle = Particle.NewParticle<FogParticle>(position, Vector2.Zero);
            particle.Velocity = velocity;
            return particle;
        }

        public static GlowStretchParticle GlowStretch(Vector2 position, Vector2 velocity)
        {
            GlowStretchParticle particle = Particle.NewParticle<GlowStretchParticle>(position, Vector2.Zero);
            particle.Velocity = velocity;
            //The inside color of the circle
            particle.InnerColor = Color.White;

            //The main color it fades to
            particle.GlowColor = Color.LightCyan;

            //The final color it fades to
            particle.OuterGlowColor = Color.DarkBlue;

            //How long to last
            particle.Duration = 15;

            //How big the circle is, don't make it too big or it'll go outside the square
            particle.BaseSize = 0.06f;
            particle.Pixelation = 0.0015f;
            return particle;
        }

        public static GlowSpikeParticle GlowSpikeBoom(Vector2 position, Color innerColor, Color glowColor, Color outerGlowColor, float duration = 15f, float baseSize = 0.12f)
        {
            GlowSpikeParticle boomParticle = Particle.NewParticle<GlowSpikeParticle>(position, Vector2.Zero);
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
        public static GlowCircleDetailedBoomParticle1 GlowCircleDetailedBoom1(Vector2 position, Color innerColor, Color glowColor, Color outerGlowColor, float duration = 15f, float baseSize = 0.12f)
        {
            GlowCircleDetailedBoomParticle1 boomParticle = Particle.NewParticle<GlowCircleDetailedBoomParticle1>(position, Vector2.Zero);
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
        public static void SnapFocusCamera(Vector2 position, float duration, string uniqueIdentity = null)
        {
            SnapFocusCameraModifier focusCameraModifier = new SnapFocusCameraModifier(position, duration, uniqueIdentity);
            Main.instance.CameraModifiers.Add(focusCameraModifier);
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

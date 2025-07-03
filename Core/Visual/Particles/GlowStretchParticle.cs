using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Core.Visual.Particles
{
    internal class GlowStretchParticle : Particle
    {
        private GlowCircleShader _shader;
        public float Timer;
        public float Duration;
        public float Progress;
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public int MaxFrameCount = 1;

        public Color InnerColor;
        public Color GlowColor;
        public Color OuterGlowColor;
        public Vector2 VectorScale;
        public float BasePower = 0.5f;
        public float BaseSize = 0.05f;
        public float Pixelation = 1f;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            VectorScale = new Vector2(Scale * 2, Scale * Main.rand.NextFloat(0.5f, 5f));
            Duration = 15;
            InnerColor = Color.White;
            GlowColor = Color.Yellow;
            OuterGlowColor = Color.Red;
            BasePower = 0.5f;
            BaseSize = 0.025f;
            Pixelation = 1f;
            customShader = _shader = new GlowCircleShader();
        }

        public override void Update()
        {
            Timer++;
            Progress = Timer / Duration;
            Velocity *= 0.9f;
            Rotation = Velocity.ToRotation();
            VectorScale *= 0.96f;
            color *= 0.99f;
            if (Timer >= Duration)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            GlowCircleShader shader = _shader;
            shader.Speed = 5;

            float bp = BasePower;
            shader.BasePower = MathHelper.Lerp(bp, bp * 2, EasingFunction.QuadraticBump(Progress));

            float s = BaseSize;
            shader.Size = MathHelper.Lerp(s, s * 2, EasingFunction.QuadraticBump(Progress));

            Color startInner = InnerColor;
            Color startGlow = GlowColor;
            Color startOuterGlow = OuterGlowColor;

            Color endColor = startOuterGlow;


            shader.InnerColor = Color.Lerp(startInner, startGlow, Progress);
            shader.GlowColor = Color.Lerp(startGlow, startOuterGlow, Progress);
            shader.OuterGlowColor = Color.Lerp(startOuterGlow, Color.Black, Progress);


            shader.InnerColor = Color.Lerp(shader.InnerColor, Color.Black, Progress);
            shader.GlowColor = Color.Lerp(shader.GlowColor, Color.Black, Progress);
            shader.OuterGlowColor = Color.Lerp(shader.OuterGlowColor, Color.Black, Progress);
            shader.Pixelation = Pixelation;

            shader.ApplyToEffect();

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(GetTexture().Value, centerPos, null, Color.White, Rotation, GetTexture().Size() / 2f, VectorScale, SpriteEffects.None, 0);
            }
        }


    }
}

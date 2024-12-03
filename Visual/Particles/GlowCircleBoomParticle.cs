using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class GlowCircleBoomParticle : Particle
    {
        public float Timer;
        public float Duration;
        public float Progress;
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public int MaxFrameCount = 1;

        public Color InnerColor;
        public Color GlowColor;
        public Color OuterGlowColor;

        public float BasePower = 0.5f;
        public float BaseSize = 0.05f;
        public float Pixelation = 1f;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            Duration = 15;
            InnerColor = Color.White;
            GlowColor = Color.Yellow;
            OuterGlowColor = Color.Red;
            BasePower = 0.5f;
            BaseSize = 0.025f;
            Pixelation = 1f;
        }

        public override void Update()
        {
            Timer++;
            Progress = Timer / Duration;
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            color *= 0.99f;
            if (Timer >= Duration)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;
            shader.Speed = 5;

            float bp = BasePower;
            shader.BasePower = MathHelper.Lerp(bp, bp * 2, Easing.SpikeOutCirc(Progress));

            float s = BaseSize;
            shader.Size = MathHelper.Lerp(s, s * 2, Easing.SpikeOutCirc(Progress));

            Color startInner = InnerColor;
            Color startGlow = GlowColor;
            Color startOuterGlow = OuterGlowColor;

            Color endColor = startOuterGlow;


            shader.InnerColor = Color.Lerp(startInner, startGlow, Progress);
            shader.GlowColor = Color.Lerp(startGlow, startOuterGlow, Progress);
            shader.OuterGlowColor = Color.Lerp(startOuterGlow, Color.Black, Progress);
            shader.Pixelation = Pixelation;
            shader.Apply();

            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(GetTexture().Value, centerPos, null, Color.White, Rotation, GetTexture().Size() / 2f, Scale, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

     
    }
}

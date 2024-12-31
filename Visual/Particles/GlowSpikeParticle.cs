using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class GlowSpikeParticle : Particle
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
        public float Height;
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
            Height = 1.0f;
            customShader = GlowPillarShader.Instance; 
        }

        public override void Update()
        {
            Timer++;
            Progress = Timer / Duration;
            Velocity *= 0.98f;
            Scale *= 0.997f;
            color *= 0.99f;
            if (Timer >= Duration)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            GlowPillarShader shader = GlowPillarShader.Instance;
            shader.Speed = 5;

            float bp = BasePower;
            shader.BasePower = MathHelper.Lerp(bp, bp * 2, Easing.SpikeOutExpo(Progress));

            float s = BaseSize;
            shader.Size = MathHelper.Lerp(0, s * 2, Easing.SpikeOutExpo(Progress));

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
            shader.Height = MathHelper.Lerp(0, Height, Easing.SpikeOutCirc(Progress));
            shader.Apply();
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(GetTexture().Value, centerPos, null, Color.White, Rotation, GetTexture().Size() / 2f, Scale, SpriteEffects.None, 0);
            }
        }
    }
}
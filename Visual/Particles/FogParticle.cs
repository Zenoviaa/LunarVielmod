using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class FogParticle : Particle
    {
        public float Timer;
        public float Duration;
        public float Progress;
        public float Dir;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public Color startColor;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            Duration = 120;
            Dir = Main.rand.NextFloat(-1f, 1f);
            customShader = FoggyShader.Instance;
            color = Color.DarkGray;
        }

        public override void Update()
        {
            Timer++;
            if (Timer == 1)
            {
                startColor = color;
            }
            Progress = Timer / Duration;
            Velocity.X *= 0.9f;
            Velocity.Y -= 0.05f;
            Rotation += 0.015f * Dir;
            Scale *= 0.997f;
            color = Color.Lerp(Color.Transparent, startColor, Easing.SpikeOutCirc(Progress));
            if (Timer >= Duration)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            FoggyShader shader = FoggyShader.Instance;

            shader.SourceSize = GetTexture().Size();
            shader.Apply();
            spriteBatch.Draw(GetTexture().Value, centerPos, null, color, Rotation, GetTexture().Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
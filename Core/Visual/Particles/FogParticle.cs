using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Core.Visual.Particles
{
    internal class FogParticle : Particle
    {
        private FoggyShader _shader;
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
            customShader = _shader = new();
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
            color = Color.Lerp(Color.Transparent, startColor, EasingFunction.QuadraticBump(Progress));
            if (Timer >= Duration)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            FoggyShader shader = _shader;

            shader.SourceSize = GetTexture().Size();
            shader.ApplyToEffect();
            spriteBatch.Draw(GetTexture().Value, centerPos, null, color, Rotation, GetTexture().Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
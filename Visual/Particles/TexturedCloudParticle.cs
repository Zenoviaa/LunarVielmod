using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class TexturedCloudParticle : Particle<TexturedCloudParticle>
    {
        private float Alpha;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            Rotation = Main.rand.NextFloat(0f, 3f);
        }

        public override void Update()
        {
            Scale *= 1.01f;
            color *= 0.99f;
            Alpha = EasingFunction.QuadraticBump(fadeIn / 90f);
            fadeIn++;
            if (fadeIn > 180)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            Vector2 scale = Vector2.One * Scale;
            spriteBatch.Draw(GetTexture().Value, centerPos, Frame, color * 0.65f * Alpha, Rotation, Frame.Size() / 2f, scale, SpriteEffects.None, 0);
        }
    }
}

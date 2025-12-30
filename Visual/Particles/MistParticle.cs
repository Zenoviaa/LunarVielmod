using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class MistParticle : LegacyParticle
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 3;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        }

        public override void Update()
        {
            Scale *= 0.98f;
            color *= 0.99f;
            fadeIn++;
            if (fadeIn > 180)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            Vector2 scale = Vector2.One * Scale;
            scale.X *= 2;
            spriteBatch.Draw(GetTexture().Value, centerPos, Frame, color * 0.3f, Rotation, Frame.Size() / 2f, scale, SpriteEffects.None, 0);
        }
    }
}

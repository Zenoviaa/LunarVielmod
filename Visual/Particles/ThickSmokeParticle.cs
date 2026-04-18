using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    /// <summary>
    /// Fun particle for creating nice thick smoke
    /// </summary>
    public class ThickSmokeParticle : Particle<ThickSmokeParticle>
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public bool expand;
        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(0, 3.14f);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.65f, 2);
            color = Color.White;
            Rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            expand = false;
        }

        public override void Update()
        {
            Velocity.Y -= 0.05f;
            Velocity *= 0.9f;
            if (expand)
            {
                Scale *= 1.01f;
            }
            else
            {
                Scale *= 0.98f;
            }
 
            color *= 0.99f;
            fadeIn++;
            if (fadeIn > 180)
                active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var textureAsset = GetTexture();
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, Color.Lerp(color, Color.Black, 0.85f), Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
    public class FaintSmokeParticle : Particle<FaintSmokeParticle>
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public bool expand;
        public Color fadeToColor;
        public float alpha;
        public override void OnSpawn()
        {
            alpha = 1f;
            Rotation = Main.rand.NextFloat(0, 3.14f);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.65f, 2);
            color = Color.White;
            Rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            expand = false;
        }

        public override void Update()
        {
            Velocity.Y -= 0.05f;
            Velocity *= 0.9f;
            if (expand)
            {
                Scale *= 1.01f;
            }
            else
            {
                Scale *= 0.96f;
            }

            alpha *= 0.98f;
            fadeIn++;
            if (fadeIn > 180)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var textureAsset = GetTexture();

            Color drawColor = Color.Lerp(color, fadeToColor, fadeIn / 60f);
            drawColor *= alpha;
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, drawColor, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

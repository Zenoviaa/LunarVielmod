using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class LightningSparkParticle : Particle<LightningSparkParticle>
    {
        public int FrameWidth = 256;
        public int FrameHeight = 256;
        public int MaxFrameCount = 1;
        public float gravity;
        public Vector2 stretchScale;
        public float dampening;
        public bool fast;
        public float rotationOffset;
        public override void OnSpawn()
        {
            rotationOffset = Main.rand.NextFloat(0f, 3.14f);
            gravity = 0.2f;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            //    customShader = DustShader.Instance;
        }

        public override void Update()
        {
            Velocity.Y += gravity;
            Velocity *= 1.0f - dampening;
            Rotation = Velocity.ToRotation() + rotationOffset;
            Scale *= 0.92f;
            if (fast)
                Scale *= 0.98f;
            color *= 0.99f;

            float stretchInterp = Velocity.Length() / 5f;
            stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
            stretchScale.Y = 1f;
            fadeIn++;
            if (fadeIn > 180 || Scale < 0.01f)
                active = false;

            //Bouncing
            Vector2 collisionVelocity = Collision.TileCollision(Center, Velocity, 2, 2);
            if (Velocity.X != collisionVelocity.X)
                Velocity.X = -collisionVelocity.X;
            if (Velocity.Y != collisionVelocity.Y)
                Velocity.Y = -collisionVelocity.Y;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var textureAsset = GetTexture();

            Color drawColor = color;

            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, drawColor, Rotation, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }
    }
}

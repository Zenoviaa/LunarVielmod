using Stellamod.Core.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Visual.Particles
{

    public class RosePetalParticle : Particle<RosePetalParticle>
    {
        public int FrameWidth = 18;
        public int FrameHeight = 16;
        public int MaxFrameCount = 3;
        public float gravity;
        public Vector2 stretchScale;
        public float dampening;
        public bool fast;
        public override void OnSpawn()
        {
            gravity = 0.2f;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        }

        public override void Update()
        {
            Velocity.Y += gravity;
            Velocity *= 1.0f - dampening;
            Rotation = Velocity.ToRotation() + fadeIn * 0.001f;
            Scale *= 0.97f;
            if (fast)
                Scale *= 0.98f;

            fadeIn++;
            if (fadeIn > 180 || Scale < 0.1f)
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
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, drawColor, Rotation + MathHelper.PiOver2, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

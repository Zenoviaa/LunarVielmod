using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class BubbleParticle : Particle<BubbleParticle>
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public float gravity;
        public Vector2 stretchScale;
        public float dampening;
        public bool fast;
        public bool longBubble;
        public override void OnSpawn()
        {
            longBubble = false;
            fast = false;
            gravity = 0.2f;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            //    customShader = DustShader.Instance;
        }

        public override void Update()
        {
            Velocity.Y += gravity;
            Velocity *= 1.0f - dampening;
            Rotation = Velocity.ToRotation();
            if(longBubble)
            {
                Scale *= 0.999f;
                if (fast)
                    Scale *= 0.989f;
                color *= 0.999f;

            }
            else
            {
                Scale *= 0.99f;
                if (fast)
                    Scale *= 0.98f;
                color *= 0.99f;

            }

            float stretchInterp = Velocity.Length() / 5f;
            stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
            stretchScale.Y = 1f;
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
            Vector2 targetScale = Scale * stretchScale;
            Vector2 scale = Vector2.Lerp(Vector2.Zero, targetScale, EasingFunction.OutExpo(fadeIn / 30f));
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, drawColor, Rotation + MathHelper.PiOver2, Frame.Size() / 2f, scale, SpriteEffects.None, 0);
        }
    }
}

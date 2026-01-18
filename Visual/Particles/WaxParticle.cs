using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class WaxParticle : Particle<WaxParticle>
    {
        private float _ratio;
        public int FrameWidth = 30;
        public int FrameHeight = 32;
        public int MaxFrameCount = 3;
        public Color innerColor;
        public Color bloomColor;
        public float time;
        public float gravity;
        public override void OnSpawn()
        {
            gravity = 0.06f;
            time = Main.rand.Next(60, 120);
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            Rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        }

        public override void Update()
        {
            fadeIn++;
            _ratio = fadeIn / time;
            if (fadeIn >= time)
                active = false;
            Velocity.Y += gravity;

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
            float scale = Scale;
            scale *= MathHelper.SmoothStep(1f, 0f, _ratio);
            spriteBatch.Draw(GetTexture().Value, centerPos, Frame, color, Rotation, Frame.Size() / 2f, scale, SpriteEffects.None, 0);
        }
    }
}

using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class LetterParticle : Particle<LetterParticle>
    {
        private float _ratio;
        public int FrameWidth = 18;
        public int FrameHeight = 18;
        public int MaxFrameCount = 3;
        public Color innerColor;
        public Color bloomColor;
        public float time;
        public float gravity;
        public override void OnSpawn()
        {
            gravity = 0.06f;
            time = Main.rand.Next(30, 60);
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
            Color drawColor = color;
            drawColor = drawColor.MultiplyRGB(Lighting.GetColor(Center.ToTileCoordinates()));
            spriteBatch.Draw(GetTexture().Value, centerPos, Frame, drawColor, Rotation, Frame.Size() / 2f, scale, SpriteEffects.None, 0);
        }
    }
}

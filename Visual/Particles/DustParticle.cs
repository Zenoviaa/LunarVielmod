using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class DustParticle : Particle
    {
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public int MaxFrameCount = 3;
        public float gravity;
        public Color innerColor;
        public Color outerColor;
        public Vector2 stretchScale;
        public override void OnSpawn()
        {
            gravity = 0.2f;
            innerColor = Color.White;
            outerColor = Color.Red;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            customShader = DustShader.Instance;
        }

        public override void Update()
        {
            Velocity.Y += gravity;
            Rotation = Velocity.ToRotation();
            Scale *= 0.98f;
            color *= 0.99f;

            float stretchInterp = Velocity.Length() / 5f;
            stretchScale.X = MathHelper.Lerp(1f, 2f, stretchInterp);
            stretchScale.Y = 1f;
            fadeIn++;
            if (fadeIn > 180)
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
            DustShader shader = DustShader.Instance;
            shader.InnerColor = innerColor;
            shader.OuterColor = outerColor;
            shader.Apply();
            spriteBatch.Draw(GetTexture().Value, centerPos, Frame, Color.White, Rotation, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }

    }
}

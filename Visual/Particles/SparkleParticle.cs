using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class SparkleParticle : Particle<SparkleParticle>
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public float gravity;
        public Color innerColor;
        public Color outerColor;
        public float dampening;
        public bool fast;
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
            Velocity *= 1.0f - dampening;
            Rotation = 0;
            Scale *= 0.97f;
            if (fast)
                Scale *= 0.98f;
            color *= 0.99f;


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
            DustShader shader = DustShader.Instance;
            shader.InnerColor = innerColor;
            shader.OuterColor = outerColor;
            shader.Apply();

            var textureAsset = GetTexture();
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, Color.White, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

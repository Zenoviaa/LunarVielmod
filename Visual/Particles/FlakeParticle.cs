using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public struct FlakeParticleSpawnParams
    {
        public FlakeParticleSpawnParams()
        {
            scaleRange = new Vector2(0.5f, 2f);
            gravity = 0.2f;
        }
        public Vector2 scaleRange;
        public float gravity;
        public static FlakeParticleSpawnParams Default = new FlakeParticleSpawnParams();
    }

    public class FlakeParticle : Particle<FlakeParticle>
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public float gravity;
        public Vector2 stretchScale;
        public float dampening;
        public bool fast;
        public override void OnSpawn()
        {
            stretchScale = Vector2.One;
            gravity = 0.2f;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            //    customShader = DustShader.Instance;
        }
        public static FlakeParticle Spawn(Vector2 position, Vector2 velocity, FlakeParticleSpawnParams? spawnParams = null)
        {
            if (!spawnParams.HasValue)
                spawnParams = new FlakeParticleSpawnParams();
            FlakeParticleSpawnParams settings = spawnParams.Value;
            float scale = Main.rand.NextFloat(settings.scaleRange.X, settings.scaleRange.Y);
            FlakeParticle dp = Spawn(position, velocity, Color.White, scale);
            dp.gravity = settings.gravity;
            return dp;
        }

        public override void Update()
        {
            Velocity.Y += gravity;
            Velocity *= 1.0f - dampening;
            Rotation = Velocity.ToRotation();
            Scale *= 0.97f;
            if (fast)
                Scale *= 0.98f;
            color *= 0.99f;

            float stretchInterp = Velocity.Length() / 5f;
 
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
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, drawColor * 0.8f, Rotation + MathHelper.PiOver2, Frame.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }
    }

}

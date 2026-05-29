using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
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
        public bool flickering;
        public bool easeInFade;
        public bool noTileCollide;
        public override void OnSpawn()
        {
            flickering = false;
            dampening = 0;
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
            if (fadeIn > 180 || Scale < 0.05f)
                active = false;

            if (noTileCollide)
                return;

            //Bouncing
            Vector2 collisionVelocity = Collision.TileCollision(Center, Velocity, 2, 2);
            if (Velocity.X != collisionVelocity.X)
                Velocity.X = -collisionVelocity.X;
            if (Velocity.Y != collisionVelocity.Y)
                Velocity.Y = -collisionVelocity.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = DrawPosition;
            DustShader shader = DustShader.Instance;
            shader.InnerColor = innerColor;
            shader.OuterColor = outerColor;
            shader.Apply();

            var textureAsset = GetTexture();

            Color color = Color.White;
            if (flickering)
            {
                color *= ExtraMath.Osc(0f, 1f, speed: 8, 0);
            }
            if (easeInFade)
            {
                color *= EasingFunction.InOutSine(fadeIn / 30f);
            }
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

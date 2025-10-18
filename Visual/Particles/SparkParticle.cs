using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class SparkParticle : Particle
    {
        private float _interpolant;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;

        public Color innerColor;
        public Color outerColor;
        public Color fadeToColor;
        public float power;
        public Vector2 stretchScale;

        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(0, 3.14f);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.06f, 0.124f);
            customShader = SparkShader.Instance;

            innerColor = Color.White;
            outerColor = Color.Yellow;
            fadeToColor = Color.DarkBlue;
            color = Color.White;
        }

        public override void Update()
        {
            Velocity *= 0.935f;
            Rotation = Velocity.ToRotation();
            color *= 0.995f;
            Scale *= 0.995f;
            fadeIn++;
            if(fadeIn == 1)
            {
                power = 0.25f;
            }

            if(fadeIn == 10)
            {
                power = 0.25f;
            }
            if (fadeIn == 25)
            {
                power = 0.25f;
            }
            power = MathHelper.Lerp(power, 2, 0.05f);
            _interpolant = fadeIn / 60;
            if (fadeIn > 120)
                active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var shader = SparkShader.Instance;
            shader.InnerColor = innerColor;
            shader.OuterColor = Color.Lerp(outerColor, fadeToColor, _interpolant);
            shader.Power = power;
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale , SpriteEffects.None, 0);
            if(Main.rand.NextBool(4))
                spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale, SpriteEffects.None, 0);
            if (Main.rand.NextBool(4))
                spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

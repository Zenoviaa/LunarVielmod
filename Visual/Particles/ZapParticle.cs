using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class ZapParticle : LegacyParticle
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
            Scale = Main.rand.NextFloat(0.2f, 1);
            stretchScale.X = Main.rand.NextFloat(0.5f, 2.5f);
            stretchScale.Y = 1;
            customShader = RadiantShader.Instance;

            innerColor = Color.White;
            outerColor = Color.Yellow;
            fadeToColor = Color.Blue;
            color = Color.White;
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Rotation = Velocity.ToRotation();
            color *= 0.995f;
            fadeIn++;
            if(fadeIn == 1)
            {
                power = 0.5f;
            }

            if(fadeIn == 10)
            {
                power = 0.9f;
            }
            if (fadeIn == 25)
            {
                power = 0.5f;
            }
            power = MathHelper.Lerp(power, 10, 0.05f);
            _interpolant = fadeIn / 60;
            if (fadeIn > 60)
                active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var shader = RadiantShader.Instance;
            shader.InnerColor = Color.Lerp(innerColor, outerColor, _interpolant);
            shader.OuterColor = outerColor;
            shader.Power = power;
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }
    }
}

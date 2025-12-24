using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class FlareParticle : LegacyParticle
    {
        private float _offset;
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
            Scale = Main.rand.NextFloat(0.65f, 1);
            _offset = Main.rand.NextFloat(0f, 10f);
            stretchScale = Vector2.One;
            customShader = FlareShader.Instance;

            innerColor = Color.OrangeRed;
            outerColor = Color.Orange;
            fadeToColor = Color.Red;
            color = Color.White;
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Rotation = Velocity.ToRotation();
            color *= 0.98f;
            fadeIn++;
            _interpolant = fadeIn / 60;
            if (fadeIn > 180)
                active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var shader = FlareShader.Instance;
            shader.InnerColor = Color.Lerp(innerColor, fadeToColor, _interpolant);
            shader.OuterColor = Color.Lerp(outerColor, fadeToColor, _interpolant);
            shader.Power = MathHelper.Lerp(1.0f, 3f, _interpolant);
            shader.Time = Main.GlobalTimeWrappedHourly * 5 + _offset;
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }
    }
}

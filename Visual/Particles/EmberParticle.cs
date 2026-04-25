using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class AmbientEmberParticle : EmberParticle
    {
        public override void OnSpawn()
        {
            base.OnSpawn();
            color = Color.Transparent;
            Rotation += Main.rand.NextFloat(0f, 3.14f);
        }

        public override void Update()
        {

            fadeIn++;
            _interpolant = fadeIn / 240f;
            float a = EasingFunction.QuadraticBump(_interpolant);
            color =  Color.White * a;
            if (fadeIn > 300)
                active = false;
        }
    }
    public class EmberParticle : LegacyParticle
    {
        private float _randOffset;
        private Vector2 _stretchScale;
        protected float _interpolant;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;

        public Color innerColor;
        public Color outerColor;
        public Color fadeToColor;
        public bool isLong;
        public override void OnSpawn()
        {
            isLong
                = false;
       
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.3f, 0.6f);
            customShader = RadiantShader.Instance;
            _stretchScale = Vector2.One;
            _randOffset = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            innerColor = Color.Yellow;
            outerColor = Color.Red;
            fadeToColor = Color.Blue;
            color = Color.White;
        }

        public override void Update()
        {
            if (isLong)
            {
                color *= 0.99f;
                Rotation = Velocity.ToRotation() + _randOffset;
                fadeIn++;
                _interpolant = fadeIn / 220f;
                if (fadeIn > 280f)
                    active = false;
            }
            else
            {
                color *= 0.98f;
                Rotation = Velocity.ToRotation();
                fadeIn++;
                _interpolant = fadeIn / 120f;
                if (fadeIn > 180f)
                    active = false;
            }
      


            Velocity.X *= 0.95f;
            Velocity.Y -= 0.02f;

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var shader = RadiantShader.Instance;
            shader.InnerColor = Color.Lerp(innerColor, outerColor, _interpolant);
            shader.OuterColor = outerColor;
            shader.Power = MathHelper.Lerp(0.5f, 20f, EasingFunction.InOutSine(_interpolant));
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale * _stretchScale, SpriteEffects.None, 0);
        }
    }
}

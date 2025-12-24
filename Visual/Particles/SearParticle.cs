using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class SearParticle : Particle
    {
        private float _interpolant;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;

        public Color innerColor;
        public Color outerColor;
        public Color fadeToColor;

        public override void OnSpawn()
        {
            Rotation = 0;
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.8f, 1f);
            customShader = RadiantShader.Instance;

            innerColor = Color.Yellow;
            outerColor = Color.Red;
            fadeToColor = Color.Blue;
            color = Color.White;
        }

        public override void Update()
        {
            color *= 0.998f;
            fadeIn++;
            _interpolant = fadeIn / 120f;
            if (fadeIn > 180)
                active = false;
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
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

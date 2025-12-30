using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using System;
using Terraria;
namespace Stellamod.Visual.Particles
{
    public class GlowDonutParticle : LegacyParticle
    {
        private float _direction;
        private Vector2 _stretchScale;
        private bool _fast;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;

        public Color innerColor;
        public Color outerColor;
        public Color fadeToColor;
        private float _interpolant;
        public bool distortOut;
        public float downwardPull;
        public bool shrink;
        public bool noStretch;
        public float xMult;
        public float rotOffset;
        public override void OnSpawn()
        {
            shrink = false;
            _stretchScale = new Vector2(1f, 0.2f);
            Rotation = Main.rand.NextFloat(0f, 3.14f);
            distortOut = true;
            _direction = Main.rand.NextFloat(-1, 2);

            Frame = new Rectangle(0, 0, 128, 128);
            Scale = Main.rand.NextFloat(0.8f, 1) * 2;
            customShader = GlowDonutShader.Instance;
            color = Color.White;
            xMult = 1f;
            innerColor = Color.White;
            outerColor = Color.LightGray;
            fadeToColor = Color.DarkBlue;
        }

        public override void Update()
        {
            color *= Main.rand.NextFloat(0.98f, 1f); ;
            if (noStretch)
            {
                _stretchScale = Vector2.One;
            }
            _stretchScale.X = xMult;
            if (shrink)
            {
                Scale *= 0.9f;

            }
            else
            {
                if (Scale < 2)
                    Scale *= 1.05f;
                else
                    Scale *= 1.02f;
            }

            Velocity *= 0.99f;
            Rotation = Velocity.ToRotation() - MathHelper.PiOver2;
            Rotation += rotOffset;
            fadeIn++;
            _interpolant = fadeIn / 30f;
            _interpolant = Math.Clamp(_interpolant, 0, 1);
            if (fadeIn > 120)
                active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            GlowDonutShader shader = GlowDonutShader.Instance;
            shader.InnerColor = Color.Lerp(innerColor, fadeToColor, _interpolant);
            shader.OuterGlowColor = Color.Lerp(outerColor, fadeToColor, _interpolant);
            shader.Tiling = Vector2.One * 1;
            if (distortOut)
                shader.Distortion = MathHelper.Lerp(0f, 0.025f, _interpolant);
            shader.Power = MathHelper.Lerp(0.25f, 5, EasingFunction.InOutExpo(_interpolant)); ;
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();


            Vector2 drawScale = Scale * _stretchScale;
            drawScale *= MathHelper.Lerp(0.5f, 1f, EasingFunction.OutExpo(_interpolant));
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, drawScale, SpriteEffects.None, 0);
        }
    }
}

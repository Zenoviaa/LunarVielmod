using Humanizer.Bytes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Igniters;
using Terraria;
using ReLogic.Content;
using Stellamod.Trails;
namespace Stellamod.Visual.Particles
{
    public class GlowFragmentParticle : Particle
    {
        private float _direction;
        private Vector2 _stretchScale;
        private bool _fast;
        public int FrameWidth = 72;
        public int FrameHeight = 72;
        public int MaxFrameCount = 1;

        public Color innerColor;
        public Color outerColor;
        public Color fadeToColor;
        private float _interpolant;
        public bool distortionInterp;
        public bool distortOut;
        public bool gravity;
        public float downwardPull;
        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(0f, 3.14f);
            gravity = true;
            _direction = Main.rand.NextFloat(-1, 2);
            _stretchScale = Vector2.One;
            _fast = Main.rand.NextBool(2);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.8f, 1f);
            customShader = GlowFragmentShader.Instance;
            color = Color.White;
        }

        public override void Update()
        {
            Velocity *= Main.rand.NextFloat(0.96f, 1f);
            _stretchScale.Y = MathHelper.Lerp(1f, 1, Velocity.Length() / 5f);
            Velocity.Y += downwardPull;
            Scale *= Main.rand.NextFloat(0.9f, 1f);
            color *= Main.rand.NextFloat(0.99f, 1f); ;
            if (_fast)
            {
                Velocity *= Main.rand.NextFloat(0.9f, 1f);
                Scale *= Main.rand.NextFloat(0.9f, 1f);
            }

            fadeIn++;
            _interpolant = fadeIn / 30f;
            if (fadeIn > 120)
                active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            GlowFragmentShader shader = GlowFragmentShader.Instance;
            shader.InnerColor = Color.Lerp(innerColor, fadeToColor, _interpolant);
            shader.OuterGlowColor = Color.Lerp(outerColor, fadeToColor, _interpolant);
            if(distortOut)
                shader.Distortion = MathHelper.Lerp(0f, 1f, _interpolant);
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
   
            spriteBatch.Draw(texture.Value, centerPos, null,color, Rotation, texture.Size() / 2f, Scale * _stretchScale, SpriteEffects.None, 0);
        }
    }
}

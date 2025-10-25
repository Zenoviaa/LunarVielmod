using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Stellamod.Core.Particles;

namespace Stellamod.Visual.Particles
{
    public class ImpactParticle : Particle
    {
        private Vector2 _stretchScale;
        private float _interpolant;
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public bool fast;

        public Color innerColor;
        public Color outerColor;
        public Color fadeToColor;

        public override void OnSpawn()
        {
    
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.3f, 0.6f);
            customShader = RadiantShader.Instance;
            _stretchScale = Vector2.One;
            _stretchScale.X *= Main.rand.NextFloat(1.5f, 2.5f);
            _stretchScale.Y *= Main.rand.NextFloat(0.1f, 0.6f);
            innerColor = Color.Yellow;
            outerColor = Color.Red;
            fadeToColor = Color.Blue;
            color = Color.White;
        }

        public override void Update()
        {
            color *= 0.98f;
            if(fast)
                color *= 0.92f;
            Velocity *= 0.95f;
            Rotation = Velocity.ToRotation();
            fadeIn++;
            _interpolant = fadeIn / 60f;
            if (fadeIn > 120f)
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
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale * _stretchScale, SpriteEffects.None, 0);
        }
    }
}

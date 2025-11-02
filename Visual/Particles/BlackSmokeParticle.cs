using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class BlackSmokeParticle : Particle
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
            Scale = Main.rand.NextFloat(0.65f, 2);
            stretchScale = Vector2.One;
            customShader = BlackLingeringSmokeShader.Instance;

            innerColor = Color.White;
            outerColor = Color.Yellow;
            fadeToColor = Color.Blue;

            innerColor = Color.Lerp(Color.DarkRed, Color.Black, 0.75f);
            outerColor = Color.Lerp(Color.DarkGray, Color.Black, 0.9f);

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
            var shader = BlackLingeringSmokeShader.Instance;
            shader.InnerColor = innerColor;
            shader.OuterColor = outerColor;
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
            spriteBatch.Draw(texture.Value, centerPos, null, color, Rotation, texture.Size() / 2f, Scale * stretchScale, SpriteEffects.None, 0);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class ConstellationParticle : Particle
    {
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public int MaxFrameCount = 3;
        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(0, 3.14f);
            Scale = Main.rand.NextFloat(0.5f, 1f);
            Frame = new Rectangle(0, Main.rand.Next(3) * FrameHeight, FrameWidth, FrameHeight);
            customShader = StarryGlowShader.Instance;
            color = Color.Purple;
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.994f;
            color *= 0.99f;

            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            var shader = StarryGlowShader.Instance;
            shader.GlowColor = Color.Pink;
            shader.Apply();


            Asset<Texture2D> texture = GetTexture();
            spriteBatch.Draw(texture.Value, centerPos, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

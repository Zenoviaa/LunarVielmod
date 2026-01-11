using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using System;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class SmokeParticle : Particle<SmokeParticle>
    {
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public int MaxFrameCount = 3;

        public Color initialColor;
        public Color fadeToColor;
        public int extraUpdates;
        public bool expand;
        public bool fast;
        public override void OnSpawn()
        {
            fast = false;
            expand = false;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            customShader = null;
            Rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        }

        private void InnerUpdate()
        {
            Velocity *= Main.rand.NextFloat(0.97f, 0.999f);
            Rotation += MathF.Sign(Velocity.X) * 0.01f;
            if (expand)
            {
                Scale *= Main.rand.NextFloat(1.0001f, 1.002f);
            }
            else
            {
                Scale *= Main.rand.NextFloat(0.97f, 0.99f);
            }
        
            if (fast)
            {
                Scale *= 0.95f;
            }
            color = Color.Lerp(initialColor, fadeToColor, fadeIn / 90f);

            float ratio = fadeIn / 180f;
            float alpha = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(ratio));
            color *= alpha;

            fadeIn++;
            if (fadeIn > 180)
                active = false;
        }
        public override void Update()
        {
            InnerUpdate();
            for (int i = 0; i < extraUpdates; i++)
            {
                InnerUpdate();
            }


        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = Center - Main.screenPosition;
            spriteBatch.Draw(GetTexture().Value, centerPos, Frame, color, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}

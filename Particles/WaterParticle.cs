

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class WaterParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        private const int Frame_Count = 16;
        private const int Frame_Duration = 1;
        public override void SetDefaults()
        {
            width = 34;
            height = 32;
            Scale = 1f;
            timeLeft = Frame_Count * Frame_Duration;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D tex3 = texture;
            Vector2 origin = this.OriginCenter();

            float totalTime = Frame_Count * Frame_Duration;
            float completion = (float)timeLeft / totalTime;

            //I want it to scale down smoothly
            float scaleMultiplier = completion;
            spriteBatch.Draw(tex3, screenPos, 
                tex3.AnimationFrame(ref frameCount, ref frameTick, Frame_Duration, Frame_Count, true), Color.White, 
                velocity.ToRotation() + 180,
                origin, 
                1.35f * scale * scaleMultiplier, SpriteEffects.None, 0f);

            return false;
        }
    }
}
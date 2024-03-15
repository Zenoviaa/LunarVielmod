

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class VoidParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        private const int Frame_Count = 13;
        private const int Frame_Duration = 1;
        public override void SetDefaults()
        {
            width = 32;
            height = 32;
            Scale = 1f;
            timeLeft = Frame_Count * Frame_Duration;
           
            layer = Layer.BeforeNPCs;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 origin = this.OriginCenter();
            spriteBatch.Draw(texture, screenPos, 
                texture.AnimationFrame(ref frameCount, ref frameTick, Frame_Duration, Frame_Count, true), Color.White, 
                velocity.ToRotation() + 180,
               origin, 
                1.35f * scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
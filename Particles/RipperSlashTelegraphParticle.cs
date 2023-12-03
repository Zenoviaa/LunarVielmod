

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class RipperSlashTelegraphParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        private const int Frame_Count = 24;
        private const int Frame_Duration = 2;
        public const int Animation_Length = Frame_Count * Frame_Duration;
        public override void SetDefaults()
        {
            width = 128;
            height = 32;
            Scale = 1f;
            timeLeft = Frame_Count * Frame_Duration;
            layer = Layer.BeforeNPCs;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D tex3 = Request<Texture2D>("Stellamod/Particles/RipperSlashTelegraphParticle").Value;
            Vector2 origin = this.OriginCenter();
            spriteBatch.Draw(tex3, screenPos, 
                tex3.AnimationFrame(ref frameCount, ref frameTick, Frame_Duration, Frame_Count, true), Color.White, 
                rotation,
                origin, 
                1.35f * scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
﻿

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class LaserTestParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        private const int Frame_Count = 8;
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
            Texture2D tex3 = Request<Texture2D>("Stellamod/Particles/LaserTestParticle").Value;
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
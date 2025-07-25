﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dusts
{
    public class Orb1Glow : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.fadeIn = 120;
            dust.frame = new Rectangle(0, 0, 54, 54);
        }

        const float SinTime = 120 / (float)Math.PI;
        const float ScaleMultiplier = 0.01f;

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * (float)Math.Sin(dust.fadeIn / SinTime) * 1.5f;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, dust.color.ToVector3() * dust.scale);

            dust.scale *= -(float)Math.Sin(dust.fadeIn / (SinTime * 0.5f)) * ScaleMultiplier + 1;
            dust.fadeIn--;
            dust.position += dust.velocity;

            if (dust.fadeIn < 0)
                dust.active = false;
            return false;
        }
    }
}
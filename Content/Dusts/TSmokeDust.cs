﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dusts
{
    public class TSmokeDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            var gray = new Color(25, 25, 25);
            Color black = Color.Black;
            Color ret;

            if (dust.alpha < 120)
                ret = Color.Lerp(dust.color, gray, dust.alpha / 120f);
            else if (dust.alpha < 180)
                ret = Color.Lerp(gray, black, (dust.alpha - 120) / 60f);
            else
                ret = black;

            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity = dust.velocity.RotatedBy(MathHelper.PiOver4 / 32);
            dust.velocity *= 0.98f;
            dust.velocity.X *= 0.95f;
            dust.velocity.Y *= 0.97f;
            dust.color *= 0.98f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.975f;
                float px = 0.5f;
               // dust.scale = MathF.Round(dust.scale / px) * px;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;
            dust.rotation += dust.velocity.Length() * 0.04f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}

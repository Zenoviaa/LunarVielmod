using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Dusts
{
    public class LumiDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
           
            dust.scale *= Main.rand.NextFloat(0.5f, 1.2f);
            dust.fadeIn = 0;
            dust.noLight = false;
            dust.rotation = Main.rand.NextFloat(6.28f);
            dust.frame = new Rectangle(0, 0, 32, 22);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }
        public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.95f;
            Lighting.AddLight(dust.position, dust.color.ToVector3() * 1f * Main.essScale);
            dust.fadeIn++;
            float alpha = (dust.fadeIn / 45f) - ((float)Math.Pow(dust.fadeIn, 2) / 3600f);

            dust.scale *= 0.99f;
			if (dust.scale < 0.2f) {
				dust.active = false;
			}
			return false;
		}
	}
}

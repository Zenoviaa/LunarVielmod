using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Dusts
{
	public class TSmokeTornadoDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
            dust.rotation = Main.rand.NextFloat(6.28f);
            dust.alpha = 100;
        //    dust.scale = 0.3f;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            var gray = new Color(25, 25, 25);
            Color black = Color.Black;
            Color ret = dust.color;
            dust.color = Color.Lerp(dust.color, Color.Black, 0.005f);
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity = dust.velocity.RotatedBy(MathHelper.PiOver4 / 4);
            dust.position.Y -= 10 * dust.scale * 0.25f;
            dust.color *= 0.98f;

            if (dust.alpha > 100)
            {
                dust.scale *= 1.05f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.04f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}

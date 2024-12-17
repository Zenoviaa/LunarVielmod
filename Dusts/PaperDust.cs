using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Dusts
{
    public class PaperDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale = 1f;
            dust.frame = new Rectangle(0, 32 * Main.rand.Next(3), 32, 32);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.92f;
            dust.rotation += 0.06f;
            dust.scale *= Main.rand.NextFloat(0.94f, 0.98f);
            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}

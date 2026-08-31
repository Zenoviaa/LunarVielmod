using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dusts
{
    public class GlowHeartDust : GlowDust
    {
        public override void OnSpawn(Dust dust)
        {
            base.OnSpawn(dust);
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 64, 64);
        }
    }
}
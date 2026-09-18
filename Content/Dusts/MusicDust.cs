using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace Stellamod.Content.Dusts
{
    public class MusicDust : GlowDust
    {
        public override void OnSpawn(Dust dust)
        {
            base.OnSpawn(dust);
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 64, 64, 64);     
        }
    }
}
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Content.Dusts;
namespace Stellamod.Buffs
{
    public class Overheated : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(player.Center, ModContent.DustType<TSmokeDust>(), Vector2.UnitY * -2, 0, Color.Black * 0.5f, Main.rand.NextFloat(0.3f, 0.7f));
            }
        }
    }
}

using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class VialedUp : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.maxFallSpeed *= 3;
            player.noFallDmg = true;
            player.jumpBoost = true;
            player.moveSpeed += 0.4f;
            player.maxRunSpeed += 0.4f;
        }
    }
}
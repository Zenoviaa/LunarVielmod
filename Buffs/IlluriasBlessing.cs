using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class IlluriasBlessing : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gravity = Player.defaultGravity;
            player.accRunSpeed += 0.05f;
            player.moveSpeed += 0.05f;
        }
    }
}
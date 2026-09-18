using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class FlowerPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 18;
        }
    }
}

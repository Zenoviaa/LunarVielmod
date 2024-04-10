using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class FlowerPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("10+ Defense and Golden trail oooo :0");
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 18;
        }
    }
}

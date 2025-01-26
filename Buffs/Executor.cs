using Stellamod.Items.Flasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Executor : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("A true warrior such as yourself knows no bounds");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            EckasectPlayer EckasectPlayer = player.GetModPlayer<EckasectPlayer>();
            EckasectPlayer.Executor = true;


        }
    }
}
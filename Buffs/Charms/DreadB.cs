using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    public class DreadB : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{	
			player.statLifeMax2 += 40;	
		}
	}
}
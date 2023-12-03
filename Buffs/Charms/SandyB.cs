using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
	public class SandyB : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("Icy Frileness!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{

			player.statDefense += 2;

		}
	}
}
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
	public class DeathMultiplier : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Death Multiplier");
			Description.SetDefault("A multiplier of death for Magiblades");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
		}
	}
}

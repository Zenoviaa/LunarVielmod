using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Buffs
{
    public class GintzelSheild : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
			// DisplayName.SetDefault("Acid Flame");
			// Description.SetDefault("'An Acidic force melts your insides'");
		}

		public override void Update(Player player, ref int buffIndex)
		{
            player.statDefense += 15;
		}
	}
}
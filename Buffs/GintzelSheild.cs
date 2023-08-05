using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Stellamod.Buffs
{
	public class GintzelSheild : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
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
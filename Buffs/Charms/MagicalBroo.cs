using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
	public class MagicalBroo : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("Icy Frileness!");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			Lighting.AddLight(player.Center, Color.PaleVioletRed.ToVector3() * 2.75f * Main.essScale);
			player.statDefense += 5;
			player.statManaMax2 += 100;
			player.statLifeMax2 += 50;
			player.magicCuffs = true;
			player.manaFlower = true;
			player.manaCost /= 20;
		}
	}
}
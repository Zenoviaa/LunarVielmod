using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    public class Diarii : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 2;
			player.armorEffectDrawOutlines = true;
			player.autoReuseGlove = true;
			player.GetDamage(DamageClass.Generic) *= 1.05f; // Increase ALL player damage by 100%
			player.wellFed = true;
			player.statManaMax2 += 10; // Increase how many mana points the player can have by 20
			player.statLifeMax2 += 10;
		}
	}
}
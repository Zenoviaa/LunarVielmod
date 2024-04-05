using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    public class BurningGB : ModBuff
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
			Lighting.AddLight(player.Center, Color.DarkOrange.ToVector3() * 2.75f * Main.essScale);
			player.statDefense += 6;
			player.GetDamage(DamageClass.Melee) *= 1.05f;
		}
	}
}
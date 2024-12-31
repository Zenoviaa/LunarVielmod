using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    public class GovheilB : ModBuff
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
		
			player.statDefense += 2;
			player.GetDamage(DamageClass.Magic) *= 1.07f;
			player.manaCost *= 0.5f;
		}
	}
}
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    public class Morrow : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("2+ Defense and Increased ranged damage and heavy increased arrow damage");
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 2;
			player.arrowDamage += 0.25f;
			player.GetDamage(DamageClass.Ranged) += 0.02f; // Increase ALL player damage by 100%
		}
	}
}
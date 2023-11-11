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
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			Lighting.AddLight(player.Center, Color.LightBlue.ToVector3() * 2.75f * Main.essScale);
			player.statDefense += 2;
			player.GetDamage(DamageClass.Magic) *= 1.15f;
			player.manaCost *= 0.5f;

		}
	}
}
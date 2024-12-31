using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Charms
{
    public class AmberB : ModBuff
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
			Lighting.AddLight(player.Center, Color.Orange.ToVector3() * 4.75f * Main.essScale);
			player.statDefense += 5;
			player.pickSpeed -= 40;
			player.moveSpeed += 0.4f;
			player.maxRunSpeed += 0.4f;
		}
	}
}
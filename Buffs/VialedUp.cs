using Microsoft.Xna.Framework;

using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
	public class VialedUp : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("10+ Defense and Golden trail oooo :0");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		private int _miracleSoulCooldown;
		public int miracleLevel;
		public int miracleTimeLeft;
		public bool hasMiracleSet;

	



		public override void Update(Player player, ref int buffIndex)
		{
			player.maxFallSpeed *= 3;
			player.noFallDmg = true;
			player.jumpBoost = true;
			player.moveSpeed += 0.4f;
			player.maxRunSpeed += 0.4f;
		}
	}
}
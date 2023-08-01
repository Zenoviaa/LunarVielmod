using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
	public class AbyssalFlame : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
			// DisplayName.SetDefault("Abyssal Flame");
			// Description.SetDefault("'A Dark force saps your vitality'");
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 8;

			if (Main.rand.Next(2) == 0)
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, 206);
				Main.dust[dust].scale = 3.5f;
				Main.dust[dust].noGravity = true;
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{

            player.lifeRegen -= 8;

			if (Main.rand.Next(2) == 0)
			{
				int dust = Dust.NewDust(player.position, player.width, player.height, 206);
				Main.dust[dust].scale = 3.5f;
				Main.dust[dust].noGravity = true;
			}
		}
	}
}
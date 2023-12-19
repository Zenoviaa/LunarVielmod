using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
	public class Chained : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 16;
			float pullStartDistance = 320;
			npc.velocity *= 0.04f;
			for (int i = 0; i < Main.maxPlayers; i++)
            {
				Player player = Main.player[i];
                if (player.active)
                {
					float distance = Vector2.Distance(player.Center, npc.Center);
					if (distance > pullStartDistance)
					{
						float pullStrength = 5f;
						npc.velocity += npc.DirectionTo(player.Center) * pullStrength;
					}
				}
			}

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Stone);
				Main.dust[dust].scale = .85f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen -= 6;
			if (Main.rand.NextBool(4))
			{
				int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Stone);
				Main.dust[dust].scale = 1f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;

				int dust2 = Dust.NewDust(player.position, player.width, player.height, DustID.Stone);
				Main.dust[dust].scale = .95f;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;
			}
		}
	}
}
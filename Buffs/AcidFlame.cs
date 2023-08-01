using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Stellamod.Buffs
{
	public class AcidFlame : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
			// DisplayName.SetDefault("Acid Flame");
			// Description.SetDefault("'An Acidic force melts your insides'");
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 8;

			if (Main.rand.Next(2) == 0)
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, 74);
				Main.dust[dust].scale = 1.5f;
				Main.dust[dust].noGravity = true;
				int dust1 = Dust.NewDust(npc.position, npc.width, npc.height, 74);
				Main.dust[dust1].scale = 1.5f;
				Main.dust[dust1].noGravity = true;
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{

            player.lifeRegen -= 16;
			player.manaRegen -= 8;
			player.blind = true;
			player.blackout = true;
            player.yoraiz0rDarkness = true;

			if (Main.rand.Next(2) == 0)
			{
				int dust = Dust.NewDust(player.position, player.width, player.height, 74);
				Main.dust[dust].scale = 1.5f;
				Main.dust[dust].noGravity = true;
			}
		}
	}
}
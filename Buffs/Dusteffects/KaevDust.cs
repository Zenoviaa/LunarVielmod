﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
	public class KaevDust : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Dust");
			Description.SetDefault("A dust that is for the death");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;

		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 5;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GemRuby);
				Main.dust[dust].scale = 0.8f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = false;
			}
		}
	}
}
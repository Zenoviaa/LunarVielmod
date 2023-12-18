using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
	public class AgreviDust : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("TrickDust");
			// Description.SetDefault("A dust that is for true tricksters");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;

		}
		public override void Update(NPC npc, ref int buffIndex)
		{


			npc.stepSpeed -= 10;


			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.FlameBurst);
				Main.dust[dust].scale = 1f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
				int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.CopperCoin);
				Main.dust[dust2].scale = 1.5f;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;

			}
		}
	}
}
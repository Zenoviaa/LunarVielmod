using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
    public class AivanDust : ModBuff
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
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.IceTorch);
				Main.dust[dust].scale = 1f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
				int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GoldCoin);
				Main.dust[dust2].scale = 1.5f;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;
				int dust3 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RedTorch);
				Main.dust[dust3].scale = 1f;
				Main.dust[dust3].noGravity = true;
				Main.dust[dust3].noLight = true;
				int dust2a = Dust.NewDust(npc.position, npc.width, npc.height, DustID.ShadowbeamStaff);
				Main.dust[dust2a].scale = 1.5f;
				Main.dust[dust2a].noGravity = true;
				Main.dust[dust2a].noLight = true;
			}
		}
	}
}
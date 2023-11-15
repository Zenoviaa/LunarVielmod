using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
    public class CoalBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Coal Buff");
			// Description.SetDefault("A dust that is for the gemsparked!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;

		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 1;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GemSapphire);
				int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GemEmerald);
				int dust3 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GemRuby);
				Main.dust[dust].scale = 1f;
				Main.dust[dust2].scale = 1f;
				Main.dust[dust3].scale = 1f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;		
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;
				Main.dust[dust3].noGravity = true;
				Main.dust[dust3].noLight = true;
			}
		}
	}
}
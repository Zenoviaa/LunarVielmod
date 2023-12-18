using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
	public class JungleDust : ModBuff
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
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.JungleTorch);
				Main.dust[dust].scale = 1f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;

			}
		}
	}
}
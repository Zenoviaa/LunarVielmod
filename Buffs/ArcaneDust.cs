using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class ArcaneDust : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FlameDust");
			// Description.SetDefault("A dust that is for the arcane!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;

		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 4;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.SpectreStaff);
				Main.dust[dust].scale = .85f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}
	}
}
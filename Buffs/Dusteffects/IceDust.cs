using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
    public class IceDust : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("IceDust");
			// Description.SetDefault("A dust that is for the frozen");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
		

		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			
			npc.onFrostBurn = true;
			npc.stepSpeed -= 10;


			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.IceTorch);
				Main.dust[dust].scale = 1.5f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}
	}
}
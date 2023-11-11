using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
    public class SongDust : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SongDust");
			// Description.SetDefault("A dust that is for the whimsical");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			BuffID.Sets.IsATagBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;

		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 2;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.FireworksRGB);
				int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GoldCoin);
				Main.dust[dust].scale = 1.5f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;		
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;
			}
		}
	}
}
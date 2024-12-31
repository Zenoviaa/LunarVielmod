using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Rekin : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;
			// DisplayName.SetDefault("DarkHold");
			// Description.SetDefault("'Dark forces grip you tighly'");
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GoldCoin);
			Main.dust[dust2].scale = 1.5f;
			Main.dust[dust2].noGravity = true;
			Main.dust[dust2].noLight = true;


		}

		
	}
}
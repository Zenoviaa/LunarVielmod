using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Dusted : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dusted");
			// Description.SetDefault("Marks enemies with dust on them");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
			BuffID.Sets.IsATagBuff[Type] = true;
		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 6;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Smoke);
				Main.dust[dust].scale = .85f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}
		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen -= 6;

			if (Main.rand.NextBool(4))
			{
				int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Smoke);
				Main.dust[dust].scale = 1f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
				int dust2 = Dust.NewDust(player.position, player.width, player.height, DustID.Smoke);
				Main.dust[dust].scale = .95f;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;
			}
		}
	}
}
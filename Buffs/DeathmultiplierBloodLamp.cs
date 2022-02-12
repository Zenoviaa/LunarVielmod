using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Buffs
{
	public class DeathmultiplierBloodLamp : ModBuff
	{
		public override void SetStaticDefaults()
		{



			DisplayName.SetDefault("Multiplier Blood Lamp");
			Description.SetDefault("A multiplier of death for Blood Lamp");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;

		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 7;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.FireworkFountain_Red);
				Main.dust[dust].scale = .85f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}


	}
}
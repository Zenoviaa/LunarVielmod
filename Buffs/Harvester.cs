using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Buffs
{
	public class Harvester : ModBuff
	{
		public override void SetStaticDefaults()
		{



			DisplayName.SetDefault("HarvestIT");
			Description.SetDefault("A signifier for harvest npcs!");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;

		}


	}
}

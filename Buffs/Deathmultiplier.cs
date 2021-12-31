using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Buffs
{
    public class Deathmultiplier : ModBuff
    {
		public override void SetStaticDefaults()
		{



			DisplayName.SetDefault("Death Multiplier");
			Description.SetDefault("A multiplier of death for Magiblades");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			
		}
 

	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
    public class EXPtime2 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("EXPtime2");
			// Description.SetDefault("thingy");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;

		}
	
	}
}
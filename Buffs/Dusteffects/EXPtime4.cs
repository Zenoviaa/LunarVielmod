using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Dusteffects
{
    public class EXPtime4 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("EXPtime4");
			// Description.SetDefault("thingymore");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;

		}
	
	}
}
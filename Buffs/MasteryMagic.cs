using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class MasteryMagic : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Seen");
			// Description.SetDefault("'Seen'");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}


	}
}
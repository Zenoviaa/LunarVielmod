using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Items.Flasks;

namespace Stellamod.Buffs
{
	public class Genesis : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			EckasectPlayer EckasectPlayer = player.GetModPlayer<EckasectPlayer>();
			EckasectPlayer.Genesis = true;


		}
	}
}
using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Items.Flasks;

namespace Stellamod.Buffs
{
	public class CannotUseFlask : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			FlaskPlayer FlaskPlayer = player.GetModPlayer<FlaskPlayer>();
			FlaskPlayer.hasTime = true;


		}
	}
}
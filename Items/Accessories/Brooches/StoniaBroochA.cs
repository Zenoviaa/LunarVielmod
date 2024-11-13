using Microsoft.Xna.Framework;

using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class StoniaBroochA : BaseBrooch
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Blue;
			Item.buffType = ModContent.BuffType<StoneB>();
			Item.accessory = true;
		}
	}
}
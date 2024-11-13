using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MorrowedBroochA : BaseBrooch
	{ 
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Green;
			Item.buffType = ModContent.BuffType<Morrow>();
			Item.accessory = true;
		}
	}
}
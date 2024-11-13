using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class AurelusBlightBroochA : BaseBrooch
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Green;
			Item.buffType = ModContent.BuffType<AurelusB>();
			Item.accessory = true;
		}
	}
}
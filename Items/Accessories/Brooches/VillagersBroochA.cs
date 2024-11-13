using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
	public class VillagersBroochA : BaseBrooch
	{	
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
			Item.buffType = ModContent.BuffType<VillagersB>();
			BroochType = BroochType.Advanced;
		}
	}
}
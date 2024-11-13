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
    public class SlimeBroochA : BaseBrooch
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Green;
			Item.buffType = ModContent.BuffType<Slimee>();
			Item.accessory = true;
		}

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            player.frogLegJumpBoost = true;  // Increase ALL player damage by 100%
        }
	}
}
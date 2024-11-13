using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MagicalBroochA : BaseBrooch
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 25);
			Item.rare = ItemRarityID.LightPurple;
			Item.buffType = ModContent.BuffType<MagicalBroo>();
			Item.accessory = true;
			BroochType = BroochType.Advanced;
		}

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);       
            player.GetDamage(DamageClass.Magic) *= 1.2f;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 100);
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 100);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 30);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 20);
			recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
			recipe.AddIngredient(ItemID.NaturesGift, 1);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();

			//Check that this item is equipped
			if (player.HasItemEquipped(Item))
			{
				//Check that you have advanced brooches since these don't work without
				if (broochPlayer.hasAdvancedBrooches)
				{
					//Give backglow to show that the effect is active
					DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 124, 225));
				}
				else
				{
					float sizeLimit = 28;
					//Draw the item icon but gray and transparent to show that the effect is not active
					Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
					return false;
				}
			}

			return true;
		}
	}
}
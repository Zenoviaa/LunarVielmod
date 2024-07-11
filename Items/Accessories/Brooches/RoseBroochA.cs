using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class RoseBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch of The Spragald");
			/* Tooltip.SetDefault("Simple Brooch!" +
				"\nEffect = +10 Defense" +
				"\n Use the power of the Spragald Spiders!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
			Item.defense = 4;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Brooch of Rose", Helpers.LangText.Common("SimpleBrooch"))
			{
				OverrideColor = new Color(198, 124, 225)
			};

			tooltips.Add(line);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.KeepBroochAlive<RoseBrooch, RoseB>(ref broochPlayer.hasRoseBrooch);
			if(player.ownedProjectileCounts[ModContent.ProjectileType<RoseShield>()] == 0)
            {
				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * -1f,
					ModContent.ProjectileType<RoseShield>(), 0, 1f, player.whoAmI);
			}
		}

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<TerrorFragments>(), 15);
			recipe.AddIngredient(ItemID.LifeCrystal, 5);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}
    }
}
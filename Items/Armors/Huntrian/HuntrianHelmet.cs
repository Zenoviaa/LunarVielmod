using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Huntrian
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class HuntrianHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Huntrian Helmet");
			/* Tooltip.SetDefault("Corroded Woods"
				+ "\n+5% Damage!" +
				"\n+20 Health"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
		
			player.statLifeMax2 += 10;
			player.GetDamage(DamageClass.Generic) *= 1.05f;

		}
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<HuntrianChestplate>() && legs.type == ModContent.ItemType<HuntrianBoots>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Increases life regen by a big amount!" + "\nReduced Healing Flask cooldown" + "\nDOES NOT STACK with philosophers stone"); // This is the setbonus tooltip

            if (player.HasBuff(BuffID.PotionSickness))
            {
				int buffIndex = player.FindBuffIndex(BuffID.PotionSickness);

				//idk how to math so I just do 45 * 60 to convert 45 seconds to ticks
				//It's easier to read I think anyways
				int secondsToSetTo = 45;
				int ticks = secondsToSetTo * 60;
				if(player.buffTime[buffIndex] > ticks)
                {
					player.buffTime[buffIndex] = ticks;
				}
			}

			//player.pStone = true;
			player.lifeRegen += 2;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 25);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddIngredient(ModContent.ItemType<GintzlMetal>(), 10);
			recipe.AddRecipeGroup(nameof(ItemID.DemoniteBar), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
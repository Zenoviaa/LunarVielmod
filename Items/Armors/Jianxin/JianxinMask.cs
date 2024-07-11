using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Slashers.Vixyl;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Jianxin
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class JianxinMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Verl Breastplate");
			/* Tooltip.SetDefault("Shines with a blooming moon"
				+ "\n+10% Ranged and Magic Damage!" +
				"\n+12 Penetration" +
				"\n+5 Critical Strike Chance!" +
				"\n+20 Max Life"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 23; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.GetArmorPenetration(DamageClass.Generic) += 10;
            player.GetDamage(DamageClass.Generic) *= 1.1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<JianxinCoat>() && legs.type == ModContent.ItemType<JianxinPants>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Increases life regen by a great amount!" + "\nMove faster and go along with the watery winds." + "\nSummons in watery dragons to come and swirl around your character." + "\nThese dragons emit a great becoming amongst the lost dynasty and give 50 Health." + "\nThis aura also lessens enemy damage by 10% and damages enemies." + "\nEnemies are less likely to target you!"); // This is the setbonus tooltip
           
            player.statLifeMax2 += 50;
            player.moveSpeed += 0.3f;
            player.maxRunSpeed += 0.3f;
            player.lifeRegen += 2;  // This is the setbonus tooltip
            player.aggro *= 2;
            player.endurance += 0.10f;

            player.GetModPlayer<MyPlayer>().Waterwhisps = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<WateryWhisp>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<WateryWhisp>(), 120, 4, player.whoAmI);
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<DragonsSurround>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<DragonsSurround>(), 120, 4, player.whoAmI);
            }

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<IshtarCandle>(), 1);
            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Superfragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 100);
            recipe.AddIngredient(ItemID.SeashellHairpin, 1);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }



        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}
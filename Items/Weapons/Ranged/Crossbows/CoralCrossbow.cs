using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Crossbows.Ultras;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.Crossbows
{

    public class CoralCrossbow : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Crossbow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Use a small crossbow and shoot three bolts!"
                + "\n'Triple Threat!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<CoralCrossbowHold>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 12);
            Item.noUseGraphic = true;
            Item.channel = true;
       

        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Coral, 10);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 15);
            recipe.AddIngredient(ModContent.ItemType<BlankCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DesertCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DungeonCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<FrostyCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MoltenCrossbow>(), 1);
            recipe.Register();
        }


    }
}
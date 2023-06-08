using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Audio;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stellamod.Projectiles.Crossbows;
using Stellamod.Projectiles.Crossbows.Gemmed;
using Stellamod.Projectiles.Crossbows.Magical;
using Stellamod.Projectiles.Crossbows.Ultras;
using Stellamod.Items.Ores;
using Stellamod.Items.Materials;

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
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<CoralCrossbowHold>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 3);
            Item.noUseGraphic = true;
            Item.channel = true;
       

        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Coral, 10);
            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 15);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 15);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DesertCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DungeonCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<FrostyCrossbow>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MoltenCrossbow>(), 1);
            recipe.Register();
        }


    }
}
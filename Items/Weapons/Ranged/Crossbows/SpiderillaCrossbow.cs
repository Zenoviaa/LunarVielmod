using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Audio;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stellamod.Projectiles.Crossbows;
using Stellamod.Projectiles.Crossbows.Lead;
using Stellamod.Projectiles.Crossbows.MerNDungeon;
using Stellamod.Projectiles.Crossbows.Sniper;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Ranged.Crossbows
{

    public class SpiderillaCrossbow : ModItem
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
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
            Item.shootSpeed = 40f;
            Item.shoot = ModContent.ProjectileType<SpiderillaCrossbowHold>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(gold: 5);
            Item.noUseGraphic = true;
            Item.channel = true;


        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);

            recipe.AddIngredient(ModContent.ItemType<DustedSilk>(), 30);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 13);
            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 13);
            recipe.AddIngredient(ItemID.Cobweb, 100);
            recipe.AddIngredient(ItemID.WebSlinger, 1);

            recipe.Register();


            
        }




    }
}
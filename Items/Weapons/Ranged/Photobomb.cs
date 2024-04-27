using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Crossbows.Lead;
using Stellamod.Projectiles.Paint;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{

    public class Photobomb : ModItem
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
            Item.useTime = 98;
            Item.useAnimation = 98;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<PhotobombProj>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 7);
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;

        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
            recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 5);
            recipe.AddIngredient(ItemID.PainterPaintballGun, 1);
            recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 10);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }


    }
}
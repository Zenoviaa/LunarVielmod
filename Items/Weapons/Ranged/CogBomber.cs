using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class CogBomber : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 34;
            Item.useTime = 40;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/gun1");
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<CogBomb>();
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        /*
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MetallicOmniSource>(), 8);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 10);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddIngredient(ItemID.MolotovCocktail, 100);
            recipe.Register();
        }*/
    }
}

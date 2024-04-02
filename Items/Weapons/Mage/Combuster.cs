using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Combuster : ModItem
    {
        private int _combo;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 54;
            Item.damage = 36;
            Item.knockBack = 8;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<CombusterSparkProj1>();
            Item.rare = ItemRarityID.LightRed;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8f, -8f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int slowdown = 6;
            int maxCombo = 15;
            if(_combo == maxCombo)
            {
                type = ModContent.ProjectileType<CombusterSparkProj3>();
                Item.useTime /= slowdown;
                Item.useAnimation /= slowdown;
            } else if(_combo == maxCombo - 1)
            {
                type = ModContent.ProjectileType<CombusterSparkProj2>();
                Item.useTime *= slowdown;
                Item.useAnimation *= slowdown;
            }
            else
            {
                bool alternate = _combo % 2 == 0;
                type = alternate ? ModContent.ProjectileType<CombusterSparkProj1>() : ModContent.ProjectileType<CombusterSparkProj2>();
            }

            _combo++;
            if (_combo >= maxCombo+1)
                _combo = 0;
            position = Main.MouseWorld;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bottle, 1);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 18);
            recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 10);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}

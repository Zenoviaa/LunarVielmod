using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Spears;
using Stellamod.Projectiles.Bow;
using Terraria.Audio;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class HeatGlider : ModItem
    {
        public int WinterboundArrow;
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BorealWood, 8);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 12);
            recipe.AddIngredient(ItemID.SnowBlock, 7);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<HuntrianArrow>();
            }
        }

    }
}

using Stellamod.Projectiles.Bow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria.Audio;
using Terraria.DataStructures;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Tech;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class Lihh : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 82;
            Item.width = 44;
            Item.height = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Lime;

      
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 19f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }


        private int _combo;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            _combo++;
            if (_combo == 3)
            {
                SoundEngine.PlaySound(SoundID.Item78, position);
                type = ModContent.ProjectileType<HeatArrow>();
                _combo = 0;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (_combo == 3)
            {
                float numberProjectiles = 5;
                float rotation = MathHelper.ToRadians(25);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);
                }
            }

            if (_combo != 3)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 / 7), type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4 / 7), type, damage, knockback, player.whoAmI);
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 12);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
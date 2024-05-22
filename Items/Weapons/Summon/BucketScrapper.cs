using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Helpers;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using Stellamod.Projectiles;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;
using Stellamod.Buffs.Minions;
using Stellamod.Projectiles.Summons.Minions;

namespace Stellamod.Items.Weapons.Summon
{
    internal class BucketScrapper : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.damage = 21;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 2;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item113;
            Item.buffType = ModContent.BuffType<BucketScrapperMinionBuff>();
            Item.shoot = ModContent.ProjectileType<BucketScrapperMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DriveConstruct>(), 20);
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}

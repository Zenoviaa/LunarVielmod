using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class Pegasus : ClassSwapItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gelatal Slaff");
            // Tooltip.SetDefault("Summons an Jelly boi to fight for you");
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override DamageClass AlternateClass => DamageClass.Summon;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 136;
        }

        public override void SetDefaults()
        {
            Item.damage = 106;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ModContent.RarityType<NiiviSpecialRarity>();

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<PegasusMinionBuff>();

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<PegasusMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasBuff(ModContent.BuffType<PegasusMinionBuff>()))
                return false;
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                player.whoAmI, 0);
            projectile.originalDamage = Item.damage;

            projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, 
                player.whoAmI, 1);
            projectile.originalDamage = Item.damage;

            projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, 
                player.whoAmI, 2);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<PureHeart>(), 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}

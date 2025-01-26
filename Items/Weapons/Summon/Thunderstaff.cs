using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class Thunderstaff : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Creeper Staff");
            // Tooltip.SetDefault("Summons an Irradiated Creeper to fight with you");
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 12;
            Item.mana = 10;
        }
        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 48;
            Item.height = 62;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.LightRed;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.UseSound = SoundID.Item46;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<CloudMinionBuff>();
            Item.shoot = ModContent.ProjectileType<CloudMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
                    return false;
            }

            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            player.AddBuff(Item.buffType, 2);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                for (int i = 0; i < 1000; ++i)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
                    {
                        Main.projectile[i].minionSlots += 1f;
                        Main.projectile[i].originalDamage = Item.damage + (int)(4 * Main.projectile[i].minionSlots);
                        if (Main.projectile[i].scale < 1.8f)
                        {
                            Main.projectile[i].scale += 0.1f;
                        }

                    }
                }
            }
            return true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<IllurineScale>());
        }
    }
}

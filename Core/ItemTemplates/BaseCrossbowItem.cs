﻿using Microsoft.Xna.Framework;
using Stellamod.Content.Items.Weapons.Ranged.Bows.IronBow;
using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.ItemTemplates
{
    internal abstract class BaseCrossbowItem : ModItem
    {
        public int CrossbowProjectileType; 
        public int staminaCost = 1;
        public int staminaProjectileShoot;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.crit = 16;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = null;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = false;
            CrossbowProjectileType = ModContent.ProjectileType<IronBowHold>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[CrossbowProjectileType] == 0;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public virtual void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayer comboPlayer = player.GetModPlayer<SwingPlayer>();
            comboPlayer.ConsumeStamina(staminaCost);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI);
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayer comboPlayer = player.GetModPlayer<SwingPlayer>();
            if (player.altFunctionUse == 2 && comboPlayer.CanUseStamina(staminaCost))
            {
                ShootSwingStamina(player, source, position, velocity, staminaProjectileShoot, damage, knockback);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, CrossbowProjectileType, damage, knockback, player.whoAmI);
            }

            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Crossbow", Helpers.LangText.Common("Crossbow"))
            {
                OverrideColor = Color.OrangeRed
            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "CrossbowHelp", Helpers.LangText.Common("CrossbowHelp"))
            {
                OverrideColor = Color.LightGray
            };
            tooltips.Add(line);
        }
    }
}

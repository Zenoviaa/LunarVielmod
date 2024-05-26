﻿using Microsoft.Xna.Framework;
using Stellamod.Buffs.Whipfx;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Summons.Orbs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon.Orbs
{
    internal class BlackBall : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BlackballDebuff.TagDamage);
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd", "Orb Weapon Type")
            {
                OverrideColor = ColorFunctions.OrbWeaponType
            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 48;
            Item.damage = 40;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 8;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightPurple;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<BlackBallProj>();
            Item.shootSpeed = 1;
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            OrbPlayer orbPlayer = player.GetModPlayer<OrbPlayer>();
            orbPlayer.EquipOrbSlot1(Type);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for(int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<BlackBallProj>()
                    && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[0]++;
                    break;
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<BlankOrb>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 23);
            recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 30);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 9);
            recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 9);
            recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
            recipe.Register();
        }
    }
}

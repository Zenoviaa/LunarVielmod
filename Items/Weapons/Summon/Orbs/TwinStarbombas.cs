using Microsoft.Xna.Framework;
using Stellamod.Buffs.Whipfx;
using Stellamod.Helpers;
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
    internal class TwinStarbombas : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TwinStarbombasDebuff.TagDamage);
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Orb"))
            {
                OverrideColor = ColorFunctions.OrbWeaponType
            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 48;
            Item.damage = 44;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 8;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Pink;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<TwinStarbombasProj1>();
            Item.shootSpeed = 1;
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            OrbPlayer orbPlayer = player.GetModPlayer<OrbPlayer>();
            orbPlayer.EquipOrbSlot1(Type);
            orbPlayer.EquipOrbSlot2(Type, ModContent.ProjectileType<TwinStarbombasProj2>());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int found = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<TwinStarbombasProj1>() 
                    && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[0]++;
                    found++;
                }
                if (Main.projectile[i].type == ModContent.ProjectileType<TwinStarbombasProj2>()
                    && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[0]++;
                    found++;
                }
                if(found >= 2)
                {
                    break;
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                 .AddIngredient(ModContent.ItemType<BlankOrb>(), 2)
                 .AddIngredient(ModContent.ItemType<STARCORE>(), 1)
                 .AddIngredient(ModContent.ItemType<AuroreanStarI>(), 150)
                 .AddTile(TileID.MythrilAnvil)
                 .Register();

        }
    }
}

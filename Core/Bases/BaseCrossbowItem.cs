using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public abstract class BaseCrossbowItem : ModItem
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
            DashPlayer comboPlayer = player.GetModPlayer<DashPlayer>();
            comboPlayer.Consume(staminaCost);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI);
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            DashPlayer comboPlayer = player.GetModPlayer<DashPlayer>();
            if (player.altFunctionUse == 2 && comboPlayer.CanConsume(staminaCost))
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


            Keys keys = Keys.LeftShift;
            bool isExpanded = Main.keyState.IsKeyDown(keys);
    
            if (!isExpanded)
            {
                line = new TooltipLine(Mod, "ExpandTooltipHelp", LangText.Common("ExpandTooltipHelp", "Left Shift"));
                line.OverrideColor = Color.LightGray;
                tooltips.Add(line);
            }
            else
            {
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

                line = new TooltipLine(Mod, "BasicSlash", LangText.Common("BasicSlash", LangText.Item(this, "BasicSlash")));
                line.OverrideColor = new Color(124, 187, 80);
                tooltips.Add(line);

                line = new TooltipLine(Mod, "StaminaSlash", LangText.Common("StaminaSlash", LangText.Item(this, "StaminaSlash")));
                line.OverrideColor = Color.Goldenrod;
                tooltips.Add(line);

                line = new TooltipLine(Mod, "StaminaCost", LangText.Common("StaminaCost", staminaCost.ToString()));
                line.OverrideColor = Color.Goldenrod;
                tooltips.Add(line);
            }
        }
    }
}

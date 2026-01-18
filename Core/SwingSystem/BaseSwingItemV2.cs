using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.SwingSystem
{
    public class SwingExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            if (item.ModItem == null)
                return;

            if (item.ModItem is BaseSwingItemV2 swingItem)
            {
                TooltipLine line = new TooltipLine(Mod, "WeaponType", LangText.Common("WeaponType" + swingItem.meleeWeaponType.ToString()));
                line.OverrideColor = Color.GreenYellow;
                lines.Add(line);

                line = new TooltipLine(Mod, "BasicSlash", swingItem.BasicSlash);
                lines.Add(line);

                line = new TooltipLine(Mod, "StaminaSlash", swingItem.StaminaSlash);
                lines.Add(line);

                line = new TooltipLine(Mod, "StaminaCost", LangText.Common("StaminaCost", swingItem.staminaCost.ToString()));
                line.OverrideColor = Color.Goldenrod;
                lines.Add(line);
            }
        }
    }
    public abstract class BaseSwingItemV2 : ModItem
    {
        public int comboResetTime = 120;
        public int staminaProjectileShoot;
        public int staminaCost = 2;


        public MeleeWeaponType meleeWeaponType;

        public string BasicSlash
        {
            get
            {
                return LangText.Common("BasicSlash", LangText.Item(this, "BasicSlash"));
            }
        }

        public string StaminaSlash
        {
            get
            {
                return LangText.Common("StaminaSlash", LangText.Item(this, "StaminaSlash"));
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            this.GetLocalization(nameof(BasicSlash), () => "No Effect");
            this.GetLocalization(nameof(StaminaSlash), () => "No Effect");
        }

        //Sealing the set defaults that are common across all things so we don't accidentally override
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            comboResetTime = 120;
            SetDefaults2();
        }

        public virtual void SetDefaults2()
        {

        }

        public virtual void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
            comboPlayer.ComboWaitTime = comboResetTime;

            int combo = comboPlayer.ComboCounter;
            int dir = comboPlayer.ComboDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            comboPlayer.IncreaseCombo();
        }

        public virtual void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == -1)
                return;

            //Only do the swinging initialization if it is a swing projectile lol
            var proj = ModContent.GetModProjectile(type);
            if (proj is BaseSwingProjectileV2)
            {
                DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
                SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
                int combo = comboPlayer.StaminaComboCounter;
                int dir = comboPlayer.ComboDirection;

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                    player.whoAmI, ai1: dir, ai2: combo);
                comboPlayer.IncreaseCombo();
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
            if (player.altFunctionUse == 2)
            {
                if (dashPlayer.CanConsume(staminaCost))
                {
                    comboPlayer.ComboWaitTime = comboResetTime;
                    dashPlayer.Consume(staminaCost);
                    ShootSwingStamina(player, source, position, velocity, staminaProjectileShoot, damage, knockback);
                }

            }
            else
            {
                ShootSwing(player, source, position, velocity, type, damage, knockback);
            }

            return false;
        }
    }
}

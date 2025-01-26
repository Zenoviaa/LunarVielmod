using Microsoft.Xna.Framework;
using Stellamod.Common.Players;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Explosions;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public enum MeleeWeaponType
    {
        Sword,
        Knives
    }
    public abstract class BaseSwingItem : ClassSwapItem
    {
        public int comboWaitTime = 60;
        public int maxCombo;
        public int maxStaminaCombo;
        public int staminaProjectileShoot;
        public int staminaToUse;
        public MeleeWeaponType meleeWeaponType;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new TooltipLine(Mod, "WeaponType", LangText.Common("WeaponType" + meleeWeaponType.ToString()));
            line.OverrideColor = ColorFunctions.GreatswordWeaponType;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "BasicSlash", LangText.Common("BasicSlash", LangText.Item(this, "BasicSlash")));
            line.OverrideColor = new Color(124, 187, 80);
            tooltips.Add(line);

            line = new TooltipLine(Mod, "StaminaSlash", LangText.Common("StaminaSlash", LangText.Item(this, "StaminaSlash")));
            line.OverrideColor = new Color(187, 80, 124);
            tooltips.Add(line);
        }
        public virtual void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ComboPlayer comboPlayer = player.GetModPlayer<ComboPlayer>();
            comboPlayer.ComboWaitTime = comboWaitTime;

            int combo = comboPlayer.ComboCounter;
            int dir = comboPlayer.ComboDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            comboPlayer.IncreaseCombo(maxCombo: maxCombo);
        }

        public virtual void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ComboPlayer comboPlayer = player.GetModPlayer<ComboPlayer>();
            comboPlayer.ComboWaitTime = comboWaitTime;
            comboPlayer.ConsumeStamina(staminaToUse);

            int combo = comboPlayer.StaminaComboCounter;
            if (combo >= maxStaminaCombo)
                combo = 0;

            int dir = comboPlayer.ComboDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            ShootStaminaEffect(player, source, position, velocity, type, damage, knockback);
            comboPlayer.IncreaseStaminaCombo(maxStaminaCombo: maxStaminaCombo);
        }

        public virtual void ShootStaminaEffect(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<BasicStaminaExplosion>(), damage, knockback);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ComboPlayer comboPlayer = player.GetModPlayer<ComboPlayer>();
            if (player.altFunctionUse == 2 && comboPlayer.CanUseStamina(staminaToUse))
            {
                ShootSwingStamina(player, source, position, velocity, staminaProjectileShoot, damage, knockback);
            }
            else
            {
                ShootSwing(player, source, position, velocity, type, damage, knockback);
            }

            return false;
        }
    }
}

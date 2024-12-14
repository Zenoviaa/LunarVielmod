using Microsoft.Xna.Framework;
using Stellamod.Common.Players;
using Stellamod.Items;
using Stellamod.Visual.Explosions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public abstract class BaseSwingItem : ClassSwapItem
    {
        public int comboWaitTime = 60;
        public int maxCombo;
        public int maxStaminaCombo;
        public int staminaProjectileShoot;
        public int staminaToUse;
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

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.SwingSystem
{
    public abstract class BaseSwingItem : ModItem
    {
        public int comboResetTime = 60;
        public int staminaCost = 1;
        public int staminaProjectileShoot;
        //Sealing the set defaults that are common across all things so we don't accidentally override
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 126;
            Item.useAnimation = 126;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            comboResetTime = 60;
            SetDefaults2();
        }

        public virtual void SetDefaults2()
        {

        }

        public virtual void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayer comboPlayer = player.GetModPlayer<SwingPlayer>();
            comboPlayer.ComboWaitTime = comboResetTime;

            int combo = comboPlayer.ComboCounter;
            int dir = comboPlayer.ComboDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            comboPlayer.IncreaseCombo();
        }

        public virtual void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayer comboPlayer = player.GetModPlayer<SwingPlayer>();
            comboPlayer.ComboWaitTime = comboResetTime;
            comboPlayer.ConsumeStamina(staminaCost);

            int combo = comboPlayer.StaminaComboCounter;
            int dir = comboPlayer.ComboDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            comboPlayer.IncreaseCombo();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
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
                ShootSwing(player, source, position, velocity, type, damage, knockback);
            }

            return false;
        }
    }
}

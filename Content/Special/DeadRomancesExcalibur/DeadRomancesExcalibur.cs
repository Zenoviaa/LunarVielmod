using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomancesExcalibur : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 10;
        Item.shoot = ModContent.ProjectileType<DeadRomancesExcaliburSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<DeadRomanceParryingBlade>();
        staminaCost = 1;
        staminaDamageMultiplier = 2;
        comboResetTime = 60;
        meleeWeaponType = MeleeWeaponType.Greatsword;
    }
    public override void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        DeadRomancePlayer romancePlayer = player.GetModPlayer<DeadRomancePlayer>();
        if (player.HasBuff<HeavenlyLove>())
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeadRomanceAscendedDash>(),
                damage, knockback, player.whoAmI);
            return;
        }
        if (romancePlayer.useGreatBlade)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeadRomanceGreatBlade>(),
                damage, knockback, player.whoAmI, ai1: -1);
            romancePlayer.ConsumeGreatBlade();
        }
        else
        {
            base.ShootSwing(player, source, position, velocity, type, damage, knockback);
        }
    }

    public override void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.HasBuff<HeavenlyLove>() || true)
        {
            type = ModContent.ProjectileType<DeadRomancesExcaliburParrySlash>();
        }

        base.ShootSwingStamina(player, source, position, velocity, type, damage, knockback);

    }
}

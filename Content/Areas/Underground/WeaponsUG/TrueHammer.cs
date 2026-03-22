using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;

public class TrueHammer : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 8;
        Item.shoot = ModContent.ProjectileType<TrueHammerSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<IvynAxeStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MinersGold>());
    }
}


public class TrueHammerSlash : BaseSwingProjectileV2
{
    private float _hitCount;
    private bool _hit;
    private bool _playSound;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddHammerSwingStyle(this);
        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
    }

    private float GetTrailWidth(float interpolant)
    {
        return EasingFunction.QuadraticBump(interpolant) * 8;
    }

    public override void AI()
    {
        base.AI();
        if (!_playSound && Interpolant >= 0.5f)
        {

            _playSound = true;
        }
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[Projectile.owner];
        _hitCount++;
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            Bounce(8);
            FXUtil.ShakeCamera(target.Center, 1024, 16);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
            _hit = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (!_hit)
        {
            modifiers.Knockback *= 0.5f;
        }
        else
        {
            modifiers.Knockback *= 2;
        }

        if (ComboIndex == ComboCount - 1)
        {
            modifiers.FinalDamage += 0.5f;
        }
    }
}
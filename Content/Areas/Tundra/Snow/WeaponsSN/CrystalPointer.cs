using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;

public class CrystalPointer : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 10;
        Item.shoot = ModContent.ProjectileType<CrystalPointerSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<CrystalPointerStaminaStab>();
        meleeWeaponType = MeleeWeaponType.Spear;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankSword>();
    }
}

public class CrystalPointerSlash : BaseSwingProjectileV2
{
    public override void DefineCombo()
    {
        base.DefineCombo();
        var swingTrailer = TrailPresets.Auroran;
        swingTrailer.invert = ComboIndex % 2 != 0;
        Trailer = swingTrailer;

        SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
        SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
        SoundStyle nSpin = SoundRegistry.NSwordSpin1;
        spearSlash1.PitchVariance = 0.25f;
        spearSlash2.PitchVariance = 0.25f;
        nSpin.PitchVariance = 0.2f;
        Add(new OvalSwing
        {
            Duration = 28,
            XSwingRadius = 100,
            YSwingRadius = 50,
            SwingDegrees = MathHelper.ToRadians(45),
            Easing = (float lerpValue) => EasingFunction.InOutExpo(lerpValue),
            Sound = spearSlash1,
        });

        Add(new OvalSwing
        {
            Duration = 28,
            XSwingRadius = 100,
            YSwingRadius = 50,
            SwingDegrees = MathHelper.Pi / 2f,
            Easing = (float lerpValue) => EasingFunction.InOutExpo(lerpValue),
            Sound = spearSlash1,
        });

        Add(new ThrustSwing
        {
            Duration = 12,
            ThrowDistance = 90,
            Easing = (float lerpValue) => EasingFunction.QuickOutSlowIn(lerpValue),
            Sound = spearSlash2
        });

        Add(new ThrustSwing
        {
            Duration = 12,
            ThrowDistance = 90,
            Easing = (float lerpValue) => EasingFunction.QuickOutSlowIn(lerpValue),
            Sound = spearSlash2
        });

        Add(new OvalSwing
        {
            Duration = 24,
            XSwingRadius = 100,
            YSwingRadius = 50,
            SwingDegrees = MathHelper.Pi / 2f,
            Easing = (float lerpValue) => EasingFunction.InOutExpo(lerpValue),
            Sound = spearSlash1,
        });

        Add(new OvalSwing
        {
            Duration = 24,
            XSwingRadius = 100,
            YSwingRadius = 50,
            SwingDegrees = MathHelper.Pi / 2f,
            Easing = (float lerpValue) => EasingFunction.InOutExpo(lerpValue),
            Sound = spearSlash1,
        });
        Add(new OvalSwing
        {
            Duration = 60,
            SwingDegrees = 360 * 4,
            XSwingRadius = 64,
            YSwingRadius = 64,
            HitCount = 8,
            Easing = (float lerpValue) => lerpValue,
            Sound = nSpin
        });

        Add(new ThrustSwing
        {
            Duration = 30,
            ThrowDistance = 128,
            Easing = (float lerpValue) => EasingFunction.QuickOutSlowIn(lerpValue),
            Sound = spearSlash2
        });

        Add(new ThrustSwing
        {
            Duration = 60,
            ThrowDistance = 200,
            Easing = (float lerpValue) => EasingFunction.QuickOutSlowIn(lerpValue),
            Sound = spearSlash2
        });

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        useAfterImage = true;
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 64, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.LightSkyBlue, Color.Blue, 0.5f), Color.Blue, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Violet, ratio);
    }

    public override void AI()
    {
        base.AI();
        glowColor = Color.Lerp(Color.Transparent, Color.LightBlue, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}

public class CrystalPointerStaminaStab : BaseSwingProjectileV2
{
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle spearSlash1 = SoundRegistry.SpearSlash1;
        SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
        SoundStyle nSpin = SoundRegistry.NSwordSpin1;
        spearSlash1.PitchVariance = 0.25f;
        spearSlash2.PitchVariance = 0.25f;
        nSpin.PitchVariance = 0.2f;
        Add(new OvalSwing
        {
            Duration = 60,
            XSwingRadius = 128,
            YSwingRadius = 48,
            SwingDegrees = MathHelper.ToRadians(2100),
            Easing = (float lerpValue) => lerpValue,
            Sound = nSpin,
        });

        Add(new ThrustSwing
        {
            Duration = 20,
            ThrowDistance = 100,
            Easing = (float lerpValue) => EasingFunction.QuickOutSlowIn(lerpValue),
            Sound = spearSlash2
        });
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
        int combo = ComboIndex + 1;
        int dir = comboPlayer.ComboDirection;


        if (ComboIndex < 1 && this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, (Main.MouseWorld - Owner.Center), Projectile.type, Projectile.damage, Projectile.knockBack,
                        Owner.whoAmI, ai2: combo, ai1: dir);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (ComboIndex == 0)
        {
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.3f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
        }

        if (ComboIndex == 1)
        {
            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);
            modifiers.FinalDamage *= 3;
        }
    }
}
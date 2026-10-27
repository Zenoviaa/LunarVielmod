using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;


public class IceFissureHammer : BaseSwingItemV2
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureHammer.KEY;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 12;
        Item.shoot = ModContent.ProjectileType<IceFissureHammerSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<IceFissureHammerStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaCost = 2;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(2f, -2f);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}

public class IceFissureSpike : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureSpike.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class IceFissureHammerStaminaSlash : BaseSwingProjectileV2
{
    private FireTrailRenderer _fireTrailRenderer;
    private float _hitCount;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        outlineColor = Color.Yellow;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.OrangeRed;
        bloom.outerBloomColor = Color.DarkRed;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        _fireTrailRenderer = new FireTrailRenderer();
        _fireTrailRenderer.SlashTrailer.TrailWidthFunction = GetTrailWidth;
        additive = true;
        Trailer = _fireTrailRenderer.SlashTrailer;
        useAfterImage = true;
        glowAfterImageColor = Color.Red;


        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.4f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.4f;

        Add(new OvalSwing
        {
            Duration = 66,
            SwingDegrees = 310,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        Add(new OvalSwing
        {
            Duration = 54,
            SwingDegrees = 330,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash2,
            HitCount = 2
        });

        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
    }

    public float GetTrailWidth(float interpolant)
    {
        return MathHelper.SmoothStep(4, 24, interpolant) * MathF.Sin(interpolant * 8);
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 32, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }


    public override void AI()
    {
        base.AI();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        _hitCount++;
        target.AddBuff(BuffID.OnFire, 60);
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            Bounce(8);
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.15f, 2, 12);
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
        if (IsFinishingSwing())
        {
            modifiers.FinalDamage += 0.5f;
        }
    }
}













public class IceFissureHammerSlash : BaseSwingProjectileV2
{
    private FireTrailRenderer _fireTrailRenderer;
    private float _hitCount;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        outlineColor = Color.Yellow;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.OrangeRed;
        bloom.outerBloomColor = Color.DarkRed;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        _fireTrailRenderer = new FireTrailRenderer();
        _fireTrailRenderer.SlashTrailer.TrailWidthFunction = GetTrailWidth;
        additive = true;
        Trailer = _fireTrailRenderer.SlashTrailer;
        useAfterImage = true;
        glowAfterImageColor = Color.Red;


        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.4f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.4f;

        Add(new OvalSwing
        {
            Duration = 66,
            SwingDegrees = 310,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        Add(new OvalSwing
        {
            Duration = 54,
            SwingDegrees = 330,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash2,
            HitCount = 2
        });

        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
    }

    public float GetTrailWidth(float interpolant)
    {
        return MathHelper.SmoothStep(4, 24, interpolant) * MathF.Sin(interpolant * 8);
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 32, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }


    public override void AI()
    {
        base.AI();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        _hitCount++;
        target.AddBuff(BuffID.OnFire, 60);
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            Bounce(8);
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.15f, 2, 12);
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
        if (IsFinishingSwing())
        {
            modifiers.FinalDamage += 0.5f;
        }
    }
}













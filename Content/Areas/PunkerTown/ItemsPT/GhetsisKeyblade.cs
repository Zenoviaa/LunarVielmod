using ReLogic.Content;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

/// <summary>
/// Long Keysword - A slower sword but hits with flames and electricity on swing, and the lightning can chain foes, also has an extender because its hardmode, 
/// Stamina(2) : stasis, which the sword does an underhand strike that launches enemies in the sky, but they stay there as they are kept in a firebubble fpr 5 seconds(doesn't work on bosses)
/// </summary>
public class GhetsisKeyblade : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 54;
        Item.shoot = ModContent.ProjectileType<GhetsisKeybladeSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<GhetsisKeybladeStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 2;
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MarshScrap>());
    }
}


public class GhetsisKeybladeSlash : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    public bool Hit;
    public bool AuroraProj1;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddSwordSwingStyle(this);
        var swingTrailer = TrailPresets.Auroran;
        swingTrailer.invert = ComboIndex % 2 != 0;
        Trailer = swingTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        useAfterImage = true;
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return base.RequestHologramTexture();
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
        bloomScale = MathHelper.Lerp(0.08f, 0f, EasingFunction.InExpo(Interpolant));
        if (!AuroraProj1 && Interpolant > 0.5f)
        {
            AuroraProj1 = true;
        }

        if (Timer % 16 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.innerColor = Color.White;
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.fast = true;
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];

            index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= swingTrailCache.Length;

            spawnPos = swingTrailCache[index];
            Vector2 spawnPos2 = swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;


            if (Main.rand.NextBool(12))
            {
                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.innerColor = Color.LightSkyBlue;
                dp.outerColor = Color.Violet;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Frostburn, 120);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class GhetsisKeybladeStaminaSlash : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    public bool Hit;
    public bool AuroraProj1;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddSwordSwingStyle(this);
        var swingTrailer = TrailPresets.Auroran;
        swingTrailer.invert = ComboIndex % 2 != 0;
        Trailer = swingTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        useAfterImage = true;
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return base.RequestHologramTexture();
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
        bloomScale = MathHelper.Lerp(0.08f, 0f, EasingFunction.InExpo(Interpolant));
        if (!AuroraProj1 && Interpolant > 0.5f)
        {
            AuroraProj1 = true;
        }

        if (Timer % 16 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.innerColor = Color.White;
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.fast = true;
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];

            index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= swingTrailCache.Length;

            spawnPos = swingTrailCache[index];
            Vector2 spawnPos2 = swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;


            if (Main.rand.NextBool(12))
            {
                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.innerColor = Color.LightSkyBlue;
                dp.outerColor = Color.Violet;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Frostburn, 120);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class GhetsisBubble : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
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
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;
public class IvynChakrams : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 2;
        Item.ArmorPenetration = 5;
        Item.DamageType = DamageClass.Summon;
        Item.shoot = ModContent.ProjectileType<IvynChakramsSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<IvynChakramsStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Chakrams;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankSafunai>();
    }
}

public class IvynSpike : ModProjectile
{
    float Gravity => 0.35f;
    ref float Timer => ref Projectile.ai[0];
    bool IsGrounded
    {
        get => Projectile.ai[1] == 1;
        set => Projectile.ai[1] = value ? 1 : 0;
    }
    float OutScale => EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 16;
        Projectile.StaticPiercingImmunityTime = 20;
        Projectile.timeLeft = 360;
        Projectile.friendly = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(6))
        {
            var pos = Projectile.position;
            var vel = Main.rand.NextVector2Circular(2, 2);
            var d = Dust.NewDustPerfect(pos, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            d.noGravity = true;
        }

        if (!IsGrounded)
        {
            Projectile.velocity.X *= 0.96f;
            Projectile.rotation += Projectile.velocity.Length() * 0.04f * MathF.Sign(Projectile.velocity.X);
        }
        else
        {
            Projectile.velocity.X *= 0.9f;
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, 0.1f);
        }

  
        Projectile.velocity.Y += Gravity;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!IsGrounded)
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Foresty, new Vector2(5f, 15f));
            fac.particleCount /= 3;
            ParticleUtils.CreateSwirlingDustCircle(fac);
        }
        IsGrounded = true;
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Poisoned, 15);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.scale = OutScale;
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.RosyBrown, Color.Transparent, 0.3f);
        var drawer = Projectile.Drawer;

        var outlineDrawer = drawer;
        outlineDrawer.VerticalFrame(1, Main.projFrames[Type]);
        outlineDrawer.color = Color.Lerp(Color.Transparent, Color.Green, 0.85f) * ExtraMath.Osc(0.9f, 1f, speed: 6f);
        outlineDrawer.color.A = 0;
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
    }
}

static file class IvynChakramsCommon
{
    public static SlashTrailer CreateChakramTrailer()
    {
        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.White;
        blackFireShader.OuterColor = Color.RosyBrown;
        blackFireShader.BackColor = Color.DarkGreen;
        SlashTrailer devilsPeak = new SlashTrailer
        {
            Shader = blackFireShader,
            TrailWidthFunction = (interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 35;
            },
            TrailColorFunction = (interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.DarkGreen, Color.RosyBrown, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }
        };
        return devilsPeak;
    }
}
public class IvynChakramsStaminaSlash : BaseSwingProjectileV2
{
    bool _hit;
    bool _spawnedClone;
    public override void DefineCombo()
    {
        base.DefineCombo();
        trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 64, ySwingRadius: 64, hitCount: 3, swingDegrees: 375);
        comboBuilder.AddToProjectile(this);


        Trailer = IvynChakramsCommon.CreateChakramTrailer();
        useAfterImage = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer % 32 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<IvynSpike>(Projectile);
                firer.position = Owner.Center;
                firer.velocity = Projectile.velocity;
                firer.velocity = firer.velocity.Resize(12).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.6f, 1.6f);
                firer.velocity.Y -= 4;
                firer.damage *= 2;
                firer.knockback = 0;
                firer.New();
            }
        }

        if (Main.rand.NextBool(144))
        {
            MakeLeaf(Projectile.Center + Main.rand.NextVector2Circular(8, 8));
        }
        if (!_spawnedClone && Interpolant > 0.1f)
        {
            if (IsFinishingSwing())
            {
                Owner.velocity += Projectile.velocity * 0.1f;
            }
            MirrorProjectile();
            _spawnedClone = true;
        }
        glowColor = Color.Lerp(Color.Transparent, Color.RosyBrown * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    void MakeLeaf(Vector2 pos)
    {
        var vel = Main.rand.NextVector2Circular(6, 6);
        Particles.IvynLeaf.Spawn(new()
        {
            position = pos,
            velocity = vel,
            timeLeft = Main.rand.Next(90, 120),
        });
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            for (var i = 0; i < 1; i++)
            {
                MakeLeaf(target.Center + Main.rand.NextVector2Circular(8, 8));
            }

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(target.Center, Projectile.Center, TriColorPalette.Foresty, new Vector2(5f, 15f));
            fac.particleCount /= 3;
            ParticleUtils.CreateSwirlingDustCircle(fac);
            _hit = true;
        }

        target.AddBuff(BuffID.Poisoned, 15);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }

}
public class IvynChakramsSlash : BaseSwingProjectileV2
{
    bool _hit;
    bool _spawnedClone;
    public override void DefineCombo()
    {
        base.DefineCombo();
        trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        for (int i = 0; i < 2; i++)
        {
            comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 64, ySwingRadius: 64, hitCount: 3, swingDegrees: 375);
        }
        comboBuilder.AddChakramUppercut(duration: 30, xSwingRadius: 96, hitCount: 3, swingDegrees: 199);
        comboBuilder.AddChakramThrow(throwDistance: 96);
        comboBuilder.AddToProjectile(this);

        Trailer = IvynChakramsCommon.CreateChakramTrailer();
        useAfterImage = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer % 32 == 0)
        {

        }
        if (Main.rand.NextBool(144))
        {
            MakeLeaf(Projectile.Center + Main.rand.NextVector2Circular(8, 8));
        }
        if (!_spawnedClone && Interpolant > 0.1f)
        {
            if (IsFinishingSwing())
            {
                Owner.velocity += Projectile.velocity * 0.1f;
            }
            MirrorProjectile();
            _spawnedClone = true;
        }
        glowColor = Color.Lerp(Color.Transparent, Color.RosyBrown * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    void MakeLeaf(Vector2 pos)
    {

        var vel = Main.rand.NextVector2Circular(6, 6);
        Particles.IvynLeaf.Spawn(new()
        {
            position = pos,
            velocity = vel,
            timeLeft = Main.rand.Next(90, 120),
        });
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            for (var i = 0; i < 1; i++)
            {
                MakeLeaf(target.Center + Main.rand.NextVector2Circular(8, 8));
            }

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(target.Center, Projectile.Center, TriColorPalette.Foresty, new Vector2(5f, 15f));
            fac.particleCount /= 3;
            ParticleUtils.CreateSwirlingDustCircle(fac);
            _hit = true;
        }

        target.AddBuff(BuffID.Poisoned, 15);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}

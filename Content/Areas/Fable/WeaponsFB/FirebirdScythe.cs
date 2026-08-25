using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class FirebirdScythe : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 12;
        Item.shoot = ModContent.ProjectileType<FirebirdScytheSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<FirebirdScytheSpinningSlash>();
        meleeWeaponType = MeleeWeaponType.Scythe;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankSword>();
    }
}

public class FirebirdBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 384;
        Projectile.height = 384;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        Main.dust[dust].noGravity = true;

        if (Timer > 45)
        {
            if (Timer % 3 == 0)
            {
                Player owner = Main.player[Projectile.owner];
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(owner.Center + Main.rand.NextVector2Circular(80, 80), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }
        }
        Timer++;
        if (Timer == 1)
        {
            for (float i = 0; i < 3; i++)
            {
                var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 4 * MathHelper.Lerp(15, 1f, i / 3f));
                donutParticle.Scale *= MathHelper.Lerp(0.3f, 2f, i / 3f);
                donutParticle.Velocity *= 0.1f;
                donutParticle.innerColor = Color.Red;
                donutParticle.outerColor = Color.DarkRed;

                donutParticle.noStretch = true;
            }

            SoundStyle impact = AssetManager.GetSound("Fire/FireballShoot1");
            impact.PitchVariance = 0.3f;
            SoundEngine.PlaySound(impact, Projectile.position);

            SoundStyle sound = AssetManager.GetSound("Fire/FireExplosion1");
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, Projectile.position);

            float numDust = 24;
            for (float n = 0; n < numDust; n++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.OrangeRed,
                    outerColor = Color.Red,
                    scaleRange = new Vector2(1f, 2f),
                    gravity = 0.05f

                };
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16) * 2f;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            for (float f = 0; f < 10; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(64, 64);
                FXUtil.GlowStretch(Projectile.Center, velocity);
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (float f = 0; f < 4; f++)
            {
                Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            Player owner = Main.player[Projectile.owner];
            owner.Hurt(new PlayerDeathReason(), 30, 1);
            ShakeScreenPosition.Shake = 12;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire, 120);
    }
}
public class FirebirdScytheSpinningSlash : BaseSwingProjectileV2
{
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle nSpin = SoundRegistry.NSwordSpin1;
        nSpin.PitchVariance = 0.3f;
        Add(new OvalSwing
        {
            Duration = 120,
            SwingDegrees = 360 * 6,
            XSwingRadius = 64,
            YSwingRadius = 64,
            HitCount = 8,
            Easing = (float lerpValue) => lerpValue,
            Sound = nSpin
        });

        useAfterImage = true;
    }

    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.Lerp(0, 64, completionRatio) * EasingFunction.QuadraticBump(Interpolant) * MathF.Sin(completionRatio * 8);
    }

    private Color GetTrailColor(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
        return trailColor;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.Goldenrod;
        laserShader.InnerColor = Color.Red;
        laserShader.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, laserShader);
    }
    public override void AI()
    {
        base.AI();
        if (Timer == 1)
        {
            SoundStyle charge = AssetManager.GetSound("Fire/FireballCharge1");
            charge.PitchVariance = 0.3f;
            SoundEngine.PlaySound(charge, Projectile.position);
        }

        if (Timer % 24 == 0)
        {
            Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2CircularEdge(256, 256);
            Vector2 velocity = Projectile.Center - spawnPoint;
            velocity *= 0.05f;
            var p = FXUtil.GlowStretch(spawnPoint, velocity);
            p.InnerColor = Color.Yellow;
            p.OuterGlowColor = Color.Red;
            p.VectorScale *= 0.4f;
        }

        glowColor = Color.Lerp(Color.Transparent, Color.Red * 1f, Interpolant);
        growScale = MathHelper.Lerp(0f, 0.7f, Interpolant);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero,
                ModContent.ProjectileType<FirebirdBoom>(), Projectile.damage * 10, Projectile.knockBack, Projectile.owner);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Player owner = Main.player[Projectile.owner];
        Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 drawOrigin = glowTexture.Size() * 0.5f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2 drawCenter = owner.Center - Main.screenPosition;
        Color glowColor = Color.Red;
        glowColor *= ExtraMath.Osc(0.5f, 1f);
        glowColor *= MathHelper.SmoothStep(0f, 1f, Interpolant);
        glowColor.A = 0;
        spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
        return base.PreDraw(ref lightColor);
    }
}
public class FirebirdScytheSlash : BaseSwingProjectileV2
{
    private bool _hit;
    private bool _playedSound;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddScytheSwingStyle(this);
        useAfterImage = true;
    }


    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.Lerp(0, 64, completionRatio) * EasingFunction.QuadraticBump(Interpolant) * (MathF.Sin(completionRatio * 16) * 0.5f + 0.5f);
    }

    private Color GetTrailColor(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
        return trailColor;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.Goldenrod;
        laserShader.InnerColor = Color.Red;
        laserShader.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, laserShader);
    }

    public override void AI()
    {
        base.AI();
        glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        if (Timer % 16 == 0 && Interpolant >= 0.3f)
        {
            if (!_playedSound)
            {
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadianceCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
                _playedSound = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            Vector2 position = target.Center;
            Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Yellow,
                    outerColor: Color.Orange,
                    fadeToColor: Color.Red,
                    distortOut: true);


            }

            _hit = true;
        }
        if (ComboIndex == ComboCount - 1)
        {
            SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadianceCast1;
            fireSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fireSound, Projectile.position);
            for (float f = 0; f < 8; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
            }

        }

        target.AddBuff(BuffID.OnFire, 120);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }

        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        SoundStyle scytheHit;

        int rand = Main.rand.Next(0, 3);
        switch (rand)
        {
            default:
            case 0:
                scytheHit = AssetRegistry.Sounds.Melee.ScytheHit1;
                break;
            case 1:
                scytheHit = AssetRegistry.Sounds.Melee.ScytheHit2;
                break;
        }

        scytheHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(scytheHit, Projectile.position);
    }
}

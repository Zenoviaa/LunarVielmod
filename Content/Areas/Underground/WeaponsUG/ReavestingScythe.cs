using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;

public class ReavestingScythe : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 18;
        Item.shoot = ModContent.ProjectileType<ReavestingScytheSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<ReavestingScytheSpinningSlash>();
        meleeWeaponType = MeleeWeaponType.Scythe;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MinersGold, BlankSword>();
    }
}

public class ReavestingScytheSpinningSlash : BaseSwingProjectileV2
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
        laserShader.LaserColor = Color.White * 0.3f;
        laserShader.InnerColor = Color.DarkGray * 0.3f;
        laserShader.OuterColor = Color.Black * 0.3f;
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
            Vector2 velocity = spawnPoint - Projectile.Center;
            velocity *= 0.05f;
            var p = FXUtil.GlowStretch(spawnPoint, velocity);
            p.InnerColor = Color.White;
            p.OuterGlowColor = Color.LightBlue;
            p.VectorScale *= 0.4f;
        }

        if (Timer % 32 == 0)
        {
            Vector2 pos = Owner.Bottom;
            float dir = Main.rand.NextBool(2) ? 1 : -1;
            Vector2 velocity = Vector2.UnitX * dir * 8;
            ThickSmokeParticle ts = ThickSmokeParticle.Spawn(pos, velocity, Scale: 2);
            ts.expand = true;
            ts.color = Color.Gray * 0.6f;
        }

        if (Timer % 16 == 0)
        {
            Vector2 pos = Owner.Bottom;
            float dir = Main.rand.NextBool(2) ? 1 : -1;
            Vector2 velocity = Vector2.UnitX * dir * 6;
            velocity.Y -= 5 * Main.rand.NextFloat(0.4f, 0.8f);
            pos += Main.rand.NextVector2Circular(24, 24);
            Dust d = Dust.NewDustPerfect(pos, DustID.Cloud, velocity);
            d.noGravity = false;

        }


        float strength = 0.05f;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (NPCSets.Heavy[npc.type])
                continue;
            if (npc.friendly)
                continue;

            GlobalNPCSucker npcSucker = npc.GetGlobalNPC<GlobalNPCSucker>();
            float dist = Vector2.Distance(Owner.Center, npc.Center);
            if (dist <= 444)
            {
                if (Timer % 128 == 0)
                {
                    if (this.OwnedByLocalClient())
                        npc.SimpleStrikeNPC(Projectile.damage, 1);
                }

                Vector2 suckPosition = Owner.Center;

                Vector2 diff = npc.Center - suckPosition;
                diff = diff.SafeNormalize(Vector2.Zero) * 256;
                Vector2 velocity = Vector2.Lerp(Vector2.Zero, diff, strength) * npc.knockBackResist;
                Vector2 diffVelocity = velocity - npcSucker.SuckVelocity;
                npcSucker.SuckVelocity += diffVelocity;
            }
        }
        glowColor = Color.Lerp(Color.Transparent, Color.Red * 1f, Interpolant);
        growScale = MathHelper.Lerp(0f, 0.7f, Interpolant);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        Player owner = Main.player[Projectile.owner];
        Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 drawOrigin = glowTexture.Size() * 0.5f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2 drawCenter = owner.Center - Main.screenPosition;
        Color glowColor = Color.White;
        glowColor *= ExtraMath.Osc(0.5f, 1f);
        glowColor *= MathHelper.SmoothStep(0f, 1f, Interpolant);
        glowColor *= 0.4f;
        glowColor.A = 0;
        spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
        return base.PreDraw(ref lightColor);
    }
}
public class ReavestingScytheSlash : BaseSwingProjectileV2
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
        return MathHelper.Lerp(0, 96, completionRatio) * EasingFunction.QuadraticBump(Interpolant);
    }

    private Color GetTrailColor(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.Black, p) * EasingFunction.QuadraticBump(p);
        return trailColor;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White * 0.3f;
        laserShader.InnerColor = Color.Gray * 0.3f;
        laserShader.OuterColor = Color.DarkGray * 0.3f;
        laserShader.LaserTexture = TrailRegistry.SmallWhispyTrail;
        laserShader.BloomTexture = TrailRegistry.WhispyTrail;
        TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, laserShader);
    }

    public override void AI()
    {
        base.AI();
        glowColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(Interpolant));
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
                    innerColor: Color.White,
                    outerColor: Color.Gray,
                    fadeToColor: Color.DarkGray,
                    distortOut: true);


            }

            _hit = true;
        }
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
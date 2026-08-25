using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
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

namespace Stellamod.Content.Areas.Terror.WeaponsTR;


public class PointingFingers : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 7;
        Item.DamageType = DamageClass.Summon;
        Item.shoot = ModContent.ProjectileType<PointingFingersSlash>();
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<
            TerrorFragments, 
            BlankSafunai>();
    }
}

public class PointingFingersSlash : BaseSwingProjectileV2,
    IDrawSanguineBlood
{
    private bool _hit;
    private bool _spawnedClone;
    public override void DefineCombo()
    {
        base.DefineCombo();
        trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 64, ySwingRadius: 64, hitCount: 3, swingDegrees: 720);
        comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 64, ySwingRadius: 64, hitCount: 3, swingDegrees: 720);
        comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 96, ySwingRadius: 96, hitCount: 3, swingDegrees: 720);
        comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 96, ySwingRadius: 96, hitCount: 3, swingDegrees: 720);
        comboBuilder.AddChakramUppercut(duration: 24, xSwingRadius: 96, hitCount: 3, swingDegrees: 135);
        comboBuilder.AddChakramThrow(throwDistance: 128);
        comboBuilder.AddToProjectile(this);
        useAfterImage = true;
    }
    private Color ColorFunction(float completionRatio)
    {
        return Color.White;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 0, completionRatio) * MathF.Sin(completionRatio * 4);
    }



    public override void AI()
    {
        base.AI();
        if (!_spawnedClone && Interpolant > 0.5f)
        {
            if (IsFinishingSwing())
            {
                Owner.velocity += Projectile.velocity * 0.1f;
            }
            MirrorProjectile();
            _spawnedClone = true;
        }
        if (Main.rand.NextBool(100))
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.White,
                outerColor = Color.Red,
                scaleRange = new Vector2(0.4f, 0.6f)
            };
            DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
        }
        glowColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
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
                    outerColor: Color.Red,
                    fadeToColor: Color.DarkRed,
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
                EmberParticle ep = LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
                ep.innerColor = Color.White;
                ep.outerColor = Color.Red;
                ep.fadeToColor = Color.DarkRed;
            }

        }

        target.AddBuff(BuffID.Bleeding, 120);
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

    public void DrawToSanguineMask(SpriteBatch spriteBatch)
    {
        if (swingTrailCache == null)
            return;

        var flamingTrailShader = BasicLaserAlphaShader.Instance;
        flamingTrailShader.Tiling = Vector2.One * 1;
        flamingTrailShader.LaserTexture = TrailRegistry.LightningTrail2;
        flamingTrailShader.BlendState = BlendState.AlphaBlend;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, ColorFunction, WidthFunction, flamingTrailShader);
    }
}

using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;


public class HypnotizingChakrams : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 5;
        Item.DamageType = DamageClass.Summon;
        Item.shoot = ModContent.ProjectileType<HypnotizingChakramsSlash>();
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankSafunai>();
    }
}

public class HypnotizingChakramsSlash : BaseSwingProjectileV2
{
    private bool _hit;
    private bool _spawnedClone;
    public override void DefineCombo()
    {
        base.DefineCombo();
        trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        for(int i = 0; i < 2; i++)
        {
            comboBuilder.AddChakramSpin2(duration: 18, xSwingRadius: 64, ySwingRadius: 64, hitCount: 3, swingDegrees: 435);
        }
        for (int i = 0; i < 2; i++)
        {
            comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 80, ySwingRadius: 80, hitCount: 3, swingDegrees: 435);
        }
        comboBuilder.AddChakramSpin2(duration: 40, xSwingRadius: 128, ySwingRadius: 128, hitCount: 3, swingDegrees: 435);
        comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 80, ySwingRadius: 80, hitCount: 3, swingDegrees: -255);
        comboBuilder.AddChakramUppercut(duration: 30, xSwingRadius: 96, hitCount: 3, swingDegrees: 199);
        comboBuilder.AddChakramThrow(throwDistance: 128);
        comboBuilder.AddToProjectile(this);


        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.LightBlue;
        blackFireShader.OuterColor = Color.DarkBlue;
        blackFireShader.InnerEmitColor = Color.LightBlue * 0.2f;
        blackFireShader.OuterEmiteColor = Color.Purple;
        blackFireShader.BloomTexture = TrailRegistry.BeamTrail;
        blackFireShader.PrimaryTexture2 = TrailRegistry.Clouds3;
        //blackFireShader.PrimaryTexture2 = TrailRegistry.StarTrail;
        SlashTrailer devilsPeak = new SlashTrailer
        {
            Shader = blackFireShader,
            TrailWidthFunction = (interpolant) =>
            {
                return MathHelper.SmoothStep(4, 8, interpolant) * EasingFunction.QuadraticBump(Interpolant);
            },
            TrailColorFunction = (interpolant) =>
            {
                return Color.Lerp(Color.DeepSkyBlue, Color.White, interpolant) * EasingFunction.QuadraticBump(Interpolant);
            }

        };

        //   outlineColor = Color.Yellow;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.LightBlue;
        bloom.outerBloomColor = Color.Purple;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        Trailer = devilsPeak;
        useAfterImage = true;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 8, ratio) * 4f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Purple * 0.9f, Color.Lerp(Color.DeepSkyBlue, Color.Violet, ExtraMath.Osc(0f, 1f, speed: 24)), ratio);
    }
    public override void AI()
    {
        base.AI();
        if (Timer % 32 == 0)
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
        if (!_spawnedClone && Interpolant > 0.1f)
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
                outerColor = Color.Aquamarine,
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
                    outerColor: Color.Cyan,
                    fadeToColor: Color.DarkBlue,
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
                ep.outerColor = Color.Cyan;
                ep.fadeToColor = Color.DarkBlue;
            }

        }
        target.AddBuff(BuffID.Confused, 15);
        target.AddBuff(BuffID.ShadowFlame, 15);
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

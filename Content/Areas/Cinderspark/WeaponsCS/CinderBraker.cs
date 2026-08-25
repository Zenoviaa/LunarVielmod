using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Swords;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class CinderBraker : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 4;
            Item.shoot = ModContent.ProjectileType<CinderBreakerSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<CinderBreakerStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Knives;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankSword>();
        }
    }


    public class CinderBreakerSlash : BaseSwingProjectileV2
    {
        private FireTrailRenderer _fireTrailRenderer;

        private bool _hit;
        private bool _hasSpawnedSecondKnife;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddKnivesSwingStyle(this);
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
            if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.9f)
            {
                CloneProjectile();
                _hasSpawnedSecondKnife = true;
            }

            bloomScale = MathHelper.Lerp(0.12f, 0f, EasingFunction.InExpo(Interpolant));
            glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 1);
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

                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                }
                if(ComboIndex == 5)
                {
                    float offset = ProjectileHelper.PerformBeamHitscan(target.Top, Vector2.UnitY, 1200);
                    Vector2 spawnPoint = target.Top + new Vector2(0, offset);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint - new Vector2(0, 384 / 2), Vector2.UnitY * 8,
                        ModContent.ProjectileType<CinderBreakerEruption>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                _hit = true;
            }
            target.AddBuff(BuffID.OnFire, 120);
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
    }

    public class CinderBreakerStaminaSlash : BaseSwingProjectileV2
    {
        private FireTrailRenderer _fireTrailRenderer;

        public bool Hit;
        public bool AuroraProj2;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            Add(new OvalSwing
            {
                Duration = 44,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) =>  EasingFunction.GreatswordAnticipation(lerpValue),
                Sound = swingSound1,

            });

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
        private bool _thrust;
        public float thrustSpeed = 5;
        public float stabRange;
        public override void AI()
        {
            base.AI();
            bloomScale = MathHelper.Lerp(0.12f, 0f, EasingFunction.InExpo(Interpolant));
            glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Interpolant > 0.5f && !AuroraProj2)
            {
                SoundStyle soundStyle = SoundRegistry.IceyWind;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 25, baseSize: 0.2f);

                for (float f = 0; f < 12; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.Red);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, shootVelocity,
                        ModContent.ProjectileType<CinderBreakerEruptor>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                }
                AuroraProj2 = true;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire, 180);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = SoundRegistry.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }
    }

    public class CinderBreakerEruptor : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float EruptionCount => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer >= 5)
            {
                Timer = 0;
            }
            if (Timer == 1 && EruptionCount < 5 && Main.myPlayer == Projectile.owner)
            {

                EruptionCount++;

                float offset = ProjectileHelper.PerformBeamHitscan(Projectile.Top, Vector2.UnitY, 1200);
                Vector2 spawnPoint = Projectile.Top + new Vector2(0, offset);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint - new Vector2(0, 384 / 2), Vector2.UnitY * 8,
                    ModContent.ProjectileType<CinderBreakerEruption>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }

    public class CinderBreakerEruption : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 384;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire, 180);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                ShakeScreenPosition.Shake = 4;
                FXUtil.ShakeCamera(Projectile.position, 1024, 8);
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"), Projectile.position);

                SoundStyle shot = AssetRegistry.Sounds.Magic.RadianceCast1;
                shot.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shot, Projectile.Bottom);
                SoundStyle shot2 = SoundID.DD2_BetsyFireballImpact;
                shot2.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shot2, Projectile.Bottom);
                var part = FXUtil.GlowCircleBoom(Projectile.Bottom,
                                  innerColor: Color.Yellow,
                                  glowColor: Color.Orange,
                                  outerGlowColor: Color.Red, duration: 24, baseSize: 0.14f);
                part.Scale *= 1.225f;
                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(Projectile.Bottom, DustID.Torch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                /*
                for (int i = 0; i < SmokeOldCenterPos.Length; i++)
                {
                    Vector2 pos = SmokeOldCenterPos[i];
                    if (i < 8)
                        continue;
                    if (Main.rand.NextBool(4))
                    {
                        Vector2 velocity = -Projectile.oldVelocity;
                        Particle.NewBlackParticle<BlackSmokeParticle>(pos, velocity * 0.5f, Color.White);
                    }
                }*/

                for (float i = 0; i < 15; i++)
                {
                    float rot = rot = -Vector2.UnitY.ToRotation();
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);

                    Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Bottom + offset,
                        innerColor: Color.Yellow,
                        glowColor: Color.Orange,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                        duration: Main.rand.NextFloat(5, 25));
                    particle.Velocity = velocity;
                    particle.Scale *= 0.35f;
                    particle.Rotation = rot;
                }

                FXUtil.ShakeCamera(Projectile.Bottom, 100, 4);
                Vector2 position = Projectile.Bottom;
                Vector2 lvelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero) * 8;
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.Yellow,
                        outerColor: Color.Orange,
                        fadeToColor: Color.Red,
                        distortOut: true);

                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                }
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Bottom, Vector2.Zero);

                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    Dust.NewDustPerfect(Projectile.Bottom, ModContent.DustType<TSmokeDust>(), pVelocity, newColor: Color.Black);
                }
            }
        }
    }
}
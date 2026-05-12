
using Microsoft.Xna.Framework;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Swords
{
    public class Auroran : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 11;
            Item.shoot = ModContent.ProjectileType<AuroranSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<AuroranStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 3;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }


    public class AuroranSlash : BaseSwingProjectileV2
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
                if (Main.myPlayer == Projectile.owner && ComboIndex == ComboCount - 1)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                        ModContent.ProjectileType<Aurora>(), (int)(Projectile.damage * 0.2f), Projectile.knockBack, Projectile.owner);
                }
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
    }

    public class AuroranStaminaSlash : BaseSwingProjectileV2
    {
        float ProjTimer;
        public bool Hit;
        public bool AuroraProj1;
        public bool AuroraProj2;
        public bool AuroraProj3;


        public override void DefineCombo()
        {
            base.DefineCombo();
            useAfterImage = true;
            var swingTrailer = TrailPresets.Auroran;
            swingTrailer.invert = ComboIndex % 2 != 0;
            Trailer = swingTrailer;

            bloomScale = 0.08f;
            useBloom = true;
            bloom.innerBloomColor = Color.White;
            bloom.outerBloomColor = Color.SkyBlue;
            bloom.bloomWidthFunction = GetBloomWidth;
            bloom.bloomColorFunction = GetBloomColor;
            additive = true;
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            Add(new OvalSwing
            {
                Duration = 68,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });

            Add(new OvalSwing
            {
                Duration = 68,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });
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


        private bool _thrust;
        public float thrustSpeed = 5;
        public float stabRange;
        public override void AI()
        {
            base.AI();
            bloomScale = MathHelper.Lerp(0.12f, 0f, EasingFunction.InExpo(Interpolant));
            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Interpolant > 0.5f && !AuroraProj2)
            {
                SoundStyle soundStyle = SoundRegistry.IceyWind;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.LightCoral,
                   outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.LightCoral,
                   outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.2f);

                for (float f = 0; f < 12; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightBlue);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, ModContent.ProjectileType<AuroranBullet>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity * 0.8f, ModContent.ProjectileType<AuroranBullet2>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity * 1.2f, ModContent.ProjectileType<AuroranBullet3>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

                }
                AuroraProj2 = true;
            }

            if (Interpolant > 0.5f)
            {

                if (!_thrust)
                {
                    Owner.velocity += swingDirection * thrustSpeed;
                    _thrust = true;
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

            SoundStyle spearHit = SoundRegistry.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;


            if (ComboIndex < 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }
    }
}
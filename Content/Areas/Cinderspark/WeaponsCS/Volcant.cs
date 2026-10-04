using Microsoft.Xna.Framework;
using Stellamod.Common.Particles;
using Stellamod.Content.Dusts;
using Stellamod.Content.Trailers;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Projectiles.Gun;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class Volcant : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 10;
            Item.ArmorPenetration = 15;
            Item.shoot = ModContent.ProjectileType<VolcantSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<VolcantStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Greatsword;
        }
    }

    public class VolcantStaminaSlash : BaseSwingProjectileV2
    {
        private NPCSucker _npcSucker;
        public override void DefineCombo()
        {

            base.DefineCombo();

            SoundStyle nSpin = SoundRegistry.NSwordSpin1;
            nSpin.PitchVariance = 0.3f;
            Add(new OvalSwing
            {
                Duration = 120,
                SwingDegrees = 360 * 8,
                XSwingRadius = 64,
                YSwingRadius = 64,
                HitCount = 16,
                Easing = (float lerpValue) => lerpValue,
                Sound = nSpin
            });

            Trailer = new IyxFlamingTrail();
            Trailer.TrailWidthFunction = WidthFunction;
            glowAfterImageColor = Color.Red * 0.1f;
            useAfterImage = true;
        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 150, completionRatio) * MathHelper.Lerp(1f, 0f, Interpolant);
        }
        public override void AI()
        {
            base.AI();
            if(Main.rand.NextBool(16) && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Main.rand.NextVector2CircularEdge(8, 8) - Vector2.UnitY * 8, ProjectileID.WandOfSparkingSpark, Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Main.rand.NextVector2CircularEdge(8, 8) - Vector2.UnitY * 8, ModContent.ProjectileType<CinderFlameball>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
            }
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
            _npcSucker ??= new NPCSucker();
            if (Interpolant > 0.5f)
            {
                _npcSucker.AI(Projectile.Center, strength: 0.8f);
            }
        }

    }

    public class VolcantSlash : BaseSwingProjectileV2
    {
        float _flashTimer;
        float _frameCounter;
        int _frame;
        float _oldRot;
        float _travelRotation;
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            Add(new SwordSwing
            {
                baseSwingTime = 62,
                swordMovementFunction = new Swings.CurlSwing
                {
                    frequency = 6,
                    swingRadians = 0.7f,
                    xRange = 144
                },
                hitCount = 8,
                sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
            });

            Add(new SwordSwing
            {
                sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
                baseSwingTime = 62,
                hitCount = 8,
                swordMovementFunction = new Swings.ChainsawSwing
                {
                    arcRadians = 0.15f,
                    backupDistance = 128,
                    thrustDistance = 164
                },
            });
            Add(new SwordSwing
            {
                baseSwingTime = 62,
                swordMovementFunction = new Swings.CurlSwing
                {
                    frequency = 6,
                    swingRadians = 0.7f,
                    xRange = 144
                },
                hitCount = 8,
                sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
            });

            Add(new SwordSwing
            {
                sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
                baseSwingTime = 62,
                hitCount = 8,
                swordMovementFunction = new Swings.ChainsawSwing
                {
                    arcRadians = 0.15f,
                    backupDistance = 128,
                    thrustDistance = 96
                },
            });

            glowAfterImageColor = Color.Red * 0.1f;
            hitStopTime = EXTRA_UPDATE_COUNT * 7;
            useAfterImage = true;
            glowAfterImageColor = Color.OrangeRed;

        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
        }

        public override void AI()
        {
            base.AI();
            if(_flashTimer > 0)
            {
                _flashTimer -= 0.5f;
            }
            _frameCounter += 0.3f;
            while(_frameCounter >= 1f)
            {
                _frameCounter -= 0.3f;
                _frame++;
                _frame %= 4;
            }

            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant) * 0.2f);
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
            if (Main.rand.NextBool(8))
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
                sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkViolet, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
                sp.Scale *= 0.48f;
                sp.behindLayer = true;
            }
        }

        public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
        {
            base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Cinderspark.WeaponsCS.Volcant_Sawblade.Asset, Projectile.Center);
            var alpha = MathHelper.Lerp(0f, 1f, _flashTimer / 10f);
            drawer.VerticalFrame(_frame, 4);
            drawer.LeftCenterOrigin();
            drawer.rotation = Projectile.rotation - MathHelper.PiOver4;
            drawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 6));
            drawer.color = Color.Lerp(drawer.color, Color.Yellow, alpha);
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);

            drawer.scale *= 1.2f;
            drawer.color *= 0.5f;
            Main.spriteBatch.Draw(drawer);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8);
                FXUtil.GlowCircleBoom(target.Center, Color.OrangeRed, Color.Red, Color.DarkRed, duration: 24, baseSize: 0.16f);
  
                _hit = true;
            }
            _flashTimer = 10;
            for (var i = 0; i < 2; i++)
            {
                var pos = target.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45f);
                vel = vel.RotatedByRandom(1.3f);
                var fx = FXUtil.GlowStretch(pos, vel);
                fx.InnerColor = Color.Red;
                fx.OuterGlowColor = Color.DarkRed;
                fx.VectorScale.Y *= 0.4f;
            }
            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.03f);
            HitstopTimer += 12;
            FXUtil.ShakeCamera(target.Center, 1024, 3);
            for (var i = 0; i < 4; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(4.5f) * Main.rand.NextFloat(5f, 25);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.OrangeRed.ToVector4(),
                    outerColor = Color.DarkRed.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
                    timeLeft = Main.rand.NextFloat(30, 90)
                });
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
        }
    }
}
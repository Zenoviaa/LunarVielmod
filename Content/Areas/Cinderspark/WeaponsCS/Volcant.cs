using Microsoft.Xna.Framework;
using Stellamod.Content.Trailers;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Gun;
using Stellamod.Visual.Particles;
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
            Item.damage = 26;
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
        private NPCSucker _npcSucker;
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddGreatswordSwingStyle(this);
            Trailer = new IyxFlamingTrail();
            Trailer.TrailWidthFunction = WidthFunction;
            glowAfterImageColor = Color.Red * 0.1f;
            hitStopTime = EXTRA_UPDATE_COUNT * 8;
        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 128, completionRatio);
        }


        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
            _npcSucker ??= new NPCSucker();
            if (Interpolant > 0.5f)
            {
                _npcSucker.AI(Projectile.Center, strength: 0.8f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.White,
                        outerColor: Color.Yellow,
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
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
            }
        }
    }
}